// Lic:
// mkl.cs
// TrickyUnits - MKL
// version: 20.08.16
// Copyright (C) 2018, 2020 Jeroen P. Broks
// This software is provided 'as-is', without any express or implied
// warranty.  In no event will the authors be held liable for any damages
// arising from the use of this software.
// Permission is granted to anyone to use this software for any purpose,
// including commercial applications, and to alter it and redistribute it
// freely, subject to the following restrictions:
// 1. The origin of this software must not be misrepresented; you must not
// claim that you wrote the original software. If you use this software
// in a product, an acknowledgment in the product documentation would be
// appreciated but is not required.
// 2. Altered source versions must be plainly marked as such, and must not be
// misrepresented as being the original software.
// 3. This notice may not be removed or altered from any source distribution.
// EndLic




using System;
using System.Collections.Generic;



namespace TrickyUnits {
    /// <summary>MKL (MaKe License) was a system I set up for automatically adding license texts and version numbering. This way I can always be sure version changes never go without nummeric changes. It's a very simplistic system, but it does what it has to do. :)</summary>
    public class MKL {
        static public string MyExe {
            get {
#if !LIBRARY
                return System.Reflection.Assembly.GetEntryAssembly().Location;
#else
                return "This property cannot be called in a library environment!";
#endif
            }
        }


        SortedDictionary<string, string> TVERSIONS = new SortedDictionary<string, string>();
        SortedDictionary<string, string> TLICENSES = new SortedDictionary<string, string>();

        static MKL MySelf = new MKL();

        static SortedDictionary<string, string> VERSIONS => MySelf.TVERSIONS;
        static SortedDictionary<string, string> LICENSES => MySelf.TLICENSES;



        /// <summary>Defines version number</summary>
        static public void Version(string f, string v){            VERSIONS[f] = v;        }
        public void SetVersion(string f,string v) { TVERSIONS[f] = v; }



        /// <summary>Defines license field</summary>
        static public void Lic(string f, string l){            LICENSES[f] = l;        }
        public void SetLic(string f,string l) { TLICENSES[f] = l; }

        /// <summary>
        /// Sets the width of the filename area in an "All" overview!
        /// </summary>
        static public int AllWidth = 40;

        /// <summary>Shows all version information</summary>
        /// <remarks>A weakness in this system is that C# requires classes and that those classes are only loaded if actually needed, or rather the first time they are called, or created for a variable. Due to this version summaries can change if this function is called before all require classes are loaded.</remarks>
        public string GetAll(bool showlic=true){
            var ret = "";
            var dots = ""; for (var i = 0; i <= AllWidth; i++) { dots += "."; }
            foreach(string k in TVERSIONS.Keys){
                var b = (k + " " + dots).Substring(0, AllWidth)+" "+TVERSIONS[k];
                if (showlic && TLICENSES.ContainsKey(k)) { b += " " + TLICENSES[k]; } 
                ret += b + "\n";
            }
            return ret;
        }
        static public string All(bool showlic = true) => MySelf.GetAll(showlic);



        static string Right(string s, int l = 1) {
            if (l > s.Length) return s;
            return s.Substring(s.Length - l, l);
        }

        static string Left(string s, int l = 1)
        {
            if (l > s.Length) return s;
            return s.Substring(0, l);
        }




        /// <summary>
        /// Shows you the 'newest version' based on the used sources
        /// </summary>
        public string GetNewest{
            get{
                var ret = " 0.00.00";
                var year = 0;
                var month = 0;
                var day = 0;
                var hitotal = 0;
                foreach(string mvalue in TVERSIONS.Values){
                    var split = mvalue.Split('.');
                    int sy=0; int sm=0; int sd=0;
                    int total = 0;
                    try{
                        sy = Int32.Parse(split[0]);
                        sm = Int32.Parse(split[1]);
                        sd = Int32.Parse(split[2]);
                        total = Int32.Parse($"{sy}{sm+10}{sd+10}"); // This looks odd, but guarantees the best outcome. This number is never visible to users anyway. It is a bit dirty, though, I gotta admit that.                        
                    } catch {
                        Console.WriteLine("WARNING! Something DID get wrong with the version parsing!");
                    }
                    //if (sy>=year && sm>=month && sd>=day){
                    if (hitotal < total) { 
                        year = sy;
                        month = sm;
                        day = sd;
                        ret = $"{Right($"0{year}", 2)}.{Right($"0{month}", 2)}.{Right($"0{day}",2)}";
                        hitotal = total;
                    }
                }
                return ret;
            }
        }

        static public string Newest => MySelf.GetNewest;

        public string GetCYear(int iyear) {
            var nyear = $"20{Left(GetNewest,2)}";
            if ($"{iyear}" != nyear) return $"{iyear}-{nyear}";
            return $"{iyear}";
        }
        static public string CYear(int iyear) => MySelf.GetCYear(iyear);


        static MKL(){
            // Despite C# considering this as "obsolete" the "MKL." prefixes MUST be present here, or MKL_Update won't onderstand these values have to be updated.
            MKL.Version("Tricky Units for C# - mkl.cs","20.08.16");
            MKL.Lic    ("Tricky Units for C# - mkl.cs","ZLib License");
        }

    }
}