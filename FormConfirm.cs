// Lic:
// FormConfirm.cs
// Form Confirm
// version: 23.09.18
// Copyright (C) 2019, 2023 Jeroen P. Broks
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
using System.Windows.Forms;

namespace TrickyUnits {

    

    static class Confirm {


        static Confirm() {
            MKL.Lic    ("Tricky Units for C# - FormConfirm.cs","ZLib License");
            MKL.Version("Tricky Units for C# - FormConfirm.cs","23.09.18");
        }

        public static void Hello() { }

        static public bool Yes(string Question) {
            DialogResult dialogResult = MessageBox.Show(Question, "You're sure?", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes) {
                return true;
            } else if (dialogResult == DialogResult.No) {
                return false;
            }
            throw new Exception("Invalid answer from Confirm box!");
        }

        public static int YNC(string Question) {
            DialogResult dialogResult = MessageBox.Show(Question, "You're sure?", MessageBoxButtons.YesNoCancel);
            if (dialogResult == DialogResult.Yes) {
                return 1;
            } else if (dialogResult == DialogResult.No) {
                return 0;
            } else if (dialogResult == DialogResult.Cancel) {
                return -1;
            }
            throw new Exception("Invalid answer from Confirm box!");
        }

        static public DialogResult Failure(string Question) {
            DialogResult dialogResult = MessageBox.Show(Question, "Something's not right", MessageBoxButtons.AbortRetryIgnore);
            return dialogResult;
        }


        static public void Annoy(string msg, string caption = "", MessageBoxIcon Icon = MessageBoxIcon.Information) => MessageBox.Show(msg, caption, MessageBoxButtons.OK, Icon);

        static public void Error(string msg, string caption = "Error!") => Annoy(msg, caption, MessageBoxIcon.Error);
    }

}