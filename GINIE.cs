// Lic:
// GINIE.cs
// GINIE Is Not INI Either
// version: 21.09.11
// Copyright (C) 2020, 2021 Jeroen P. Broks
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
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
namespace TrickyUnits {

    class GINIE_IllegalTag : Exception {
        public GINIE_IllegalTag() { }

        override public string Message => "Illegal tag in source. Was \"[\" closed by a proper \"]\"?";
        
    }

    class GINIE_SysRequestNotUnderstood : Exception {
        public GINIE_SysRequestNotUnderstood(string req) { this.req = req; }
        private string req;

        override public string Message => $"System request not understood => {req}";

    }

    class GINIE_IllegalEnd : Exception {
        public GINIE_IllegalEnd() { }

        override public string Message => "*END instruction unexpected";

    }

    class GINIE_NoTag : Exception {
        public GINIE_NoTag() { }
        public override string Message => "No tag";
    }

    class GINIE_IllegalDefinition:Exception {
        public GINIE_IllegalDefinition(string d) { this.d = d; }
        string d;
        public override string Message => $"Illegal Defintiion: {d}";
    }

    // GINIE is not INI, either!
    /// <summary>
    /// GINIE = GINIE is not INIE, either! A class almost compatible with INI, but with a few different twists.
    /// </summary>
    class GINIE {

        static public void Hello() {
            MKL.Lic    ("Tricky Units for C# - GINIE.cs","ZLib License");
            MKL.Version("Tricky Units for C# - GINIE.cs","21.09.11");
        }

        private GINIE() { Hello(); }


        /// <summary>
        ///  Parse GINIE source into data
        /// </summary>
        /// <param name="source">Source code</param>
        /// <returns></returns>
        static public GINIE FromSource(string[] source) {
            var ret = new GINIE();
            var tag = "";
            var list = "";
            foreach (string l in source) {
                var line = l.Trim();
                if (line != "") {
                    switch (line[0]) {
                        case '\\':
                            line = line.Substring(1);
                            goto default;
                        case '#':
                        case ';':
                            break;
                        case '[':
                            if (line[line.Length - 1] != ']') {
                                Debug.WriteLine($"Line=\"{line}\"; #line={line.Length}; 0='{line[0]}' {line.Length-1}='{line[line.Length-1]}'");
                                throw new GINIE_IllegalTag();
                            }
                            tag = line.Substring(1, line.Length - 2).Trim().ToUpper();
                            break;
                        case '*':
                            if (qstr.Prefixed(line.ToUpper(), "*LIST:")) {
                                if (tag == "") throw new GINIE_NoTag();
                                list = line.Substring(6);
                                break;
                            }
                            if (line.ToUpper()=="*END") {
                                if (list!="") { list = "";break; }
                                throw new GINIE_IllegalEnd();
                            }
                            throw new GINIE_SysRequestNotUnderstood(line);
                        default:
                            if (tag == "") throw new GINIE_NoTag();
                            if (tag == "*LIC*" || tag == "*REM*") break;
                            if (list != "") {
                                ret.List(tag, list).Add(line);
                                break;
                            }
                            var p = line.IndexOf('=');
                            if (p <= -1) throw new GINIE_IllegalDefinition(line);
                            var key = line.Substring(0, p);
                            var val = line.Substring(p + 1);
                            for(int c = 0; c < 255; c++) {
                                key = key.Replace($"\\{c.ToString("03d")}", $"{(char)c}");
                                val = val.Replace($"\\{c.ToString("03d")}", $"{(char)c}");
                            }
                            ret[tag, key] = val;
                            break;
                    }
                }
            }
            return ret;
        }
        
        static public GINIE FromSource(string source) => FromSource(source.Split('\n'));
        static public GINIE FromSource(List<string> source) => FromSource(source.ToArray());

        static public GINIE FromFile(string file,bool allownonexistent = true) {
            if (!File.Exists(file)) {
                if (allownonexistent)
                    return new GINIE();
                else
                    return null;                    
            }
            // TODO: Auto-detect binary form if existent
            return FromSource(QuickStream.LoadString(file));
        }

