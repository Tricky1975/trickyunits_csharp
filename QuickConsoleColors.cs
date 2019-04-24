// Lic:
// QuickConsoleColors.cs
// Quick Console Colors
// version: 19.04.24
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

namespace TrickyUnits{
    static class QCol {
        static QCol(){
            MKL.Version("Tricky Units for C# - QuickConsoleColors.cs","19.04.24");
            MKL.Lic    ("Tricky Units for C# - QuickConsoleColors.cs","ZLib License");
        }

        /// <summary>Does nothing at all. But calling it forces C# to load this class making sure the version data is up-to-date!</summary>
        static public void Hello();

static              ConsoleColor obcl = Console.BackgroundColor;
      static  ConsoleColor ofcl = Console.ForegroundColor;

           public static void OriCol() { Console.ForegroundColor = ofcl;Console.BackgroundColor = obcl; }

        public static  void ColWrite(ConsoleColor c, string m) { Console.ForegroundColor = c; Console.Write(m); }

public static         void Red(string m) => ColWrite(ConsoleColor.Red, m);
public static         void Magenta(string m) => ColWrite(ConsoleColor.Magenta, m);
public static         void Yellow(string m) => ColWrite(ConsoleColor.Yellow, m);
public static         void Cyan(string m) => ColWrite(ConsoleColor.Cyan, m);
public static         void White(string m) => ColWrite(ConsoleColor.White, m);
public static         void Green(string m) => ColWrite(ConsoleColor.Green, m);


    }
}
