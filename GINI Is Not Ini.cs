// Lic:
//   GINI Is Not Ini.cs
//   
//   version: 18.11.13
//   Copyright (C) 2018 Jeroen P. Broks
//   This software is provided 'as-is', without any express or implied
//   warranty.  In no event will the authors be held liable for any damages
//   arising from the use of this software.
//   Permission is granted to anyone to use this software for any purpose,
//   including commercial applications, and to alter it and redistribute it
//   freely, subject to the following restrictions:
//   1. The origin of this software must not be misrepresented; you must not
//      claim that you wrote the original software. If you use this software
//      in a product, an acknowledgment in the product documentation would be
//      appreciated but is not required.
//   2. Altered source versions must be plainly marked as such, and must not be
//      misrepresented as being the original software.
//   3. This notice may not be removed or altered from any source distribution.
// EndLic
using System;
using System.Collections.Generic;
using System.IO;

namespace TrickyUnits
{
    /*
     * GINI is a very simple config parser and writer, designed for quickly 
     * saving and loading configuration files, which should be readable for the 
     * human eye. 
     * 
     * GINI is by far not as sophisticated as existing formats such as ini or
     * json, but that was not my aim at the time. It had to be as simple as
     * possible, and as far as my own needs were concerned, that mission has
     * been accomplished.
     * 
     * GINI is an open format, and as long as you respect that part of GINI,
     * this code can be used under the terms of the zlib, and yeah, you may
     * write your own GINI parser... No problem at all. ;)
     * 
     */

    /// <summary>This type is used by GINI to store stuff in.</summary>
    class TGINI
    {

        Dictionary<string, string> vars = new Dictionary<string, string>();
        Dictionary<string, List<string>> lists = new Dictionary<string, List<string>>();
        //lists map[string] qll.StringList
        //init bool


        public TGINI()
        {
            var g = this;
            //fmt.Println("Init new GINI variable")
            //fmt.Printf("before %s\n",g.vars)
            //g.init = true
            //g.vars = map[string] string{ }
            //g.lists = make([][]string,0)
            //g.listpointer = map[string]int{ }
            //g.lists = map[string] []string{}
            //g.lists = map[string] qll.StringList{}
            //fmt.Printf("after %s\n",g.vars)
            GINI.DBGChat("TGINI class created");
        }

        ~TGINI()
        {
            GINI.DBGChat("TGINI class destroyed");
        }

        /// <summary>Returns an array with all variable names!</summary>
        public string[] Vars(){
            var tret = new List<string>();
            foreach (string key in vars.Keys) tret.Add(key);
            tret.Sort();
            return tret.ToArray();
        }

        /// <summary>Define var</summary>
        public void D(string s, string v)
        {
            //g.init1st()
            //g.vars  = make(map[string] string) // debug!
            vars[s.ToUpper()] = v;
        }

        /// <summary>
        /// Read (call) var
        /// </summary>
        /// <returns>The variable's value</returns>
        /// <param name="s">Variable name</param>
        public string C(string s)
        {
            var g = this;//g.init1st(); 
            //if v,ok:=g.vars[strings.ToUpper(s)];ok {
            if (g.vars.ContainsKey(s.ToUpper()))
            {
                return g.vars[s.ToUpper()];
            }
            else
            {
                return "";
            }
        }

        /// <summary>Creates a list if needed</summary> 
        public void CL(string a, bool onlyifnotexist = true)
        {
            //var g = this; //g.init1st(); 
            var ca = a.ToUpper();
            if (lists.ContainsKey(ca)) {
                if (onlyifnotexist)
                {
                    return;

                }
            }
            //GTK.QuickGTK.Warn($"Creating {ca} // {onlyifnotexist} "); // DEBUG ONLY!!!
            //fmt.Printf("Creating list: %s\n",a)
            //g.lists[strings.ToUpper(a)] = qll.CreateStringList() // make([]string,0)
            //g.lists[strings.ToUpper(a)] = make([]string,0)
            lists[ca] = new List<string>();
            //g.listpointer[strings.ToUpper(a)] = len(g.lists) - 1
        }

        /// <summary>
        /// Add value to a list. If not existent create the list.
        /// </summary> 
        public void Add(string nlist, string value)
        {
            CL(nlist, true);
            var cl = nlist.ToUpper();
            lists[cl].Add(value);
            //qll.StringListAddLast(&(g.lists[l]),value)
        }

        /// <summary> Just returns the list. Creates it if it doesn't yet exist!</summary>
        public List<string> List(string nlist)
        {
            CL(nlist, true);
            //lists[strings.ToUpper(nlist)]
            return lists[nlist.ToUpper()];
        }

