// Lic:
//   QuickGTK.cs
//   
//   version: 18.09.02
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
