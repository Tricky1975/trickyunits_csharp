/* 
    NOTES FROM THE ORIGINAL BLITZMAX CODE
Rem
        RPGStats.bmx
	(c) 2015, 2016, 2017, 2018 Jeroen Petrus Broks.
	
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v. 2.0. If a copy of the MPL was not 
	distributed with this file, You can obtain one at 
	http://mozilla.org/MPL/2.0/.
        Version: 18.01.16
End Rem


Version History: BlitzMax
15.02.14 - This "library" has been set up for DuCraL from origin. 
15.05.02 - It was also meant to be used in LAURA II, and to be turned into a mod by then and so it happened.
         - NewParty, ClearParty,RemoveFromParty were added
15.05.10 - Added Countlist, ListValue, PureListValue
15.07.18 - Added linked data support
15.07.27 - Added "New" support to StrData and Stat
15.07.28 - Deprecated Stat.Me and TMe
         - Added StatExists() method to RPGStat
15.07.30 - Added Minimum support for Points
15.08.14 - Added support to ignore scripts (in case this unit is only used in a quick viewing utility)
15.08.23 - Added fieldname returning strings.
15.09.15 - Added ListHas method (I can't believe I forgot that one)
15.10.04 - Added SafeStat() Method
15.10.17 - Fixed issue not saving minimum values in Points.
Version History: C#
19.07.09 - Lua API dummied to make this class usable for multiple engines and not only GALE (which has been dropped, and will only be maintained if people ask for it)
*/


/* This is normal code in BlitzMax, but has no value at all in C#
Strict
Import tricky_units.StringMap
Import gale.multiscript
Import gale.Main
Import jcr6.jcr6main
Import jcr6.zlibdriver
Import tricky_units.MKL_Version
Import tricky_units.TrickyReadString
Import tricky_units.jcr6stringmap
Import brl.max2d

MKL_Version "Tricky's Units - RPGStats.bmx","18.01.16"
MKL_Lic     "Tricky's Units - RPGStats.bmx","Mozilla Public License 2.0"
*/