        /// <summary>
        /// Returns 'true' if a list exist, otherwise it returns false (duh!)
        /// </summary>
        /// <returns><c>true</c>, if exists was listed, <c>false</c> otherwise.</returns>
        /// <param name="list">List reference name</param>
        public bool ListExists(string list) => lists.ContainsKey(list.ToUpper());

        /// <summary>
        /// Returns the string at the given index.
        ///  If out of bounds an empty string is returned
        /// </summary>
        /// <returns>The string stored at that specific index.</returns>
        /// <param name="list">List referrence.</param>
        /// <param name="idx">Index number.</param>
        public string ListIndex(string list, int idx)
        {
            var l = List(list);
            if (idx < 0 || idx >= l.Count) { return ""; }
            return l[idx];
        }

        /// <summary>Duplicates the pointer of a list to a new list name
        /// If the original list does not exist the request will be ignored!
        /// Also note if the target destination already has a list it will remain there
        /// And the garbage collector won't pick it up unless the entire GINI var is destroyed)
        /// </summary>
        public void ListDupe(string source, string target)
        {
            var cs = source.ToUpper();
            var ct = source.ToUpper();

            if (ListExists(cs))
            {
                lists[ct] = lists[cs];
            }
        }

        // Parses the lines of a text-based GINI file into the GINI data
        // Please note this method is for merging data purposes, if you don't
        // want to merge, use the regular functions ;)
        public void ParseLines(string[] l)
        {
            // this entire function has been translated from BlitzMax, however the [OLD] tag has been removed. (it was deprecated anyway).
            //g.init1st()
            //lst:=make([]string,0)
            var lst = new List<string>();
            //var lst //qll.StringList
            var tag = "";
            var tagsplit = new string[0];
            //tagparam:=make([] string,0)
            var tline = "";
            var cmd = "";
            var para = "";
            var pos = 0;
            var line = "";
            var listkeys = new string[0];
            var linenumber = 0;// Not present in BMax, but required in go, and makes it even easier of debugging too :P (And for C# I made a different loop function JUST for the ocassion).
            for (linenumber = 0; linenumber < l.Length; linenumber++)
            {
                line = l[linenumber];
                if (line != "")
                {
                    if (qstr.Left(qstr.MyTrim(line), 1) == "[" && qstr.Right(qstr.MyTrim(line), 1) == "]")
                    {
                        var wTag = qstr.Mid(qstr.MyTrim(line), 2, qstr.Len(qstr.MyTrim(line)) - 2);
                        if (wTag.ToUpper() == "OLD")
                        {
                            throw new Exception($"[OLD] has been removed in the Go and C# versions of GINI! (line #{linenumber})");
                            //fmt.Printf("ERROR! The [old] tag is NOT supported in this Go version of GINI (and in the original BlitzMax version it's deprecated) in line %d", linenumber)
                            //return
                        }
                        tagsplit = wTag.Split(':');
                        tag = tagsplit[0].ToUpper();
                        if (tagsplit[0].ToUpper() == "LIST")
                        {
                            if (tagsplit[0].Length < 2)
                            {
                                throw new Exception($"ERROR! Incorrectly defined list in line #{linenumber}");
                                //return;
                            }
                            //lst = qll.CreateStringList() //make([] string,0)
                            //                    lst = len(g.lists) //[]string{}
                            //g.lists = append(g.lists,[]string{})
                            var templist = new List<string>();
                            lst = templist;
                            listkeys = tagsplit[1].Split(',');
                            foreach (string K in listkeys)
                            {
                                //'ini.clist(UnIniString(K))
                                //fmt.Printf("Creating list: %s\n",K)
                                //g.lists[strings.ToUpper(UnIniString(K))] = lst
                                //g.listpointer[strings.ToUpper(UnIniString(K))] = lst
                                lists[K.ToUpper()] = templist;
                            } //Next
                              //'lst=ini.list(UnIniString(K)) 
                        }//EndIf
                    }
                    else
                    {
                        switch (tag)
                        { //Select tag
                            case "REM":
                                break;
                            case "SYS":
                            case "SYSTEM":
                                tline = qstr.MyTrim(line);
                                pos = tline.IndexOf(' ', 0); //tline.find(" ")
                                if (pos <= -1)
                                {
                                    pos = tline.Length;
                                }
                                cmd = qstr.Left(tline, pos).ToUpper();
                                para = qstr.Mid(tline, pos, tline.Length - pos);
                                //cmd  = strings.ToUpper(tline[:pos])
                                //para = tline[pos + 1:]
                                switch (cmd)
                                {
                                    case "IMPORT":
                                    case "INCLUDE":
                                        //pos = strings.IndexAny(para,"/") //para.find("/")<0
                                        /*
                                        ?win32
                                        pos = pos And Chr(para[1])<>":"
                                        pos = pos And para.find("\")
                                        ?
                                        */
                                        /*
                                        if pos>0 {
                                            para=filepath(String(file))+"/"+para
                                        }
                                        */
                                        /*
                                        ?debug
                                        Print "Including: "+para
                                        ?
                                        */
                                        //g.ReadFile(para) //LoadIni para,ini
                                        Console.WriteLine($"Line {linenumber} -- WARNING\nNo support yet for file inclusion or importing\n");
                                        break;
                                    default:
                                        throw new Exception($"System command {cmd} not understood: {tline} in line #{linenumber}\n");
                                } //End Select   
                                break;
                            case "VARS":
                                if (line.IndexOf('=') < 0)
                                {
                                    Console.WriteLine($"Warning! Invalid var definition: {line} in line #{linenumber} \t=> {line}\n");
                                }
                                else
                                {
                                    //tagsplit=line.split("=")
                                    var temppos = line.IndexOf('=');
                                    tagsplit = new string[2];
                                    tagsplit[0] = qstr.Left(line, temppos);
                                    tagsplit[1] = qstr.Right(line, line.Length - temppos -1 );
                                    D(GINI.UnIniString(GINI.UnIniString(tagsplit[0])), GINI.UnIniString(tagsplit[1]));
                                } //EndIf
                                break;
                            case "LIST":
                                //g.lists[lst] = append(g.lists[lst], UnIniString(line)) //ListAddLast lst,uninistring(line)
                                lst.Add(GINI.UnIniString(line));
                                break;
                            case "CALL":
                                Console.WriteLine($"WARNING! I cannot execute line {linenumber} as the [CALL] block is not supported in C#\n");
                                break;
                            default:
                                throw new Exception($"ERROR! Unknown tag: {tag} (line #{linenumber}:{line})\n");
                        } //End Select  
                    } //EndIf
                } //EndIf       
            } // Next
        } //End Function


