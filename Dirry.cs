// Lic:
// Dirry.cs
// TrickyUnits - Dirry
// version: 20.08.16
// Copyright (C) 2018, 2020 Jeroen P. Broks
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





#undef DirryDebug



using System.Collections.Generic;
using System.IO;
using System;
using System.Diagnostics;

using System.Runtime.InteropServices;




namespace TrickyUnits
{
    public enum AltDrivePlaforms
    {
        None, Windows, Mac, Linux
    }

    public class Dirry
    {
        static Dirry()
        {
            MKL.Version("Tricky Units for C# - Dirry.cs","20.08.16");
            MKL.Lic    ("Tricky Units for C# - Dirry.cs","ZLib License");
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            nodollar = false;
            Add("$Home$", home);
            Add("$Documents$", $"{home}/Documents");
            Add("$MyDocs$", $"{home}/Documents");
            Add("$AppSupport$", $"{home}/.Tricky__ApplicationSupport"); // <- This is a dirty method, but unfortunately Mono and C# have very poor (read: no) support for doing this properly!!!
            Add("$UserName$", Environment.UserName);
            Add("$AppDir$", AppDomain.CurrentDomain.BaseDirectory);
            Add("$LaunchDir$", System.IO.Directory.GetCurrentDirectory()); // <- Please note this only is valid if this class is called right when the application starts up.
            nodollar = true;
        }



        static bool nodollar = false;
        static readonly Dictionary<string, string> Troep = new Dictionary<string, string>();
        static public string LaunchedFrom { get => Troep["$LauchDir$"]; }



        static public void Add(string key, string value)
        {
            var skey = key;
            if (nodollar)
            {
                if (key[0] == '$') return;
                if (key[0] != '*') skey = $"*{key}";
                if (skey[key.Length - 1] != '*') skey += "*";
            }
            Troep[skey] = value;
        }



        /// <summary>
        /// Returns a string with dirry tags replaced by the proper data
        /// </summary>
        static public string C(string str)
        {
            nodollar = false;
            Add("$CurrentDir$", System.IO.Directory.GetCurrentDirectory());
            nodollar = true;
            var ret = str;
            foreach (string k in Troep.Keys)
            {
#if DirryDebug
                Console.WriteLine($"{k} = {Troep[k]}");
#endif
                ret = ret.Replace(k, Troep[k]);
            }
            return ret;
        }

        static Dictionary<string, string> AltDrives = new Dictionary<string, string>();

        /// <summary>
        /// Tries to set up ALT-Drives based on how drives are mounted on the system. Please note that it's important for the system to know what OS you are running, since not all systems have the same getup for this! (NOTE! Based on how .NET improvements reach my ear, this function may change in usage)
        /// </summary>
        /// <param name="p">Platform</param>
        /// <param name="linuxautomountdir">Can't assume that ~/media is where to look. :(</param>
        static public void InitAltDrives(AltDrivePlaforms p, string linuxautomountdir = "~/media") {
            AltDrives.Clear();
            string ud = "";
            switch (p) {
                case AltDrivePlaforms.None:
                    return;
                case AltDrivePlaforms.Mac:
                    ud = "/Volumes";
                    break;
                case AltDrivePlaforms.Linux:
                    ud = linuxautomountdir;
                    break;
                case AltDrivePlaforms.Windows:
                    WindowsDriveCheck();
                    return;
                default:
                    throw new Exception("I never heard of that platform!");
            }
            var di = new DirectoryInfo(ud);
            foreach (DirectoryInfo fi in di.GetDirectories()) {
                if (fi.Name[0] != '.') AltDrives[$"{fi.Name.ToUpper()}"] = $"{ud}/{fi.Name}/";
            }
        }

        static public string[] AltDriveList {
            get {
                var a = new List<string>();
                foreach (string k in AltDrives.Keys) {
                    var k2 =k ;
                    // while (k2[k2.Length - 1] == '\\') k2 = k2.Substring(0, k2.Length - 1);
                    if (k2[k2.Length - 1] != '\\')
                        a.Add(k2);
                }
                a.Sort();
                return a.ToArray();
            }
        }

        static public void InitAltDrives(string linuxmountdir = "~/media") {
            switch (Environment.OSVersion.Platform) {
                case PlatformID.MacOSX:
                    InitAltDrives(AltDrivePlaforms.Mac);
                    return;
                case PlatformID.Unix:
                    InitAltDrives(AltDrivePlaforms.Linux, linuxmountdir);
                    return;
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    InitAltDrives(AltDrivePlaforms.Windows);
                    return;
                default:
                    throw new Exception("Platform for AltDrives() not recognized!");
            }
        }


        static private void WindowsDriveCheck() {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo d in allDrives) {
                if (d.IsReady == true) {
                    if (d.VolumeLabel != "") {
                        AltDrives[$"{d.VolumeLabel.ToUpper()}"] = $"{d.Name.Replace("\\","/")}";
                        AltDrives[d.Name] = $"{d.Name.Replace("\\", "/")}";
                    }
                }
            }
        }

        static public string ADrives() {
            var ret = "";
            foreach (string k in AltDrives.Keys) ret += $"{k}={AltDrives[k]}; ";
            return ret;
        }

        static void dbg(string m) {
#if DirryDebug
            Debug.WriteLine($"DEBUG> {m}");
#endif
        }
        
        static public string AD(string s) {
            dbg($"AD(\"{s}\"); Index(':')=>{s.IndexOf(':')}; ");
            if (s.IndexOf(':') < 1) return s;
            var sp = s.Split(':');
            if (sp.Length != 2) return s;
            var k = sp[0].ToUpper();
            if (!AltDrives.ContainsKey(k)) return s;
            return $"{AltDrives[k]}{sp[1].Replace("\\","/")}";
        }

    }

}