// Lic:
// FileList.cs
// TrickyUnits - FileList
// version: 21.09.11
// Copyright (C) 2018, 2021 Jeroen P. Broks
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



#define SymLink_Debug_LoudMouth

using System;
using System.Collections.Generic;
using System.IO;

namespace TrickyUnits {
    public partial class FileList {
        // This variable contains the error message if something went wrong
        static string FLError = "";

        /// <summary>
        /// True if file is a SymLink (there is some controverse over this function, as it's said it could return false positives, however by lack of a better function, this will have to do).
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static public bool IsSymbolic(string path) {
            FileInfo pathInfo = new FileInfo(path);
            var ret = pathInfo.Attributes.HasFlag(FileAttributes.ReparsePoint);
#if SymLink_Debug_LoudMouth
            if (ret) System.Diagnostics.Debug.WriteLine($"{path} has been identified as a SymLink");
#endif
            return ret;
        }

        /// <summary>
        /// Just gets a string list of all files in a path
        /// path   = the path to search (duh!)
        /// gt     = search type 0 is files only, with setting 1 directories can be listed as well, 2 will list directories only, and 3 will generate a tree with all directories.
        /// hidden = Allow hidden files in the search results. Please note, hidden means by UNIX STANDARDS, so it will only check if a file is prefixed with a "." or not!
        /// </summary>
        static public string[] GetDir(string path, int gt = 0, bool sorted = true, bool hidden = false, bool allowsymlinks=true) {
            // init
            FLError = "";
            var w = new List<string>();
            var di = new DirectoryInfo(Dirry.AD(path));
            // Check
            if (!di.Exists) {
                FLError = "TrickyFileList.FileList.GetDir(\"" + path + "\"," + gt + "," + "," + sorted + "," + hidden + "): Directory does not exist!";
                return null;
            }
            // Listout
            foreach (FileInfo fi in di.GetFiles()) {
                if ((gt == 0 || gt == 1 || gt == 3) && (hidden || fi.Name.Substring(0, 1) != ".") && (allowsymlinks||(!IsSymbolic($"{path}/{fi.Name }")))) 
                    w.Add(fi.Name);
            }
            foreach (DirectoryInfo fi in di.GetDirectories()) {
                if (hidden || fi.Name.Substring(0, 1) != ".") {
                    switch (gt) {
                        case 1:
                        case 2:
                            w.Add(fi.Name);
                            break;
                        case 3:
                            var gd = GetDir(path + "/" + fi.Name, 3, false, hidden);
                            if (gd == null) return null; // Error catching. FLError has already been defined so no need to do that again!
                            foreach (string nf in gd) {
                                w.Add(fi.Name + "/" + nf);
                            }
                            break;
                    }
                }
            }
            // Sort if asked
            if (sorted) w.Sort();
            // return the crap
            return w.ToArray();
        }



        /// <summary>
        /// Gets the directory tree.
        /// </summary>
        /// <returns>The tree.</returns>
        /// <param name="path">Path to be turned into a tree</param>
        /// <param name="sorted">If set to <c>true</c> all entries will be sorted.</param>
        /// <param name="hidden">If set to <c>true</c> hidden files/directories will also be taken into account.</param>
        static public string[] GetTree(string path, bool sorted = true, bool hidden = false) => GetDir(path, 3, sorted, hidden);


        /// <summary>
        /// Does nothing, but calling this forces C# to initiate this class updating the MKL version stuff in the process!
        /// </summary>
        static public void Hello() { }


        static FileList() {
            MKL.Version("Tricky Units for C# - FileList.cs","21.09.11");
            MKL.Lic    ("Tricky Units for C# - FileList.cs","ZLib License");
        }

    }

}