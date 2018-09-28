// Lic:
//   Dirry.cs
//   Dirry
//   version: 18.09.28
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

using System.Collections.Generic;
using System;

namespace TrickyUnits
{
    class Dirry
    {
        static Dirry(){
            MKL.Version("Tricky Units for C# - Dirry.cs","18.09.28");
            MKL.Lic    ("Tricky Units for C# - Dirry.cs","ZLib License");
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            nodollar = false;
            Add("$Home$",home );
            Add("$Documents$", $"{home}/Documents");
            Add("$MyDocs$", $"{home}/Documents");
            Add("$AppSupport$", $"{home}/.Tricky__ApplicationSupport"); // <- This is a dirty method, but unfortunately Mono and C# have very poor (read: no) support for doing this properly!!!
            Add("$UserName$", Environment.UserName);
            Add("$AppDir$", AppDomain.CurrentDomain.BaseDirectory);
            Add("$LaunchDir$", System.IO.Directory.GetCurrentDirectory()); // <- Please note this only is valid if this class is called right when the application starts up.
            nodollar = true;
       }

        static bool nodollar = false;
        static readonly Dictionary<string, string> Troep = new Dictionary<string, string>();

        static public string LaunchedFrom { get => Troep["$LauchDir$"]; }


        static public void Add(string key, string value)
        {
            var skey = key;
            if (nodollar)
            {
                if (key[0] == '$') return;
                if (key[0] != '*') skey = $"*{key}";
                if (skey[key.Length - 1] != '*') skey += "*";
            }
            Troep[skey] = value;
        }

        /// <summary>
        /// Returns a string with dirry tags replaced by the proper data
        /// </summary>
        static public string C(string str)
        {
            nodollar = false;
            Add("$CurrentDir$", System.IO.Directory.GetCurrentDirectory());
            nodollar = true;
            var ret = str;
            foreach (string k in Troep.Keys)
            {
                Console.WriteLine($"{k} = {Troep[k]}");
                ret = ret.Replace(k, Troep[k]);
            }
            return ret;
        }

    }
}
