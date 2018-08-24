// Lic:
//   mkl.cs
//   MKL
//   version: 18.08.24
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
            MKL.Version("Tricky Units for C# - mkl.cs","18.08.24");
            MKL.Lic    ("Tricky Units for C# - mkl.cs","ZLib License");
        }

    }
}
