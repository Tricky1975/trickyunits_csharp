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
