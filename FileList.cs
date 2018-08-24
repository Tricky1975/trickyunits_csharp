using System;
using System.Collections.Generic;
using System.IO;
namespace TrickyUnits
{
    public class FileList
    {

        // This variable contains the error message if something went wrong
        static string FLError = "";

        // Just gets a string list of all files in a path
        // path   = the path to search (duh!)
        // gt     = search type 0 is files only, with setting 1 directories can be listed as well, 2 will list directories only, and 3 will generate a tree with all directories.
        // hidden = Allow hidden files in the search results. Please note, hidden means by UNIX STANDARDS, so it will only check if a file is prefixed with a "." or not!
        static public string[] GetDir(string path, int gt=0, bool sorted=true, bool hidden=false){
            // init
            FLError = "";
            var w = new List<string>();
            var di = new DirectoryInfo(path);
            // Check
            if (!di.Exists) {
                FLError="TrickyFileList.FileList.GetDir(\""+path+"\","+gt+","+","+sorted+","+hidden+"): Directory does not exist!";
                return null;
            }
            // Listout
            foreach (FileInfo fi in di.GetFiles()){
                if ((gt == 0 || gt == 1 || gt == 3) && (hidden || fi.Name.Substring(0,1)!=".")) w.Add(fi.Name);
            }
            foreach (DirectoryInfo fi in di.GetDirectories()){
                if (hidden || fi.Name.Substring(0, 1) != "."){
                    switch (gt)
                    {
                        case 1:
                        case 2:
                            w.Add(fi.Name);
                            break;
                        case 3:
                            var gd = GetDir(path + "/" + fi.Name, 3, false, hidden);
                            if (gd == null) return null; // Error catching. FLError has already been defined so no need to do that again!
                            foreach (string nf in gd){
                                w.Add(fi.Name + "/" + nf);
                            }
                            break;
                    }
                }
            }

            // Sort if asted
            if (sorted) w.Sort();


            // return the crap
            return w.ToArray();
        }

        static public string[] GetTree(string path, bool sorted=true, bool hidden=false){
            return GetDir(path, 3, sorted, hidden);
        }
    }
}