using UseJCR6;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace TrickyUnits {


    #region Classes needed for quick translation from BlitzMax
    internal class StringMap {
        // This class was only required due to BlitzMax having no real map/dictionary support and using a separate library for this. Translating all the code into "pure" C# code would be too much work, so this solution will have to do.
        static public void MapInsert(StringMap M,string key, string value) {
            M.Map[key] = value;
        }

        public string Value(string key) {
            if (Map.ContainsKey(key)) return Map[key]; else return "";
        }

        static public Dictionary<string, string>.KeyCollection MapKeys(StringMap M) => M.Map.Keys;

        private Dictionary<string, string> Map = new Dictionary<string, string>();
    }

    // This class was only required due to BlitzMax having no real map/dictionary support and using a separate library for this. Translating all the code into "pure" C# code would be too much work, so this solution will have to do.
    internal class TMap {
        private Dictionary<object, object> Map = new Dictionary<object, object>();
        static public void MapInsert(TMap M, object key, object value) => M.Map[key] = value;
        static public object MapValueForKey(TMap M,object key) {
            if (M.Map.ContainsKey(key)) return M.Map[key]; else return null;
            
        }
        static public void ClearMap(TMap M) => M.Map.Clear();
        static public Dictionary<object,object>.KeyCollection MapKeys(TMap M) => M.Map.Keys;
    }
    #endregion

    /// <summary>
    /// You can assign a console function here allowing RPG Var to put debug value onto a debug console. 
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="r"></param>
    /// <param name="g"></param>
    /// <param name="b"></param>
    delegate void RPGCONSOLEVOID(string msg, byte r = 255, byte g = 255, byte b = 255);

    #region Data Classes
    /// <summary>
    /// This class is used for "Points" based statistics such as Hitpoints, Mana points etc.
    /// </summary>
    public class RPGPoints {

        public int Have, Maximum, Minimum;
        public string MaxCopy;

        public void Inc(int a) => Have += a;
        public void Dec(int a) => Have -= a;
    }

    /// <summary>
    /// Used for general stats, such as attack power, and so on!
    /// </summary>
    public class RPGStat {
        public bool Pure = false;
        public string ScriptFile = "";
        public string CallFunction = "";
        public int Value = 0;
        public int Modifier = 0;
    }


    /// <summary>
    /// Contains character data. 
    /// </summary>
    class RPGCharacter {

        public string Name = "";

        public TMap StrData = new TMap();
        public TMap Stats = new TMap();
        public TMap Lists = new TMap();
        public TMap Points = new TMap();

        // Field PortraitBank:TBank // For now taken out of use
        // Field Portrait:TImage // For now taken out of use

        public RPGStat Stat(string St) => (RPGStat)TMap.MapValueForKey(Stats, St);
        public List<string> List(string lst) => (List<string>)TMap.MapValueForKey(Lists, lst);
        public RPGPoints Point(string p) => (RPGPoints)TMap.MapValueForKey(Points, p);
    }
    #endregion

    #region The actual library itself!
    static class RPG {
        // Just to prevent myself some headaches!
        const bool True = true;
        const bool False = false;

        static private readonly bool Chat = true;

        static public bool MustHavePortrait = true;

        static public TJCRDIR RPGJCR;
        static public string RPGJCRDir;

        static public string RPGID;
        static public string RPGEngine;

        static public RPGCONSOLEVOID RPGConsoleWriter;
        static private void ConsoleWrite(string M, byte R = 255, byte G = 255, byte B = 255) {
            RPGConsoleWriter?.Invoke(M, R, G, B);
        }


        /// <summary>
        ///  If set to 'true' the lua scripts tied to a stat will be ignored. (Most of meant for quick viewers)
        /// </summary>

        static public bool RPG_IgnoreScripts = false;

        /* Moved to top of code in its own class
Type RPGPoints
	Field Have,Maximum,Minimum
	Field MaxCopy$
	
	Method Inc(a) Have:+a End Method
	Method Dec(a) Have:-a End Method
	End Type
    */

        /* Moved to top of code in its own class
        Type RPGStat 
            Field Pure
            Field ScriptFile$
            Field CallFunction$
            Field Value
            Field Modifier
            End Type
            */

        /*
         * Moved to top of code in its own class
Rem
bbdoc: Contains character data.
End Rem
Type RPGCharacter
Field Name$
'Field StrData:StringMap = New StringMap
Field StrData:TMap = New TMap
Field Stats:TMap = New TMap
Field Lists:TMap = New TMap
Field Points:TMap = New TMap
Field PortraitBank:TBank
Field Portrait:TImage

Method Stat:RPGStat(St$)
Return RPGStat(MapValueForKey(Stats,St))
End Method

Method List:TList(lst$)
Return TList(MapValueForKey(Lists,lst))
End Method

Method Point:RPGPoints(p$)
Return rpgpoints(MapValueForKey(Points,p))
End Method

End Type
*/
        static public TMap RPGChars = new TMap();
        static public string[] RPGParty = new string[6];


        /// <summary>
        /// returns: The character data tied to the requested tag
        /// </summary>        
        static public RPGCharacter GrabChar(string Ch) => (RPGCharacter)TMap.MapValueForKey(RPGChars, Ch);


        class TMe { // Deprecated. Only included to make sure conflicts never occur.
            string Char = "", Stat = "";
        }

        //Private
        //'Type linkeddata
        //'	Field c1$,c2$,d$
        //'	End Type
        //'Global linkeddatalist:TList = New TList
        class TRPGData {
            public string d="";
        }
        static StringMap dstr(TMap A) {
            //var k = "";
            var ret = new StringMap();
            foreach (string k in TMap.MapKeys(A))
                StringMap.MapInsert(ret, k, ((TRPGData)TMap.MapValueForKey(A, k)).d);

            return ret;
        }

        static TMap ddat(StringMap A) {
            //Local k$
            var ret = new TMap();
            TRPGData t;
            foreach (string k in StringMap.MapKeys(A)) {
                t = new TRPGData();
                t.d = A.Value(k);
                TMap.MapInsert(ret, k, t);
            }
            return ret;
        }

        #region Dummied Lua API
        //Public

        /* This type in BlitzMax is entirely set up to be used in GALE. 
         * Since GALE is no longer used in my C# work, and me wanting to have stuff in a more flexible manner this entire type (or class if you like) will NOT be translated but be dummied in stead.
         * A separate class for the BUBBLE engine (where this code will primarily be used in NALA will be set up separately!

Type RPGLuaAPI ' BLD: Object RPGChar\nThis object contains features you need for RPG Character and statistics

Field Me:TMe = New TMe ' --- no doc. This is deprecated, and not to be used. I only left it in the source to prevent compiler errors. Old BLD desc: Variable with the fields Char and Stat. They can both be used to read the current character and stat being processed. It's best to never assign any data to this. These vars are only to be used by the scripts being called by Stat readers.

Method NewParty(MaxChars) ' BLD: Creates a new party. If MaxChars is not set the default value 6 will be used. There is no "official" limit to the maximum amount of characters in a party, it's just how much memory you have.<p>If you never use this function you have a party of max 6 characters available.
Local m = 6
If maxchars m=maxchars
If maxchars<0 GALE_Error("Negative max party")
RPGParty = New String[m]
End Method

Method ClearParty() ' BLD: Removes all characters from the party
For Local ak=0 Until Len(RPGParty) RPGParty[ak]="" Next
End Method

Method RemoveFromParty(Tag$) ' BLD: The character with the tag specified will be removed from the party and the party will automatically remove the empty spaces.
Local P$[] = New String[Len(RPGParty)]
Local c = 0
Local ak
For ak=0 Until Len(RPGParty)
    If RPGParty[ak]<>Tag And RPGParty[ak]<>"" 
        P[c] = RPGParty[ak]
        c:+1
        EndIf
    Next
RPGParty = P
End Method


Method PartyTag$(pos) ' BLD: Returns the codename of a character on the specific tag
If pos>=Len(RPGParty) Return "***ERROR***"
Return RPGParty[pos]
End Method

Method ReTag(characterold$,characternew$) ' BLD: The tag of a party member will be changed.<br>If this character is currently in the party, the party will automatically be adapted.<br>Please note, if a character already exists at the new tag, it will be overwritten, so use this with CARE!
    Local ch:RPGCharacter = grabchar(characterold$)
    If Not ch Return GALE_Error("Original character doesn't exist",["F,RPGChar.ReTag("+characterold+","+characternew+")"])
    MapInsert RPGChars,characternew,ch
    MapRemove RPGChars,characterold
    For Local i=0 Until Len RPGParty
        If RPGParty[i]=characterold RPGParty[i]=characternew
    Next
End Method	

Method SetParty(pos,Tag$) ' BLD: Assign a party member to a slot. Please note depending on the engine there can be a maximum of slots.
If pos>=Len(RPGParty) Return
RPGParty[pos] = Tag
End Method

Method Portrait(Char$,x,y) ' BLD: Show a character picture if it exists.
Local ch:RPGCharacter = grabchar(char)
If Not ch GALE_Error("Character doesn't exist",["F,RPGChar.Portrait","char,"+char])
If ch.Portrait DrawImage ch.Portrait,x,y 'Else Print "No picture on: "+Char
End Method

Method TrueStat:RPGStat(char$,stat$) ' The true stats and its values. This is not documented as using this is pretty dangerous and should only be done when you know what you are doing.
Local ch:RPGCharacter = grabchar(char)
If Not ch GALE_Error("Character doesn't exist",["F,RPGChar.TrueStat","char,"+char])
Local ST:RPGStat = ch.stat(stat)
If Not st GALE_Error("Stat doesn't exist",["F,RPGChar.TrueStat","char,"+char,"Stat,"+stat])
Return st
End Method

Method CountChars() ' BLD: Returns the number of characters currently stored in the memory. (Not necesarily the amount of party members) :)
Local cnt = 0
For Local k$ = EachIn MapKeys(rpgchars) cnt:+1 Next
Return cnt
End Method

Method CharByNum$(index) ' BLD: Returns the name of the character found at index number 'index'.<P>This function has only been designed for iteration purposes, and can better not be used if you are not sure what you are doing. If the index number is "out of bounds", an empty string will be returend. The first entry is number 0.
Local ret$
Local cnt = 0
For Local k$ = EachIn MapKeys(rpgchars) 
    If cnt=index Return k
    cnt:+1 
    Next
End Method


Method Stat(Char$,Stat$,nomod=0) ' BLD: Returns the stat value
Local ch:RPGCharacter = grabchar(char)
Local csr$
Local lua$ = ".lua"
If Not ch GALE_Error("Character doesn't exist",["F,RPGChar.Stat","char,"+char,"Stat,"+stat])
Local ST:RPGStat = ch.stat(stat)
If Not st GALE_Error("Stat doesn't exist",["F,RPGChar.Stat","char,"+char,"Stat,"+stat])
If st.scriptfile And st.callfunction And (Not RPG_IgnoreScripts)
    Me.Char = Char
    Me.Stat = Stat
    csr = "CHARSTAT:"+Upper(st.ScriptFile)
    If ExtractExt(st.Scriptfile).toupper()="LUA" lua=""		
    If Not GALE_MS.ContainsScript(csr) GALE_MS.Load(csr,RPGJCRDIR+st.Scriptfile+lua)
    If Not GALE_MS.ContainsScript(csr) GALE_Error(RPGJCRDIR+st.Scriptfile+lua+" not loaded correctly!")
    GALE_MS_Run csr,st.callfunction,[Char,Stat]
    Me = New TMe
    EndIf		
Return st.value + (st.modifier * Int(nomod=0))
End Method

Method SafeStat(Char$,Stat$,nomod=0) ' BLD: Returns the stat value, but would the normal Stat() method crash the game if a character or stat does not exist, this one will then return 0
Local ch:RPGCharacter = grabchar(char)
If Not ch Return 0
If ch.stat(stat) Return Self.Stat(char,stat,nomod) Else Return 0	
End Method

Method DefStat(char$,Stat$,value=0,OnlyIfNotExist=0) ' BLD: Defines a value. Please note that if a stat is scripted the scripts it refers to will always use this feature itself to define the value. If "OnlyIfNotExist" is checked to 1 or any higher number than that, the definition only takes place of the stat doesn't exist yet. This was a safety precaution if you want to add stats later without destroying the old data if it exists, but to create it if you added a stat which was (logically) not yet stored in older savegames.
Local ch:RPGCharacter = grabchar(char)
If Not ch GALE_Error("Character doesn't exist",["F,RPGChar.DefStat","char,"+char,"Stat,"+stat,"Value,"+value])
Local ST:RPGStat = ch.stat(stat)
If Not MapContains(ch.Stats,stat)
    St = New RPGStat
    MapInsert ch.Stats,stat,st
ElseIf OnlyIfNotExist
    Return
    End If		
st.Value=Value
End Method

Method NewStat(char$,Stat$,Value) ' BLD: Shortcut to DefStat with OnlyIfNotExist defined ;)
DefStat Char,Stat,Value,True
End Method

Method StatExists(char$,Stat$) ' BLD: Returns 1 if the stat exists, returns 0 if stat does not exist
Local ch:RPGCharacter = grabchar(char)
If Not ch GALE_Error("Character doesn't exist",["F,RPGChar.Stat","char,"+char])
Return ch.stat(stat)<>Null
End Method


Method LinkStat(sourcechar$,targetchar$,statname$) ' BLD: The stat of two characters will be linked. This means that if one stat changes the other will change and vice versa. Upon this definition, the targetchar's stat will be overwritten. After that the targetchar or the sourcechar do not matter any more, they will basically share the same stat. (This feature came to be due to its need in Star Story) :)<p>Should the targetchar's stat not exist it will be created in this function.
Local ch1:RPGCharacter = grabchar(sourcechar)
If Not ch1 GALE_Error("Source Character doesn't exist",["F,RPGChar.LinkStatStat","sourcechar,"+sourcechar,"targetchar,"+targetchar,"stat,"+statname])
Local ch2:RPGCharacter = grabchar(targetchar)
If Not ch2 GALE_Error("Target Character doesn't exist",["F,RPGChar.LinkStatStat","sourcechar,"+sourcechar,"targetchar,"+targetchar,"stat,"+statname])
Local ST:RPGStat = ch1.stat(statname)
If Not ST GALE_Error("Source Character's stat doesn't exist",["F,RPGChar.LinkStatStat","sourcechar,"+sourcechar,"targetchar,"+targetchar,"stat,"+statname])
MapInsert ch2.Stats,statname,ST
End Method

Method LinkList(sourcechar$,targetchar$,statname$) ' BLD: The list of two characters will be linked. This means that if one list changes the other will change and vice versa. Upon this definition, the targetchar's stat will be overwritten. After that the targetchar or the sourcechar do not matter any more, they will basically share the same stat. (This feature came to be due to its need in Star Story) :)<p>Should the targetchar's stat not exist it will be created in this function.
Local ch1:RPGCharacter = grabchar(sourcechar)
If Not ch1 GALE_Error("Source Character doesn't exist",["F,RPGChar.LinkStatStat","sourcechar,"+sourcechar,"targetchar,"+targetchar,"stat,"+statname])
Local ch2:RPGCharacter = grabchar(targetchar)
If Not ch2 GALE_Error("Target Character doesn't exist",["F,RPGChar.LinkStatStat","sourcechar,"+sourcechar,"targetchar,"+targetchar,"stat,"+statname])
Local ST:TList = ch1.list(statname)
If Not ST GALE_Error("Source Character's stat doesn't exist",["F,RPGChar.LinkStatStat","sourcechar,"+sourcechar,"targetchar,"+targetchar,"stat,"+statname])
MapInsert ch2.Lists,statname,ST
End Method

  Method SetStat(char$,Stat$,value=0,OnlyIfNotExist=0) ' BLD: Alias for DefStat
DefStat(char$,Stat$,value,OnlyIfNotExist) 
End Method

Method ScriptStat(char$,stat$,script$,func$) ' BLD: Define the script for a stat. Please note, if either the script or the function is not properly defined the system will ignore the scripting.
Local ch:RPGCharacter = grabchar(char)
If Not ch GALE_Error("Character doesn't exist",["F,RPGChar.ScriptStat","char,"+char])
Local ST:RPGStat = ch.stat(stat)
If Not st GALE_Error("Stat doesn't exist",["F,RPGChar.ScriptStat","Char,"+char,"Stat,"+stat])
st.Scriptfile = Script
st.CallFunction = Func
Print 
End Method

Method ModStat(char,stat,modifier) ' BLD: Set the modifier
End Method

Method SetName(char$,name$) ' BLD: Name the character
Local ch:RPGCharacter = grabchar(char)
If Not ch GALE_Error("Character doesn't exist",["F,RPGChar.Stat","char,"+char])
ch.Name = Name	
End Method

Method GetName$(char$) ' BLD: Retrieve the name of a character
Local ch:RPGCharacter = grabchar(char)
If Not ch GALE_Error("Character doesn't exist",["F,RPGChar.Stat","char,"+char])
Return ch.Name
End Method

Method SetData(char$,key$,str$) ' BLD: Define data in a character
Local ch:RPGCharacter = grabchar(char)
If Not ch 
    GALE_Error("Character doesn't exist",["F,RPGChar.SetData","char,"+char])
    EndIf	
Local td:trpgdata 
If MapContains(ch.strData,key) 
    td = trpgdata(MapValueForKey(ch.strdata,key))
    Else
    td = New trpgdata; 
    MapInsert ch.strdata,key,td
    EndIf
td.d=str
End Method

Method DataExists(char$,key$) ' BLD: Returns 1 if exists, and 0 if it doesn't
Local ch:RPGCharacter = grabchar(char)
If Not ch 
    GALE_Error("Character doesn't exist",["F,RPGChar.SetData","char,"+char])
    EndIf	
'Local td:trpgdata 
Return MapContains(ch.strData,key) 
End Method


Method NewData(Char$,key$,str$) ' BLD: If a data field does not exist, create it and define it. If it already exists, ignore it! (1 is returned if a definition took place, 0 is returned when no definition is done)
Local ch:RPGCharacter = grabchar(char)
If Not ch 
    GALE_Error("Character doesn't exist",["F,RPGChar.SetData","char,"+char])
    EndIf
If Not MapContains(ch.strdata,key) SetData char,key,str; Return True
Return False
End Method

Method LinkData(sourcechar$,targetchar$,dataname$) ' BLD: The stat of two characters will be linked. Works similar to LinkStat but then with Data.
Local ch1:RPGCharacter = grabchar(sourcechar)
If Not ch1 GALE_Error("Source Character doesn't exist",["F,RPGChar.LinkData","sourcechar,"+sourcechar,"targetchar,"+targetchar,"stat,"+dataname])
Local ch2:RPGCharacter = grabchar(targetchar)
If Not ch2 GALE_Error("Target Character doesn't exist",["F,RPGChar.LinkData","sourcechar,"+sourcechar,"targetchar,"+targetchar,"stat,"+dataname])
Local ST:trpgdata = trpgdata(MapValueForKey(ch1.strdata,dataname))
If Not ST GALE_Error("Source Character's data doesn't exist",["F,RPGChar.LinkData","sourcechar,"+sourcechar,"targetchar,"+targetchar,"stat,"+dataname])
MapInsert ch2.strdata,dataname,ST	
End Method


'Method DefData(Char$,K$,S$) Setdata Char,K,S End method

Method GetData$(char$,key$) ' BLD: Retrieve the data in a character
Local ch:RPGCharacter = grabchar(char)
If Not ch GALE_Error("Character doesn't exist",["F,RPGChar.GetData","char,"+char,"Data:"+key])
'Return ch.strdata.value(key)
Local td:trpgdata = trpgdata(MapValueForKey(ch.strdata,key))
If Not td Return ""
Return td.d
End Method

Method Data$(c$,k$) Return GetData(C,K) End Method

Method DelStat(char$,stat$) ' BLD: Delete a stat in a character
    Local ch:RPGCharacter = grabchar(char)
    If Not ch GALE_Error("Character doesn't exist",["F,RPGChar.DelStat","char,"+char,"Stat:"+stat])
    If Not MapContains(ch.stats,stat) 
        ConsoleWrite "DelStat: WARNING! Character "+char+" does not HAVE a stat named: "+stat
        Return
    EndIf
    MapRemove ch.stats,stat
End Method

Method DelData(char$,key$) ' BLD: Delete data in a character	
Local ch:RPGCharacter = grabchar(char)
If Not ch 
    GALE_Error("Character doesn't exist",["F,RPGChar.DelData","char,"+char])
    EndIf
MapRemove ch.strdata,key
End Method

Method CharExists(Char$) ' BLD: Returns 1 if the character exists and returns 0 if it doesn't.<p>Always remember that 0 does count as "true" in Lua.
Return grabchar(char)<>Null
End Method

Method CharList$(Char$) ' BLD: Retuns a string with all codenames of the loaded chars separated by ;
Local ret$
For Local k$=EachIn MapKeys ( RPGChars )
    If ret ret:+";"
    ret:+k
    Next
Return ret
End Method

Method CreateChar(Char$) ' BLD: Create a character (if a character already exists under that name, it will simply be deleted).
MapInsert RPGChars,Char,New RPGCharacter
galecon.galeConsoleWrite "Character ~q"+char+"~q has been created"
End Method

Method DelCharacter(char$) ' BLD: Deletes a character
Local ch:RPGCharacter = grabchar(char)
If Not ch 
    GALE_Error("Character doesn't exist",["F,RPGChar.DelCharacter","char,"+char])
    EndIf
MapRemove rpgchars,char	
End Method

Method DelChar(char$) ' BLD: Alias for DelCharacter. Some people (like me) are just lazy.
delcharacter char
End Method

Method CreateList(char$,List$) ' BLD: Create a list, but don't add any items yet
AddList char,list,"POEP"
ClearList char,list
End Method

Method AddList(char$,List$,Item$) ' BLD: Create a list inside a character record. If the requested list does not yet exist, this function will create it.
Local ch:RPGCharacter = grabchar(char)
If Not ch GALE_Error("Character doesn't exist",["F,RPGChar.AddList","char,"+char,"List,"+List,"Item,"+Item])
If Not ch.list(list) MapInsert ch.lists,List,New TList
ListAddLast ch.list(list),Item
End Method

Method RemList(char$,List$,Item$) ' BLD: Remove an item from a list
Local ch:RPGCharacter = grabchar(char)
If Not ch GALE_Error("Character doesn't exist",["F,RPGChar.AddList","char,"+char,"List,"+List])
Local ls:TList = ch.list(list)
If Not ls GALE_Error("List doesn't exist",["F,RPGChar.AddList","char,"+char,"List,"+List])
ListRemove ls,item
End Method

Method ClearList(char$,List$) ' BLD: Empty a list
Local ch:RPGCharacter = grabchar(char)
If Not ch GALE_Error("Character doesn't exist",["F,RPGChar.ClearList","char,"+char,"List,"+List])
Local ls:TList = ch.list(list)
If Not ls GALE_Error("List doesn't exist",["F,RPGChar.ClearList","char,"+char,"List,"+List])
brl.linkedlist.ClearList ls	
End Method

Method CountList(char$,List$) ' BLD: Count the number of entries in the list
Local ch:RPGCharacter = grabchar(char)
If Not ch GALE_Error("Character doesn't exist",["F,RPGChar.CountList","char,"+char,"List,"+List])
Local ls:TList = ch.list(list)
If Not ls GALE_Error("List doesn't exist",["F,RPGChar.CountList","char,"+char,"List,"+List])
Return brl.linkedlist.CountList(ls)
End Method

Method ValueList$(Char$,List$,Index) ' BLD: Return the value at the index. <br>This routine has been adepted to Lua starting with 1 and ending on the max number!
Return PureValueList(Char$,List$,Index-1)
End Method

Method PureValueList$(Char$,List$,Index)
Local ch:RPGCharacter = grabchar(char)
If Not ch GALE_Error("Character doesn't exist",["F,RPGChar.PureValueList","char,"+char,"List,"+List])
Local ls:TList = ch.list(list)
If Not ls GALE_Error("List doesn't exist",["F,RPGChar.PureValueList","char,"+char,"List,"+List])
If index>=brl.linkedlist.CountList(ls) GALE_Error("List index out of range",["char,"+char,"List,"+List,"Pure Index,"+Index,"Lua Index,"+Int(Index+1)])
Return String(ls.valueatindex(index))
End Method	

Method DestroyList(char$,List$) ' BLD: Destroy the list
Local ch:RPGCharacter = grabchar(char)
If Not ch GALE_Error("Character doesn't exist",["F,RPGChar.DestroyList","char,"+char,"List,"+List])
Local ls:TList = ch.list(list)
If Not ls GALE_Error("List doesn't exist",["F,RPGChar.DestroyList","char,"+char,"List,"+List])
MapRemove ch.lists,list
End Method

Rem
Method ListLen(Char$,List$) ' -- BLD: Return the number of items in a list. If the list doesn't exist it returns 0.
Local ch:RPGCharacter = grabchar(char)
If Not ch GALE_Error("Character doesn't exist",["F,RPGChar.CountList","char,"+char,"List,"+List]); Return 0
Local ls:TList = ch.list(list)
If Not ls Return 0
Return CountList(ls)
End Method
End Rem


Method ListItem$(char$,List$,Index) ' BLD: Return the item at an index on the list. When the index number is too high an error will pop up.<p>Please note, the api in this method has been adepted to Lua, so it starts with 1, and ends with the countnumber of the list. 0 is therefore not taken as a valid index! Always remember it!
Local ch:RPGCharacter = grabchar(char)
If Not ch GALE_Error("Character doesn't exist",["F,RPGChar.CountList","char,"+char,"List,"+List])
Local ls:TList = ch.list(list)
If Not ls GALE_Error("List doesn't exist",["F,RPGChar.CountList","char,"+char,"List,"+List])
If index<1 Or index>CountList(char,list) GALE_Error("List index out of bounds!",["F,RPGChar.Listitem","Char,"+char,"List,"+list,"Index,"+Index,"Allowed rage,1 till "+CountList(char,list)])
Local TrueIndex = Index - 1
Return String(ls.valueatindex(TrueIndex))
End Method

Method ListExist(char$,List$) ' BLD: returns 1 if the character and the list tied to that character exist otherwise it returns 0
Local ch:RPGCharacter = grabchar(char)
If Not ch Return 0
If ch.list(list) Return 1
End Method

Method PointsExists(char$,points$)
Local ch:RPGCharacter = grabchar(char)
If Not ch 
    L_ConsoleWrite "WARNING! PointsExist(~q"+Char+"~q,~q"+points+"~q): Character doesn't exist. Returning False anyway"
    Return False
    EndIf
Return MapContains(ch.points,points)
End Method

Method Points:RPGPoints(char$,points$,docreate=0) ' BLD: Points. Has two fields. "Have" and "Maximum". These can be used for Hit Points, Mana, Skill Points, maybe even experience points. Whatever. There is also a field called "MaxCopy" which is a string. When you copy this field, the system will always copy the value of the stat noted in this string for the Maximum value. When the value "docreate" is set to 1 the system will create a new value if it doesn't exist yet, when it's set to 2, it will always create a new value and destroy the old, any other value will not allow creation and cause an error if the points do not exist.
Local ch:RPGCharacter = grabchar(char)
If Not ch GALE_Error("Character doesn't exist",["F,RPGChar.Points","char,"+char,"Points,"+Points])
Select DoCreate
    Case 1
        If Not MapContains(ch.points,points) MapInsert ch.points,points,New RPGPoints
    Case 2
        MapInsert ch.points,points,New RPGPoints
    Case 255
        If Not MapContains(ch.points,points) Return Null ' value 255 is not documented as it's used internally, and usage in Lua would most likely lead to crashes without a proper error message.	
    End Select
Local p:RPGPoints = RPGPoints(MapValueForKey(ch.points,points))
If Not p GALE_Error("Points could not be retrieved",["F,RPGChar.Points","Char,"+Char,"Points,"+Points,"DoCreate,"+docreate])
'Print "Points call: "+char+","+Points+","+DoCreate+";     p.Have="+p.have+"; p.Maximum="+p.Maximum+"; p.maxcopy="+p.MaxCopy
If p.maxcopy
    p.maximum = stat(char,p.maxcopy)
    EndIf
If p.have>p.maximum p.have=p.maximum
If p.have<p.minimum p.have=p.minimum
If p.minimum>p.maximum Then GALE_Error("Points minimum bigger than maximum! How come?",["Char,"+char,"Points,"+points,"Minimum,"+p.minimum,"Maximum,"+p.maximum])
Return p
End Method

Method LinkPoints(sourcechar$,targetchar$,pointsname$) ' BLD: The stat of two characters will be linked. This means that if one stat changes the other will change and vice versa. Upon this definition, the targetchar's stat will be overwritten. After that the targetchar or the sourcechar do not matter any more, they will basically share the same stat. (This feature came to be due to its need in Star Story) :)<p>Should the targetchar's stat not exist it will be created in this function.
Local ch1:RPGCharacter = grabchar(sourcechar)
If Not ch1 GALE_Error("Source Character doesn't exist",["F,RPGChar.LinkStatStat","sourcechar,"+sourcechar,"targetchar,"+targetchar,"stat,"+pointsname])
Local ch2:RPGCharacter = grabchar(targetchar)
If Not ch2 GALE_Error("Target Character doesn't exist",["F,RPGChar.LinkStatStat","sourcechar,"+sourcechar,"targetchar,"+targetchar,"stat,"+pointsname])
Local ST:RPGPoints = Points(sourcechar,pointsname)
If Not ST GALE_Error("Source Character's points doesn't exist",["F,RPGChar.LinkStatStat","sourcechar,"+sourcechar,"targetchar,"+targetchar,"stat,"+pointsname])
MapInsert ch2.Points,pointsname,ST
End Method

Method IncStat(char$,Statn$,value=1) ' BLD: Increases a stat by the given number. If value is either 0 or undefined, the stat will be increased by 1
Local v=value
If v=0 v=1
DefStat char,statn,stat(char,statn)+v
End Method

Method DecStat(char$,Statn$,value=1) ' BLD: Decreases a stat by the given number. If value is either 0 or undefined, the stat will be decreased by 1
Local v=value
If v=0 v=1
DefStat char,statn,stat(char,statn)-v
End Method

Method StatFields$(char$) ' BLD: Returns a string with all statistics fieldnames separated by ";". It is recommended to use a split function to split it (if you don't have one I'm sure you can find some scripts for that if you google for that).
Local ret$
Local ch:RPGCharacter = grabchar(char)
If Not ch GALE_Error("Character doesn't exist",["F,RPGChar.StatFields","char,"+char])
For Local K$=EachIn MapKeys(ch.stats)
    If ret ret:+";"
    ret:+k
    Next
Return ret
End Method

Method ListFields$(char$) ' BLD: Returns a string with all list fieldnames separated by ";". It is recommended to use a split function to split it (if you don't have one I'm sure you can find some scripts for that if you google for that).
Local ret$
Local ch:RPGCharacter = grabchar(char)
If Not ch GALE_Error("Character doesn't exist",["F,RPGChar.ListFields","char,"+char])
For Local K$=EachIn MapKeys(ch.lists)
    If ret ret:+";"
    ret:+k
    Next
Return ret
End Method

Method ListHas(char$,list$,itemstring$) ' BLD: returns 1 if the item was found in the list. If the list or the item does not exist it returns 0
Local ch:RPGCharacter = grabchar(char)
If Not ch GALE_Error("Character doesn't exist",["F,RPGChar.ListHas","char,"+char,"list,"+list,"itemstring,"+itemstring])
If Not MapContains(ch.lists,list) Return
Return ListContains(ch.list(list),itemstring)
End Method

Method ListOut$(char$,list$,separator$=";") ' BLD: Puts all item sof a list in a string diveded by ; by default, unless you set a different separator
If Not separator separator=";"
Local ret$
Local ch:RPGCharacter = grabchar(char)
If Not ch GALE_Error("Character doesn't exist",["F,RPGChar.ListOut","char,"+char,"list,"+list,"separator,"+separator])
If Not MapContains(ch.lists,list) Return
For Local i$ = EachIn ch.list(list)
    If ret ret:+";"
    ret:+i
Next
Return ret
End Method		

Method DataFields$(char$) ' BLD: Returns a string with all stringdata fieldnames separated by ";". It is recommended to use a split function to split it (if you don't have one I'm sure you can find some scripts for that if you google for that).
Local ret$
Local ch:RPGCharacter = grabchar(char)
If Not ch GALE_Error("Character doesn't exist",["F,RPGChar.DataFields","char,"+char])
For Local K$=EachIn MapKeys(ch.strdata)
    If ret ret:+";"
    ret:+k
    Next
Return ret
End Method

Method PointsFields$(char$) ' BLD: Returns a string with all stringdata fieldnames separated by ";". It is recommended to use a split function to split it (if you don't have one I'm sure you can find some scripts for that if you google for that).
Local ret$
Local ch:RPGCharacter = grabchar(char)
If Not ch GALE_Error("Character doesn't exist",["F,RPGChar.PointsFields","char,"+char])
For Local K$=EachIn MapKeys(ch.Points)
    If ret ret:+";"
    ret:+k
    Next
Return ret
End Method

Method ListChars$() ' BLD: Returns a string with all character codenames of characters currently stored in the memory. (They do not need to be in the party right now). The names are separated by ;
Local ret$
For Local K$= EachIn MapKeys(RPGChars)
    If ret ret:+";"
    ret:+k
    Next
End Method	

End Type	

Global RPGChar:RPGLuaAPI = New RPGLuaAPI

GALE_Register RPGChar,"RPGChar"	
GALE_Register RPGChar,"RPGStat"
GALE_Register RPGChar,"RPGStats"
*/
        #endregion

        static private void DebugLog(string L) => Debug.WriteLine(L);

        /// <summary>
        /// Loads all RPG data from a JCR directory.
        /// The database vars are part of this module, and will only return "True" if succesful"
        /// </summary>
        static public void RPGLoad(TJCRDIR LoadFrom, string Dir = "") {
            var D = Dir.Replace("\\", "/"); if (D != "" && qstr.Right(D, 1) != "/") D += "/";
            QuickStream BT;
            int ak;
            //string F = "";
            string P = "", TN = "";
            var LChars = new List<string>();
            RPGCharacter ch;
            int tag;
            RPGStat sv = null; // Compiler is stubborn and stupid, as this '=null' is NOT needed at all!
            RPGPoints sp = null;
            DebugLog($"Loading party: {Dir}");
            // Load party members
            BT = new QuickStream(LoadFrom.AsMemoryStream($"{D}Party"));
            ak = 0;
            RPGParty = new string[BT.ReadInt()];
            while (BT.EOF) {
                if (ak >= RPGParty.Length) {
                    DebugLog("WARNING! Too many party members in party!");
                    break;
                }
                RPGParty[ak] = BT.ReadString();
                DebugLog($"Party Member #{ak}> {RPGParty[ak]}");

                ak++;
            }
            BT.Close();
            TMap.ClearMap(RPGChars);
            // Let's first determine which characters we actually have?
            foreach (string F in LoadFrom.Entries.Keys) {
                P = $"{D.ToUpper()}CHARACTER/";
                if (qstr.Left(F, qstr.Len(P)) == P && qstr.StripDir(F) == "NAME") {
                    TN = qstr.ExtractDir(LoadFrom.Entries[F].Entry);
                    TN = qstr.StripDir(TN);
                    LChars.Add(TN);
                }
            }
            // Let's now load the characters
            foreach (string F in LChars) {
                ch = new RPGCharacter();
                TMap.MapInsert(RPGChars, F, ch);
                // Name
                BT = new QuickStream(LoadFrom.AsMemoryStream($"{D}Character/" + F + "/Name"));
                ch.Name = BT.ReadString();
                BT.Close();
                // Data
                ch.strdata = ddat(LoadStringMap(LoadFrom, D + "Character/" + F + "/StrData"));
                // Stats
                BT = new QuickStream(LoadFrom.AsMemoryStream(D + "Character/" + F + "/Stats"));
                while (!BT.EOF) {
                    tag = BT.ReadByte();
                    //Print tag
                    switch (tag) {
                        case 1:
                            TN = BT.ReadString();
                            //Print TN
                            sv = new RPGStat();
                            TMap.MapInsert(ch.Stats, TN, sv);
                            break;
                        case 2:
                            sv.Pure = (BT.ReadByte() != (byte)0);
                            break;
                        case 3:
                            sv.ScriptFile = BT.ReadString();
                            sv.CallFunction = BT.ReadString();
                            break;
                        case 4:
                            sv.Value = BT.ReadInt();
                            break;
                        case 5:
                            sv.Modifier = BT.ReadInt();
                            break;
                        default:
                            //EndGraphics
                            throw new System.Exception($"FATAL ERROR:~n~nUnknown tag in character ({F}) stat file ({tag}) within this savegame file ");
                    }
                }
                BT.Close();
                // Lists
                BT = new QuickStream(LoadFrom.AsMemoryStream(D + "Character/" + F + "/Lists"));
                while (!BT.EOF) {
                    tag = BT.ReadByte();
                    switch (tag) {
                        case 1:
                            TN = BT.ReadString();
                            TMap.MapInsert(ch.Lists, TN, new List<string>());
                            break;
                        case 2:
                            ch.List(TN).Add(BT.ReadString());
                            break;
                        default:
                            // EndGraphics
                            throw new System.Exception($"FATAL ERROR:~n~nUnknown tag in character ({F}) list file ({tag}) within this savegame file ");
                    }
                }
                BT.Close();
                // Points
                BT = new QuickStream(LoadFrom.AsMemoryStream(D + "Character/" + F + "/Points"));
                while (!BT.EOF) {
                    tag = BT.ReadByte();
                    switch (tag) {
                        case 1:
                            sp = new RPGPoints();
                            TN = BT.ReadString();
                            TMap.MapInsert(ch.Points, TN, sp);
                            break;
                        case 2:
                            sp.MaxCopy = BT.ReadString();
                            break;
                        case 3:
                            sp.Have = BT.ReadInt();
                            break;
                        case 4:
                            sp.Maximum = BT.ReadInt();
                            break;
                        case 5:
                            sp.Minimum = BT.ReadInt();
                            break;
                        default:
                            //EndGraphics
                            throw new System.Exception($"FATAL ERROR:~n~nUnknown tag in character ({F}) points file ({tag}) within this savegame file ");
                    }
                }
                BT.Close();
                // For the time being no portrait support!
                // Portrait
                //If JCR_Exists(loadfrom,D+"Character/"+F+"/Portrait.png")	
                //	ch.portraitbank = JCR_B(LoadFrom,D+"Character/"+F+"/Portrait.png")	
                //	ch.portrait = LoadImage(ch.portraitbank)	
                //	If Not ch.portrait And MustHavePortrait
                //		EndGraphics
                //			Notify "FATAL ERROR:~n~nPortrait not well retrieved"
                //			End
                //EndIf
                //		EndIf
                //	Next	
                string linktype = "", linkch1 = "", linkch2 = "", linkstat = "";
                if (LoadFrom.Exists(D + "Links")) {
                    BT = new QuickStream(LoadFrom.AsMemoryStream(D + "Links"));
                    do { //Repeat
                        tag = BT.ReadByte();
                        switch (tag) {
                            case 001:
                                linktype = BT.ReadString();
                                linkch1 = BT.ReadString();
                                linkch2 = BT.ReadString();
                                linkstat = BT.ReadString();
                                switch (linktype.ToUpper()) {
                                    case "STAT": RPGChar.LinkStat(linkch1, linkch2, linkstat); break;
                                    case "PNTS": RPGChar.LinkPoints(linkch1, linkch2, linkstat); break;
                                    case "DATA": RPGChar.LinkData(linkch1, linkch2, linkstat); break;
                                    case "LIST": RPGChar.LinkList(linkch1, linkch2, linkstat); break;
                                    default: DebugLog("ERROR! I don't know what a " + linktype + " is so I cannot link!"); break;
                                }
                                break;
                            case 255:
                                goto GetOutOfThisLinkLoop; // Nothing I hate more than "goto" commands, but due to the way switches are set up in C-dialects, I got no choice!
                            default:
                                throw new System.Exception($"ERROR! Unknown link command tag ({tag})");

                        }
                    } while (!BT.EOF); //Until Eof(bt)
                GetOutOfThisLinkLoop:
                    BT.Close();
                }
            }
            //return true;
        }

        static public string RPGStr() { // Undocumented.  I don't remember why I brought this in. Must be for debugging. When I see the number of cosmetic errors (just by reading the code), I even think this was never tested or used. I just translated it for safety's sake!
            var ret = new StringBuilder(1);
            RPGCharacter ch;
            foreach (string P in RPGParty) {
                ret.Append($"\nParty:{P}");
            }
            foreach (string key in TMap.MapKeys(RPGChars)) {
                ch = (RPGCharacter)TMap.MapValueForKey(RPGChars, key);
                if (ch == null)
                    DebugLog("WARNING! A wrong record in the chars map");
                else {
                    // Name
                    ret.Append("\nNEW");
                    ret.Append("\n\t" + ch.Name);
                    // Data
                    foreach (string k in TMap.MapKeys(ch.StrData)) ret.Append("\n\tD(" + k + ")=" + dstr(ch.StrData).Value(k);
                    // Stats
                    foreach (string skey in TMap.MapKeys(ch.Stats)) {
                        var v = ch.Stat(skey);
                        ret.Append("\n\tSt");
                        ret.Append("\n\t\tskey=" + skey);
                        ret.Append($"\n\t\tpure={v.Pure}");
                        ret.Append("\n\t\tScript=" + v.ScriptFile);
                        ret.Append("\n\t\tFunction=" + v.CallFunction);
                        ret.Append("\n\t\tValue=" + v.Value);
                        ret.Append("\n\t\tModifier=" + v.Modifier);
                    }
                    // Lists
                    ret.Append("\n\tLists");
                    foreach (string lkey in TMap.MapKeys(ch.Lists)) {
                        ret.Append("\t\t" + lkey);
                        foreach (string item in ch.List(lkey)) {
                            ret.Append("\t\t\t" + item);
                        }
                    }
                    // Points
                    ret.Append("\n\tPoints");
                    foreach (string pkey in TMap.MapKeys(ch.Points)) {
                        ret.Append("\n\t\tkey " + pkey);
                        ret.Append("\n\t\tmaxcopy " + ch.Point(pkey).MaxCopy);
                        ret.Append("\n\t\thave " + ch.Point(pkey).Have);
                        ret.Append("\n\t\tmax " + ch.Point(pkey).Maximum);
                    }
                }
            }
            return ret.ToString();
        }

        static private void SaveRPGLink(TJCRCreateStream BTE, string ltype, string ch1, string ch2, string stat) {
            BTE.WriteByte(1); //' marks new entry version 1
            BTE.WriteString(ltype);
            BTE.WriteString(ch1);
            BTE.WriteString(ch2);
            BTE.WriteString(stat);
        }


Rem 
bbdoc: Saves the currently available RPG characters and party data
End Rem
Function RPGSave(SaveTo:Object,Dir$="")
Local BT:TJCRCreate
Local D$ = Replace(Dir,"\","/")
Local BTE:TJCRCreateStream
If TJCRCreate(SaveTo)
	BT = TJCRCreate(Saveto)
ElseIf String(SaveTo)
	BT = JCR_Create(String(SaveTo))
Else
	GALE_Error "Unknown object to save RPG stats to"
	EndIf
If D And Right(D,1)<>"/" D:+"/"
' Save Party members
BTE = BT.CreateEntry(D+"Party","zlib")
WriteInt BTE.Stream,Len(RPGParty)
For Local P$=EachIn RPGParty
	TrickyWriteString BTE.Stream,P
	Next
BTE.Close()
' Save all characters
Local ch:RPGCharacter
For Local key$=EachIn MapKeys(RPGChars)
	ch = RPGCharacter(MapValueForKey(RPGChars,key))
	If Not ch 
		Print "WARNING! A wrong record in the chars map"
		Else
		' Name
		BTE = BT.CreateEntry(D+"Character/"+key+"/Name","zlib")
		TrickyWriteString BTE.Stream, ch.Name
		BTE.Close()
		' Data
		SaveStringMap(BT,D+"Character/"+key+"/StrData",dstr(ch.strdata),"zlib")
		' Stats
		BTE = BT.CreateEntry(D+"Character/"+key+"/Stats","zlib")
		For Local skey$=EachIn MapKeys(ch.Stats)
			Local v:rpgstat = ch.stat(skey)
			WriteByte bte.stream,1
			TrickyWriteString bte.stream,skey
			WriteByte bte.stream,2
			WriteByte bte.stream,v.pure
			WriteByte bte.stream,3
			TrickyWriteString bte.stream,v.scriptfile
			TrickyWriteString bte.stream,v.callfunction
			WriteByte bte.stream,4
			WriteInt bte.stream,v.Value
			WriteByte bte.stream,5
			WriteInt bte.stream,v.modifier
			Next
		BTE.Close()
		' Lists
		BTE = BT.CreateEntry(D+"Character/"+key+"/Lists","zlib")
		For Local lkey$=EachIn MapKeys(ch.lists)
			WriteByte bte.stream,1
			TrickyWriteString bte.stream,lkey
			For Local item$=EachIn ch.list(lkey)
				WriteByte bte.stream,2
				TrickyWriteString bte.stream,item
				Next
			Next
		BTE.Close()	
		' Points
		BTE = BT.CreateEntry(D+"Character/"+key+"/Points","zlib")
		For Local pkey$=EachIn MapKeys(ch.points)
			WriteByte bte.stream,1
			TrickyWriteString bte.Stream,pkey
			WriteByte bte.stream,2
			TrickyWriteString bte.Stream,ch.point(pkey).maxcopy
			WriteByte bte.stream,3
			WriteInt bte.stream,ch.point(pkey).have
			WriteByte bte.stream,4
			WriteInt bte.stream,ch.point(pkey).maximum
			WriteByte bte.stream,5
			WriteInt bte.stream,ch.point(pkey).minimum
			Next
		BTE.Close()	
		' Picture
		If ch.portraitbank
			bt.addentry ch.portraitbank,D+"Character/"+key+"/Portrait.png","zlib"
			EndIf
		EndIf
	Next
' If there are any links list them in the save file
BTE = BT.CreateEntry(D+"Links","zlib")	
Local ch1$,ch2$,stat$,och1:RPGCharacter,och2:RPGCharacter
For ch1=EachIn MapKeys(RPGChars) For ch2=EachIn MapKeys(RPGChars)
	If ch1<>ch2
		och1=RPGCharacter(MapValueForKey(RPGChars,ch1))
		och2=RPGCharacter(MapValueForKey(RPGChars,ch2))
		For stat=EachIn MapKeys(och1.stats)
			If och1.stat(stat)=och2.stat(stat) SaveRPGLink BTE,"Stat",ch1,ch2,stat
			Next
		For stat=EachIn MapKeys(och1.strdata)
			If MapValueForKey(och1.strdata,stat)=MapValueForKey(och2.strdata,stat) SaveRPGLink BTE,"Data",ch1,ch2,stat
			Next
		For stat=EachIn MapKeys(och1.points)
			If MapValueForKey(och1.points,stat)=MapValueForKey(och2.points,stat) SaveRPGLink BTE,"PNTS",ch1,ch2,stat
			Next
		For stat=EachIn MapKeys(och1.lists)
			If MapValueForKey(och1.lists,stat)=MapValueForKey(och2.Lists,stat) SaveRPGLink BTE,"LIST",ch1,ch2,stat
			Next
		EndIf	
	Next Next
WriteByte bte.stream,255
bte.close
' Close if needed
If String(SaveTo) BT.Close()	
End Function