        public void ReadFromBytes(byte[] b)
        {
            // This is a new approach for GINI.
            // The BlitzMax variant doesn't even support it.
			// This has not yet been tested as there is no writer for it yet.
			// I just need to find time to fully complete this experiment XD
            //g.init1st()

            var bt = new QuickStream(new MemoryStream(b)); //bytes.NewReader(b)
            var head = bt.ReadString(5);
            if (head != "GINI\x1a")
            {
                throw new Exception("The buffer read is not a GINI binary");
            }
            while (true)
            {
                var tag = bt.ReadByte();
                switch (tag)
                {
                    case 1: // Basic Variable
                        var k = bt.ReadString();
                        var v = bt.ReadString();
                        D(k, v);
                        break;
                    case 2: // Create list
                        var cklst = bt.ReadString();
                        CL(cklst, false);
                        break;
                    case 3: // Add to list
                        var kl = bt.ReadString();
                        var kv = bt.ReadString();
                        Add(kl, kv);
                        break;
                    case 4: // link list
                        var list2link = bt.ReadString();
                        var list2link2 = bt.ReadString();
                        list2link = list2link2;
                        break;
                    case 255:
                        bt.Close();
                        return;
                    default:
                        throw new Exception("ERROR! Unknown tag: {tag}");
                }
            } // infinite loop
        } // func


        /// <summary>Converts gini data into a string you can save as a GINI file</summary>
        public string ToSource()
        {
            var tme = DateTime.Now.ToString(); //:=time.Now()
            var ret = "[rem]\nGenerated file!\n\n";
            //ret+=fmt.Sprintf("Generated: %d/%d/%d",tme.Month(),tme.Day(),tme.Year())+"\n\n"
            ret += $"Generated: {tme}\n\n";
            ret += "[vars]\n";
            var tret = new List<string>();//[] string{}
            foreach (string k in vars.Keys) //for k,v:=range g.vars
            {
                var v = vars[k];
                //ret+=k+"="+v+"\n"
                tret.Add(k + "=" + v);
            }
            tret.Sort(); //sort.Strings(tret)
            foreach (string l in tret) { ret += l + "\n"; }
            ret += "\n\n";
            var ldone = new Dictionary<string, bool>();
            foreach (string list in lists.Keys)
            {
                var point = lists[list];
                var slists = "";
                //for k,p:=range g.listpointer
                if (!ldone.ContainsKey(list))
                {
                    foreach (string k in lists.Keys)
                    {
                        var p = lists[k];
                        if (p == point)
                        {
                            if (slists != "") { slists += ","; }
                            slists += k;

                        }
                    }
                    //if (slists == "")
                    //{
                    //fmt.Printf("ERROR! List pointer -- without references!", point)
                    //  }
                    ldone[list] = true;
                    //} else {
                    ret += "[List:" + slists + "]\n";
                    foreach (string v in lists[list]) { 
                        ret += v + "\n";
                        //TrickyUnits.GTK.QuickGTK.Info($"I save {v} from {list} // {lists[list].Count}"); // DEBUG ONLY!!!!
                    }
                    //for _, v:= range list { ret += v + "\n"}
                    ret += "\n";

                }
            }
            return ret;
        }

