using System;
using System.Collections.Generic;

namespace TrickyUnits
{
    class MKL
    {
        static SortedDictionary<string, string> VERSIONS = new SortedDictionary<string, string>();
        static SortedDictionary<string, string> LICENSES = new SortedDictionary<string, string>();

        // Defines version number
        static public void Version(string f, string v){
            VERSIONS[f] = v;
        }

        // Defines license field
        static public void Lic(string f, string l){
            LICENSES[f] = l;
        }

        // Shows all version information
        static public string All(bool showlic=true){
            var ret = "";
            var dots = ""; for (var i = 0; i <= 40; i++) { dots += "."; }
            foreach(string k in VERSIONS.Keys){
                var b = (k + " " + dots).Substring(0, 40)+" "+VERSIONS[k];
                if (showlic) { b += " " + LICENSES[k]; } // TODO: Fix crash if license is not present!
                ret += b + "\n";
            }
            return ret;
        }

        static MKL(){
            MKL.Version("test1", "Test2");
            MKL.Lic("test1", "Test3");
        }

    }
}