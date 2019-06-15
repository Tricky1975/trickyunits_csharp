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
    /// <summary>
    /// File request dialogs. Does require System.Windows.Forms in order to be used!
    /// </summary>
    class FFS { // Not that internet slang abbreviation :-P

        static FFS() {
            MKL.Lic    ("Tricky Units for C# - FormSelectFiles.cs","ZLib License");
            MKL.Version("Tricky Units for C# - FormSelectFiles.cs","19.06.15");
        }

        ///<summary>Only used to make sure all version info is loaded!</summary> 
        public static void Hello() { }


        /// <summary>
        /// Request a directory through a dialog box.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Request a file.
        /// </summary>
        /// <param name="save">If set, the window will be set most of all for saving!</param>
        /// <returns></returns>
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