        /// <summary>Save as a source file (editable in text editors)</summary>
        /// <seealso cref="ToSource"/>
        public void SaveSource(string filename)
        {
            var src = ToSource();
            QOpen.SaveString(filename, src);
        }

    }


    /// <summary>
    /// GINI Is Not Ini
    /// This is the base "core" class of GINI.
    /// </summary>
    class GINI
    {
        const string allowedChars = "qwertyuiopasdfghjklzxcvbnm[]{}1234567890-_+$!@%^&*()_+QWERTYUIOPASDFGHJKL|ZXCVBNM<>?/ '.";




        // The functions below have also been translated from BlitzMax to Go and then been translated to C#... Maybe not the best way to go, but it should work :P

        // It tries to get unwanted characters out, but it's never been fully trustworthy.
        // Any ideas to get this fully working are welcome!
        static  string IniString(string A)
        {// XAllow been removed
            var i = 0;
            //Local ret$[] = ["",A]
            var ret = "";
            var allowed = true;
            for (i = 1; i <= A.Length; i++)
            {
                var found = false;
                for (int j = 1; j <= allowedChars.Length; j++) { found = found || qstr.Mid(allowedChars, j) == qstr.Mid(A, i); }
                allowed = allowed && found; //&& allowedChars.IndexAny(allowedChars, string(A[i])) > (-1); //(allowedchars+XAllow).find(Chr(A[i]))>=0
                                            //'If Not allowed Print "I will not allow: "+Chr(A[i])+"/"+A
                var ascii = qstr.Mid(A, i).ToCharArray();
                ret += $"#({(byte)ascii[0]})"; //fmt.Sprintf("#(%d)",A[i])
            } //Next
            if (allowed)
            {
                return A;
            }
            else
            {
                return ret;
            }
        }// Return ret[allowed]   //End Function

        // Undo the inistring
        static public string UnIniString(string A)
        {
            var ret = A;
            for (int i = 0; i < 256; i++)
            {
                //ret = strings.Replace(ret, fmt.Sprintf("#(%d)",i),string (i),-900)
                //ret = string.Replace(ret,"#u("+i+")",string(i))
                ret = ret.Replace($"#({i})", ((char)i).ToString());
            } //Next
            return ret;
        } //End Function

        static public TGINI ReadFromLines(string[] lines)
        {
            var ret = new TGINI();
            //ret.init1st()
            ret.ParseLines(lines);
            return ret;
        }

        static  TGINI ReadFromBytes(byte[] thebytes)
        {
            var ret = new TGINI();
            //ret.init1st()
            ret.ReadFromBytes(thebytes);
            return ret;
        }


        // This function can read a GINI file.
        // Either being a 'text' file or 'compiled' file doesn't matter
        // this routine can autodetect that.
        static public TGINI ReadFromFile(string file)
        {
            if (!File.Exists(file))
            {
                Console.WriteLine($"GINI file {file} doesn't exist");
                return null;
            }
            var ret = new TGINI();
            var b = QOpen.GetFile(file);
            return ParseBytes(b);
        }

        // This can be used when you have alternative ways to load a file (like JCR6 for example).
        // Whether these bytes form a text file or a "compiled" gini file doesn't matter, this routine will autoamtically detect it and call the correct parser.
        static TGINI ParseBytes(byte[] b)
        {
            TGINI ret;
            string head = "";
            if (b.Length > 5)
            {
                for (int i = 1; i < 5; i++) { head += qstr.Chr(b[i]); }
            }
            if (head == "GINI\x1a")
            {
                ret = ReadFromBytes(b);
            }
            else
            {
                var s = System.Text.Encoding.Default.GetString(b); //Console.WriteLine($"[Parsing]\n{s}\n[/Parsing]");
                var sl = s.Split('\n');
                ret = ReadFromLines(sl);
            }
            return ret;

        }

        static bool debug = false; // Should always be false. (Used to be a const, but that cause unneeded warnings to pop up!)

        static public void DBGChat(string a){
            if (debug) { Console.WriteLine(a); }
        }

        public GINI(){
            MKL.Version("Tricky Units for C# - GINI Is Not Ini.cs","18.11.13");
            MKL.Lic    ("Tricky Units for C# - GINI Is Not Ini.cs","ZLib License");
            var tb = debug;
        }
    }
}
