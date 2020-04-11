// Lic:
// Swap.cs
// Swap
// version: 20.04.11
// Copyright (C)  Jeroen P. Broks
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

#define Swap_Debug

using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace TrickyUnits {

    /// <summary>
    /// This is a kind of a map or dictionary with only string keys and string values, meant to store large amounts of data of a longer time, and thus saving things in swap files if needed. 
    /// Note: In order to prevent conflicts with file systems (especially case INSENSITIVE ones, sich as FAT32, ExFAT, NTFS, well nearly all Windows compatible file systems), the keys are case insensitive
    /// Note: / and \ are both invalid in key characters and will be replaced with periods. Also this class was NOT set up to be fast!!! It was set up not to waste more RAM than needed, but also not to write more to the disk than needed. Especially with SSD drives you don't wanna write more than you have to!
    /// </summary>
    class Swap {

        static void Chat(string s) {
#if Swap_Debug
            Debug.WriteLine($"SWAP DEBUG:> {s}");
#endif
        }

        private class SwapData {
            StringBuilder Data;
            public int Modifications { get; private set; } = 0;
            public DateTime Modified { get; private set; } = DateTime.Now;
            public SwapData(StringBuilder a,bool modified=false) { Data = a; if (modified) Modifications = 1; }
            public SwapData(string a, bool modified = false) { Data = new StringBuilder(a); if (modified) Modifications = 1; }
            public SwapData(byte[] a, bool modified = false) { Data = new StringBuilder(Encoding.Default.GetString(a)); if (modified) Modifications = 1; }
            public void Def(string a) { Data.Clear(); Data.Append(a); Modifications++; }
            public void App(string a) { Data.Append(a); Modified = DateTime.Now; Modifications++; }
            override public string ToString() => Data.ToString();
        }

        /// <summary>
        /// Contains the swap folder. Cool for reference, but once created, this field may not be modified anymore!
        /// </summary>
        readonly public string SwapFolder;
        readonly TMap<string, SwapData> Map = new TMap<string, SwapData>();

        /// <summary>
        /// Creates a Swap class and sets a folder to store swap data in! If the chosen folder does not exist, it will be attempted to create! Be very careful, as this folder may NEVER be used for anything else than for this class, or loss of data can (and maybe even will) happen!
        /// </summary>
        /// <param name="MySwapFolder"></param>
        public Swap(string MySwapFolder) {
            SwapFolder = MySwapFolder;
        }

        void MD() => Directory.CreateDirectory(SwapFolder);

        string SafeTag(string Tag) {
            var ret = Tag.ToUpper().Replace("\\", ".");
            ret = ret.Replace("/", ".");
            for (byte i = 255; i > 0; i--) {
                var c = (char)i;
                if ((c < 'a' && c > 'z') && c != ' ' && (c < '0' && c > '9') && (c < 'A' && c > 'Z') && c != '_' && c != '.') ret.Replace($"{c}", $"#{i}");
            }
            return ret;
        }

        string TagFile(string Tag) => $"{SwapFolder}/{Tag}";

        /// <summary>
        /// Clears the entire swap! Yes, this will remove all swap files in the folder set to this class!
        /// </summary>
        public void Clear() {
            Map.Clear();
            MD();
            var l = FileList.GetDir(SwapFolder,0,false,true);
            foreach (string f in l) File.Delete(f);
        }

        public void Kill(string Tag) {
            var t = SafeTag(Tag);
            var f = TagFile(t);
            Map.Kill(t);
            MD();
            if (File.Exists(f)) File.Delete(f);
            Update();
        }

        void Update() {
            var keys = Map.Keys;
            var count = keys.Count;
            var nu = DateTime.Now;
            var ekill = new List<string>();
            foreach (string key in keys) {
                var entry = Map[key];
                int maxtime = ((entry.Modifications * 2) - count)+1;
                int time = (int)nu.Subtract(entry.Modified).TotalMinutes;
                if (time > maxtime || count>5 ) {
                    Chat($"Entry {key} is over time ({time} minutes elapsed, and {maxtime} were allowed). This entry was since put in RAM modified {entry.Modifications} time(s)");
                    if (entry.Modifications>0) QuickStream.SaveString(TagFile(key), entry.ToString());
                    ekill.Add(key);
                }
            }
            foreach (string key in ekill) Map.Kill(key);
        }

        public string[] Keys {
            get {
                MD();
                var l = new List<string>();
                var fl = FileList.GetDir(SwapFolder, 0, false, true);
                foreach (string k in fl) l.Add(k);
                foreach (string k in Map.Keys) if (!l.Contains(k)) l.Add(k);
                l.Sort();
                return l.ToArray();
            }
        }
        /// <summary>
        /// Creates a Lua script in which all keys are listed
        /// Specifically created for BUBBLE, but other Lua based engines can use it too, I suppose :P
        /// </summary>
        public string KeysLua {
            get {
                var comma = false;
                var ret = new StringBuilder("return {");
                foreach(string k in Keys) {
                    if (comma)
                        ret.Append(", ");
                    comma = true;
                    ret.Append($"\n\t\"{k}\"");
                }
                ret.Append("}\n");
                return ret.ToString();
            }
        }



        public string this[string key] {
            get {
                var tag = SafeTag(key);
                var fil = TagFile(tag);
                var SW = Map[tag];
                MD();
                if (SW == null) {
                    if (!File.Exists(fil)) return "";
                    SW = new SwapData(QuickStream.LoadString(fil));
                    Map[tag] = SW;
                }
                Update();
                return SW.ToString();
            }
            set {
                var tag = SafeTag(key);
                var fil = TagFile(tag);
                var SW = Map[tag];
                MD();
                if (SW == null) {
                    Chat($"Creating new entry: {tag}");
                    Map[tag] = new SwapData(value,true);                    
                } else {
                    Chat($"Rewrite existing entry: {tag}");
                    SW.Def(value);
                }
                Update();
            }
        }

        public void App(string key,string v) {
            var tag = SafeTag(key);
            var fil = TagFile(tag);
            var SW = Map[tag];
            if (SW == null) {
                if (File.Exists(fil))
                    SW = new SwapData(QuickStream.LoadString(fil));
                else
                    SW = new SwapData("");
                Map[tag] = SW;
            }
            SW.App(v);
            Update();

        }

    }

}