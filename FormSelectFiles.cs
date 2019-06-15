// Lic:
// FormSelectFiles.cs
// FFS
// version: 19.06.15
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

using System.Diagnostics;
namespace TrickyUnits {
    class FFS { // Not that internet slang abbreviation :-P

        public static string RequestDir() {
            string ret = "";
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog()) {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                switch (result) {
                    case System.Windows.Forms.DialogResult.OK:
                        ret = dialog.SelectedPath;
                        break;
                    case System.Windows.Forms.DialogResult.Cancel:
                    case System.Windows.Forms.DialogResult.Abort:
                        ret = "";
                        break;
                    default:
                        Debug.WriteLine($"WARNING! Unknown result type returned from dir selection: {result}");
                        break;
                }
            }
            return ret;
        }


        static string RequestOpenFile() {
            string ret = "";
            using (var dialog = new System.Windows.Forms.OpenFileDialog()) {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                switch (result) {
                    case System.Windows.Forms.DialogResult.OK:
                        ret = dialog.FileName;
                        break;
                    case System.Windows.Forms.DialogResult.Cancel:
                    case System.Windows.Forms.DialogResult.Abort:
                        ret = "";
                        break;
                    default:
                        Debug.WriteLine($"WARNING! Unknown result type returned from dir selection: {result}");
                        break;
                }
            }
            return ret;
        }


        static string RequestSaveFile() {
            string ret = "";
            using (var dialog = new System.Windows.Forms.SaveFileDialog()) {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                switch (result) {
                    case System.Windows.Forms.DialogResult.OK:
                        ret = dialog.FileName;
                        break;
                    case System.Windows.Forms.DialogResult.Cancel:
                    case System.Windows.Forms.DialogResult.Abort:
                        ret = "";
                        break;
                    default:
                        Debug.WriteLine($"WARNING! Unknown result type returned from dir selection: {result}");
                        break;
                }
            }
            return ret;
        }

        public static string RequestFile(bool save = false) {
            switch (save) {
                case false:
                    return RequestOpenFile();
                case true:
                    return RequestSaveFile();
                default:
                    throw new System.Exception("System abuse detected"); // This line is only there to satify the compiler which is not smart enough to see that this can NEVER happen!
            }
        }

    }
}

