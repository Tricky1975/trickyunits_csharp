using System;
using System.Windows.Forms;

namespace TrickyUnits {

    

    static class Confirm {


        static Confirm() {
            MKL.Lic("", "");
            MKL.Version("", "");
        }

        static void Hello() { }

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

    }

}