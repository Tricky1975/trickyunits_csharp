// Lic:
// gtk/inputdialog.cs
// QuickGTK
// version: 19.03.09
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


using TrickyUnits;



using System.Text.RegularExpressions;

using Gtk;

using System;



namespace TrickyUnits.GTK{



    /// <summary>

    /// This delegate type can be used for callbacks from Quick Input Box. This function will both be called wether the user clicks "Ok" or "Cancel", however the "ok" boolean will receive 'true' and 'returnstring' will then contain the string, if the user clicks cancel 'returnstring' will be an empty string and 'ok' will be false.

    /// </summary>

    delegate void QuickInputBoxCallBack(string returnstring,bool ok);



    /// <summary>

    /// Quick input box.

    /// </summary>

    class QuickInputBox{



        public static string con_cancel = "Cancel";

        public static string con_ok = "Ok";



        QuickInputBoxCallBack callback=null;

        string allowregex = "";

        Widget hidemainwindow = null;

        Window mywindow = null;

        Entry myentry = null;

        string Value { get => myentry.Text; set { myentry.Text = value; }}





        void DestroyThis(){

            mywindow.Destroy();

            if (hidemainwindow!=null) hidemainwindow.ShowAll();

        }



        void OnCancel(object sender ,EventArgs blahblah) {

            callback("", false);

            DestroyThis();

        }



        void OnOk(object sender,EventArgs blahblah){

            var v = Value;

            if (allowregex != "") {

                var r = new Regex("^"+allowregex+"$");

                if (!r.IsMatch(v)) {

                    QuickGTK.Error("Invalid input!");

                    return;

                }

            }

            callback(v, true);

            DestroyThis();

        }



        static public void Hello(){

            MKL.Version("Tricky Units for C# - inputdialog.cs","19.03.09");

            MKL.Lic    ("Tricky Units for C# - inputdialog.cs","ZLib License");

        }



        /// <summary>

        /// Creates a window in which a question will be asked, and will destroy the window after the user either confirms or cancels the input.

        /// </summary>

        /// <param name="Question">Question to appear on the title bar</param>

        /// <param name="CallBackFunction">This function is called when the operation is confirmed or cancelled prior to the window's destruction.</param>

        /// <param name="defaultvalue">Default value.</param>

        /// <param name="HideMainWindow">Hide main window.</param>

        /// <param name="AllowRegEx">When set the input MUST match the set up regular expression or the input will be rejected (System will automatically prefix the regex with ^ and end it with a $)</param>

        static public void Create(string Question, QuickInputBoxCallBack CallBackFunction, string defaultvalue = "", Widget HideMainWindow = null, string AllowRegEx = ""){

            var needlessvar = new QuickInputBox(Question, CallBackFunction, defaultvalue, HideMainWindow, AllowRegEx);

        }

        public QuickInputBox(string Question,QuickInputBoxCallBack CallBackFunction,string defaultvalue="",Widget HideMainWindow=null,string AllowRegEx=""){

            Hello();

            callback = CallBackFunction;

            allowregex = AllowRegEx;

            hidemainwindow = HideMainWindow;

            if (hidemainwindow != null) hidemainwindow.Hide();

            mywindow = new Window(Question);

            mywindow.SetSizeRequest(1200, 60);

            mywindow.ModifyBg(StateType.Normal, new Gdk.Color(0, 0, 0));

            var mb = new VBox();

            myentry = new Entry(defaultvalue); mb.Add(myentry);

            var buttons = new HBox(); mb.Add(buttons);

            var buttons1 = new HBox(); buttons.Add(buttons1);

            var buttons2 = new HBox(); buttons.Add(buttons2);

            var cancel = new Button(con_cancel);

            var ok = new Button(con_ok);

            cancel.Clicked += OnCancel;

            ok.Clicked += OnOk;

            myentry.Activated += OnOk;

            myentry.DeleteEvent += OnCancel;

            buttons2.Add(cancel);

            buttons2.Add(ok);

            mywindow.Add(mb);

            mywindow.ShowAll();

            }

    }

}

