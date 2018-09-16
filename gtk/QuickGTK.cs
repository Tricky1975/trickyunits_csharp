// Lic:
//   QuickGTK.cs
//   
//   version: 18.09.16
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
using Gtk;
namespace TrickyUnits.GTK{

    /// <summary>
    /// Quick GTK Color class used to store the data SelectColor returns.
    /// </summary>
    class QuickGTKColor {
        public QuickGTKColor(ColorSelection acs) { cs = acs; }
        ColorSelection cs;

        public int red { get => cs.CurrentColor.Red; }
        public int green { get => cs.CurrentColor.Green; }
        public int blue { get => cs.CurrentColor.Blue; }
        public int alpha { get => cs.CurrentAlpha; }


    }

    /// <summary>
    /// This class contains functions that you may just want to use quickly
    /// without any crap of creating and disposing stuff.
    /// </summary>
    class QuickGTK
    {

        /// <summary>
        /// When you do not assign any windows to the functions put in here, QuickGTK will simply us this one.
        /// </summary>
        static public Window MyMainWindow;

        /// <summary>
        /// Shows a dialog box to select a color
        /// </summary>
        /// <returns>The color.</returns>
        static public QuickGTKColor SelectColor()
        {
            ColorSelection ret = null;
            ColorSelectionDialog cdia = new ColorSelectionDialog("Select color");
            cdia.Response += delegate (object o, ResponseArgs resp)
            {

                if (resp.ResponseId == ResponseType.Ok)
                {
                    ret = cdia.ColorSelection; //.CurrentColor;
                }
            };
            cdia.Run();
            cdia.Destroy();
            return new QuickGTKColor(ret);
        }


        /// <summary>
        /// Yes/No question
        /// </summary>
        static public bool Confirm(string Question,MessageType mt= MessageType.Question)
        {
            bool ret;
            MessageDialog md1 = new MessageDialog(null, DialogFlags.Modal,
                                                  mt, ButtonsType.YesNo, Question);
            md1.DefaultResponse = ResponseType.Yes;
            ResponseType response = (ResponseType)md1.Run();
            //md1.Show();
            ret = (response == ResponseType.Yes);
            md1.Destroy();
            //md1.Dispose();
            return ret;
        }

        /// <summary>
        /// Asks a question with Yes, No and Cancel for answers
        /// </summary>
        /// <returns>-1 if user chose cancel, 0 if no and 1 if yes</returns>
        /// <param name="Question">Question.</param>
        static public short Proceed(string Question, MessageType mt = MessageType.Info)
        {
            // This approach was needed, since GTK does not support Yes/No/Cancel questions, or so it seems.
            if (Confirm(Question+"\n\nCanceling is possible after saying 'no'", mt)) return 1;
            if (Confirm(Question + "\n\nDo you want to cancel the operation entirely?", mt)) return -1;
            // Hopefully a more "elegant" solution may be possible later... :-/
            return 0;
        }


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
