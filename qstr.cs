// Lic:
// qstr.cs
// TrickyUnits - Quick String
// version: 19.10.27
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









using System;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;







namespace TrickyUnits {
    /// <summary>
    /// Just some quick functions to use with strings.
    /// Nothing special :P
    /// </summary>
    class qstr {
        static qstr() {
            MKL.Version("Tricky Units for C# - qstr.cs","19.10.27");
            MKL.Lic    ("Tricky Units for C# - qstr.cs","ZLib License");
        }

        /// <summary>
        /// Does nothing, but calling this just forces C# to load this class.
        /// </summary>
        public static void Hello() { }

        public static string EOLNType(string a) {
            string ret = "LF";            
            var i = a.IndexOf('\r'); if (i < 0) return "LF";
            if (i == a.Length - 1 || Mid(a,i + 2, 1) != "\n") return "CR";
            if (Mid(a, i + 2) == "\n") ret = "CRLF";
            return ret;

        }

        /// <summary>
        /// Calculates MD5 hash from string.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string md5(string source) {
            string hash;
             {
                using (MD5 md5Hash = MD5.Create()) {
                    byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(source));
                    StringBuilder sBuilder = new StringBuilder();
                    for (int i = 0; i < data.Length; i++) {
                        sBuilder.Append(data[i].ToString("x2"));
                    }
                    hash = sBuilder.ToString();
                }
            }
            return hash;
        }



        public static string Str(string str,int num) {
            if (num < 0) throw new Exception("Negative numbers not allowed in qstr.Str() request!");
            var ret = new StringBuilder(str.Length*num);
            for (int i = 0; i < num; i++) ret.Append(str);
            return ret.ToString();
        }

        public static string Right(string s, int l = 1) {
            if (l > s.Length) return s;
            return s.Substring(s.Length - l, l);
        }



        public static string Left(string s, int l = 1) {
            if (l > s.Length) return s;
            return s.Substring(0, l);
        }



        public static string Mid(string s, int pos, int l = 1) {
            /*
            if (pos + l > s.Length) return s;
            return s.Substring(pos - 1, l);
            */
            // This is slower, but at least it works.... I hope!
            var ret = "";
            for (int i = 0; i < l && i<s.Length; i++) ret += s.Substring(i + (pos - 1), 1);
            return ret;
        }



        public static int Len(string s) => s.Length;  // The only reason why I put this one in, was for quick translations from BlitzMax.
        public static string MyTrim(string s) => s.Trim(); // The only reason why I put this one in, was for Quick translations from Go, where I used this function, as the Trim feature in Go was impractical.
        public static string Chr(int i) => ((char)i).ToString(); // The C# method is just impractical. Sorry!
        public static string Upper(string s) => s.ToUpper(); // BlitzMax conversion
        public static string Lower(string s) => s.ToLower(); // BlitzMax conversion

        public static string ExtractExt(string myFilePath) {
            var ret = System.IO.Path.GetExtension(myFilePath);
            if (ret == "") { return ""; }
            return Right(ret, ret.Length - 1);
        }

        public static string StripExt(string myFilePath) {
            var d = ExtractDir(myFilePath);
            var f = StripDir(myFilePath);
            if (d=="")
                return $"{f.Substring(0, f.Length - System.IO.Path.GetExtension(f).Length)}";
            return $"{d}/{f.Substring(0, f.Length - System.IO.Path.GetExtension(f).Length)}";
        }



        public static string ExtractDir(string myFilePath) => System.IO.Path.GetDirectoryName(myFilePath).Replace("\\","/");
        public static string StripDir(string myFilePath) => System.IO.Path.GetFileName(myFilePath);



        public static bool Prefixed(string mystring, string prefix) => Left(mystring, prefix.Length) == prefix;
        public static bool Suffixed(string mystring, string suffix) => Right(mystring, suffix.Length) == suffix;



        public static string RemPrefix(string mystring, string fix) {
            var ms = mystring;
            if (Prefixed(ms, fix)) ms = Right(ms, ms.Length - fix.Length);
            return ms;
        }



        public static string RemSuffix(string mystring, string fix) {
            var ms = mystring;
            if (Suffixed(ms, fix)) ms = Left(ms, ms.Length - fix.Length);
            return ms;
        }



        public static byte ASC(string s, int offs = 0) {
            byte[] asciiBytes = System.Text.Encoding.ASCII.GetBytes(s);
            int o = offs;
            if (o >= asciiBytes.Length || o < 0) return 0;
            return asciiBytes[o];
        }



        public static string SafeString(string a){
            var ret = "";
            for (int i = 0; i < a.Length;i++){
                if (a[i] > 30 && a[i] < 123 && a[i]!='"') ret += Chr(a[i]);
                else {
                    switch (a[i]){
                        case '"': ret += "\\\""; break;
                        case '\b': ret += "\\b"; break;
                        case '\n': ret += "\\n"; break;
                        case '\r': ret += "\\r"; break;
                        case '\\': ret += "\\\\"; break;
                        default:
                            ret += "\\" + Right($"00{Convert.ToString(a[i], 8)}", 3);
                            break;
                    }
                }
            }
            return ret;
        }



        /// <summary>
        /// Returns a string with the wanted suffix if the suffix hasn't already been set yet!
        /// </summary>
        public static string SetSuffix(string ori,string suffix,bool casesensitive=false){
            if (Suffixed(ori, suffix) || (!casesensitive && Suffixed(ori.ToUpper(), suffix.ToUpper()))) return ori;
            return ori + suffix;

        }



        /// <summary>
        /// Returns a string with the wanted prefix if the suffix hasn't already been set yet!
        /// </summary>
        public static string SetPrefix(string ori, string prefix, bool casesensitive = false) {
            if (Prefixed(ori, prefix) || (!casesensitive && Prefixed(ori.ToUpper(), prefix.ToUpper()))) return ori;
            return prefix+ori;
        }



        /// <summary>
        /// Converts string to int if possible, and retuns 0 if not possible. You can use "$" as a prefix for hexadecimal numbers and "%" as a prefix for binary numbers.
        /// </summary>
        /// <returns>The integer if succesful otherwise 0.</returns>
        /// <param name="s">The string to convert.</param>
        public static int ToInt(string s) {
            var ret = 0;
            var s2i = s;
            try {
                switch (s[0]) {
                    case '$':
                        s2i = Right(s, s.Length - 1);
                        return System.Int32.Parse(s2i, System.Globalization.NumberStyles.HexNumber);
                    case '%':
                        s2i = Right(s, s.Length - 1);
                        ret = 0;
                        int bit = 1;
                        for (int i = s2i.Length; i > 0; i--) {
                            switch (Mid(s2i, i, 1)) {
                                case "1": ret += bit; break;
                                case "0": break;
                                default: return 0;
                            }
                            bit += bit;
                        }
                        break;
                }
                ret = System.Int32.Parse(s2i);
                return ret;

            } catch { return 0; }
        }



        /// <summary>
        /// Same as ToInt(), but then for long (64bit integers)
        /// </summary>
        public static long ToLong(string s) {
            long ret = 0;
            var s2i = s;
            try {
                switch (s[0]) {
                    case '$':
                        s2i = Right(s, s.Length - 1);
                        return System.Int64.Parse(s2i, System.Globalization.NumberStyles.HexNumber);
                    case '%':
                        s2i = Right(s, s.Length - 1);
                        ret = 0;
                        int bit = 1;
                        for (int i = s2i.Length; i > 0; i--) {
                            switch (Mid(s2i, i, 1)) {
                                case "1": ret += bit; break;
                                case "0": break;
                                default: return 0;
                            }
                            bit += bit;
                        }
                        break;
                }
                ret = System.Int64.Parse(s2i);
                return ret;
            } catch { return 0; }
        }

        public static double ToDouble(string s)  {
            double ret;
            try {
                ret = double.Parse(s);
            } catch { return 0; }
            return ret;
        }

        public static string[] Split(string str, string splitstring) => Regex.Split(str, splitstring);



        public static string sprintf(string input, params object[] inpVars) {
            // This function was authored by https://stackoverflow.com/users/598420/anirudha
            // https://stackoverflow.com/questions/14482341/c-net-and-sprintf-syntax
            // Now I am not fully happy about this, as it does support the basics %s and %d etc, but it has no support for %4d and stuff like that
            // But it's a start :P (RegEx has never been my forte).
            int i = 0;
            input = Regex.Replace(input, "%.", m => ("{" + i++/*increase have to be on right side*/ + "}"));
            //Console.WriteLine($"input = {input}");
            return string.Format(input, inpVars);
        }



        public static void printf(string input, params object[] inpVars) => Console.Write(input, inpVars);

        public static string OrText(string One, string Two) { if (One.Trim() != "") return One; else return Two; }

    }



}










