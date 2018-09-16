// Lic:
//   qstr.cs
//   Quick String Functions
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

namespace TrickyUnits
{
    /// <summary>
    /// Just some quick functions to use with strings.
    /// Nothing special :P
    /// </summary>
    class qstr
    {
        static qstr()
        {
            MKL.Version("Tricky Units for C# - qstr.cs","18.09.16");
            MKL.Lic    ("Tricky Units for C# - qstr.cs","ZLib License");
        }

        public static string Right(string s, int l=1){
            if (l > s.Length) return s;
            return s.Substring(s.Length - l, l);
        }

        public static string Left(string s, int l=1){
            if (l > s.Length) return s;
            return s.Substring(0, l);

        }

        public static string Mid(string s, int pos, int l=1){
            if (pos + l > s.Length) return s;
            return s.Substring(pos - 1, l);
        }

        public static int Len(string s) =>  s.Length;  // The only reason why I put this one in, was for quick translations from BlitzMax.
        public static string MyTrim(string s) => s.Trim(); // The only reason why I put this one in, was for Quick translations from Go, where I used this function, as the Trim feature in Go was impractical.
        public static string Chr(int i) => ((char)i).ToString(); // The C# method is just impractical. Sorry!
        public static string Upper(string s) => s.ToUpper(); // BlitzMax conversion
        public static string Lower(string s) => s.ToLower(); // BlitzMax conversion

        public static string ExtractExt(string myFilePath){
            var ret = System.IO.Path.GetExtension(myFilePath);
            if (ret == "") { return ""; }
            return Right(ret, ret.Length - 1);
        }

        public static string ExtractDir(string myFilePath) => System.IO.Path.GetDirectoryName(myFilePath);

        public static bool Prefixed(string mystring, string prefix) => Left(mystring, prefix.Length) == prefix;
        public static bool Suffixed(string mystring, string suffix) => Right(mystring, suffix.Length) == suffix;

        public static byte ASC(string s,int offs=0){
            byte[] asciiBytes = System.Text.Encoding.ASCII.GetBytes(s);
            int o = offs;
            if (o >= asciiBytes.Length || o<0) return 0;
            return asciiBytes[o];
        }

    }
}