        SortedDictionary<string, SortedDictionary<string, string>> Values = new SortedDictionary<string, SortedDictionary<string, string>>();
        SortedDictionary<string, SortedDictionary<string, List<string>>> Lists  = new SortedDictionary<string, SortedDictionary<string, List<string>>>();
        public string AutoSaveSource = "";

        public List<string> List(string sec, string key) {
            sec = sec.ToUpper();
            key = key.ToUpper();
            if (!Lists.ContainsKey(sec)) Lists[sec] = new SortedDictionary<string, List<string>>();
            if (!Lists[sec].ContainsKey(key)) Lists[sec][key] = new List<string>();
            return Lists[sec][key];
        }

        public bool HasList(string sec,string key) {
            sec = sec.ToUpper();
            key = key.ToUpper();
            if (!Lists.ContainsKey(sec)) return false;
            return Lists[sec].ContainsKey(key);
        }

        public void ListAdd(string sec,string key,string value,bool sort = true) {
            List(sec, key).Add( value);
            if (sort) List(sec, value).Sort();
            if (AutoSaveSource != "") SaveSource(AutoSaveSource);
        }

        public void ListAddNew(string sec,string key,string value,bool sort=true) {
            if (!List(sec, key).Contains(value)) ListAdd(sec, key,value,sort);
        }

        public SortedDictionary<string, SortedDictionary<string, string>>.KeyCollection EachSections => Values.Keys;

        public SortedDictionary<string, string>.KeyCollection Each(string sec) {
            sec = sec.ToUpper();
            if (!Values.ContainsKey(sec)) return null;            
            return Values[sec].Keys;
        }

        

        public string this[string sec, string key] {
            get {
                sec = sec.ToUpper();
                key = key.ToUpper();
                if (!Values.ContainsKey(sec)) return "";
                if (!Values[sec].ContainsKey(key)) return "";
                return Values[sec][key];
            }
            set {
                sec = sec.ToUpper();
                key = key.ToUpper();
                if (!Values.ContainsKey(sec)) Values[sec] = new SortedDictionary<string, string>();
                Values[sec][key] = value;
                if (AutoSaveSource != "") {
                    SaveSource(AutoSaveSource);
                }
            }
        }
        /*
        System.Collections.Generic.List<string> List(string sec, string key) {
            sec = sec.ToUpper();
            key = key.ToUpper();
            if (!Lists.ContainsKey(sec)) Lists[sec] = new SortedDictionary<string, System.Collections.Generic.List<string>>();
            if (!Lists[sec].ContainsKey(key)) Lists[sec][ key] = new System.Collections.Generic.List<string>();
            return Lists[sec][key];
        }
        */

        public string ToSource() {
            var Done = new List<string>();
            var ret = new StringBuilder();
            foreach (string k in Values.Keys) {
                ret.Append($"[{k}]\n");
                Done.Add(k);
                foreach (string key in Values[k].Keys) ret.Append($"{qstr.SafeString(key)}={qstr.SafeString(Values[k][key])}\n");
                if (Lists.ContainsKey(k)) {
                    foreach (string key in Lists[k].Keys) {
                        ret.Append($"*list:{qstr.SafeString(key)}\n");
                        foreach (string item in Lists[k][key]) ret.Append($"\t{qstr.SafeString(item)}\n");
                        ret.Append($"*end\n");
                    }
                }
            }
            foreach (string k in Lists.Keys) {
                if (!Done.Contains(k)) {
                    ret.Append($"[{k}]\n");
                    foreach (string key in Lists[k].Keys) {
                        ret.Append($"*list:{qstr.SafeString(key)}\n");
                        foreach (string item in Lists[k][key]) ret.Append($"\t{qstr.SafeString(item)}\n");
                        ret.Append($"*end\n");
                    }
                }
            }
            return ret.ToString();
        }


        public void SaveSource(string file) => QuickStream.SaveString(file, ToSource());
        
    }
}