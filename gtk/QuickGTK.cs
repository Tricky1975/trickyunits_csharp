using Gtk;
namespace TrickyUnits.GTK{

    /// <summary>
    /// This class contains functions that you may just want to use quickly
    /// without any crap of creating and disposing stuff.
    /// </summary>
    class QuickGTK{

        /// <summary>
        /// When you do not assign any windows to the functions put in here, QuickGTK will simply us this one.
        /// </summary>
        static public Window MyMainWindow;

        /// <summary>
        /// Used for "Yes" on questions. Change if your application is not in English
        /// </summary>
        static public string Yes = "Yes";

        /// <summary>
        /// Used for "No" on questions. Change if your application is not in English
        /// </summary>
        static public string No = "No";


        static void MessageDialogBox(string message,MessageType MT,Window pwin=null){

            MessageDialog md = new MessageDialog(pwin ?? MyMainWindow,
                 DialogFlags.DestroyWithParent, MT,
                 ButtonsType.Close, message);
            md.Run();
            md.Destroy();
        }

        /// <summary>
        /// Throw an error message box
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="pwin">Parent window</param>
        static public void Error(string message, Window pwin = null) => MessageDialogBox(message, MessageType.Error, pwin);

        /// <summary>
        /// Throw a warning message box
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="pwin">Parent window</param>
        static public void Warn(string message, Window pwin = null) => MessageDialogBox(message, MessageType.Warning, pwin);

        /// <summary>
        /// Throw an informative message box
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="pwin">Parent window</param>
        static public void Info(string message, Window pwin = null) => MessageDialogBox(message, MessageType.Info, pwin);


    }

}
