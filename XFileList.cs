// Lic:
// XFileList.cs
// X FileList
// version: 19.10.27
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
using Microsoft.VisualBasic.CompilerServices;
using System.Collections.Generic;


// This has been put appart as it may not always be needed to include MicroSoft.VisualBasic.CompilerServices
// And this this can save me a lot of trouble having to import this references when I don't really need it
// for the rest of the program.
namespace TrickyUnits {

    public partial class FileList {

        static public bool WC_Match(string WildCardString, string ChkString) {
            return LikeOperator.LikeString(ChkString, WildCardString, Microsoft.VisualBasic.CompareMethod.Binary);
        }

        /// <summary>
        /// Gets directory and checks by WildCards
        /// </summary>
        /// <param name="path"></param>
        /// <param name="WildCard"></param>
        /// <param name="gt"></param>
        /// <param name="sorted"></param>
        /// <param name="hidden"></param>
        /// <returns></returns>
        static public string[] GetDir(string path, string WildCard, int gt = 0, bool sorted = true, bool hidden = false) {
            var r = new List<string>();
            var g = GetDir(path, gt, sorted, hidden);
            foreach (string file in g) {
                if (WC_Match(WildCard, file)) r.Add(file);
            }
            if (sorted) r.Sort();
            return r.ToArray();
        }

        static public string[] GetTree(string path, string WildCard, bool sorted = true, bool hidden = false) => GetDir(path,WildCard, 3, sorted, hidden);

    }
}

