using System;
using System.Diagnostics;

namespace TrickyUnits {
    class TrickyDebug {

        /// <summary>
        /// Outputs to the console, but only in debug builds!
        /// </summary>
        /// <param name="s"></param>
        public static void Chat(params string[] s) {
#if DEBUG
            for (int i = 0; i < s.Length; ++i) {
                if (i > 0) Console.Write("\t");
                Console.Write(s[i]);
            }
            Console.WriteLine("");
#endif
        }

        /// <summary>
        /// When running in Visual Studio (or MonoDevelop), it will ask for a keypress. When running from prompt or IE, nothing will happen.
        /// This function was made to easily have VS wait with closing windows prior to closing down.
        /// Please note that when you use VS on Mac or Linux, this can cause two keypresses to be asked, based on how your terminal has been configured/developed.
        /// </summary>
        public static void AttachWait() {
            if (Debugger.IsAttached) {
                Console.Write("Please hit any key to continue. . . "); Console.ReadKey();
                Console.Write("                                    \r");
            }
        }
    }
}