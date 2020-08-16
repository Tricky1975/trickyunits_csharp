// Lic:
// qstream.cs
// TrickyUnits - Quick Stream
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


using System.IO;
using System.Text;
using System.Collections.Generic;
using System;



namespace TrickyUnits {
    public class QuickStream {
        Stream mystream;
        long truepos;
        bool closed = false;



        /// <summary>
        /// Gets the size of a stream.
        /// </summary>
        /// <value>The size.</value>
        public long Size { get { return mystream.Length; } }

        /// <summary>
        /// Gets or sets the position of the stream (can be used for seeking). When trying to seek beyond the EOF point, this routine will automatically seek the end of the file.
        /// </summary>
        /// <value>The position.</value>
        public long Position {
            set {
                truepos = value;
                if (EOF) truepos = Size;
                mystream.Seek(truepos, 0);
            }
            get { return truepos; }
        }

        readonly bool IsLittleEndian = BitConverter.IsLittleEndian;

        readonly byte Endian = 1; // 0 = Do not check, leave it to the CPU (NOT recommended), 1 = Always IsLittleEndian (default), 2 = Always big Endian;



        /// <summary>
        /// Gets a value indicating whether this <see cref="T:TrickyUnits.QuickStream"/> reached the end of the stream/file.
        /// </summary>
        /// <value><c>true</c> if EOF; otherwise, <c>false</c>.</value>
        public bool EOF { get { return Position >= Size; } }



        public QuickStream(Stream setstream, byte EndianCode = 1, bool norepos = false) {
            Hello();
            mystream = setstream;
            if (!norepos) Position = 0;
            Endian = EndianCode;
        }





        /// <summary>
        /// Returns the pure stream variable as used by C# itself. This function is only meant for advanced usage when QuickStream does not provide the functionality required for this operation.
        /// </summary>
        /// <returns>The stream.</returns>

        public Stream GetStream() { return mystream; }



        byte[] GrabBytes(int num) {
            byte[] ret = new byte[num];
            mystream.Read(ret, 0, num);
            truepos += num;
            switch (Endian) {
                case 1:
                    if (!IsLittleEndian) { Array.Reverse(ret); }
                    break;
                case 2:
                    if (IsLittleEndian) { Array.Reverse(ret); }
                    break;
            }
            return ret;
        }



        /// <summary>
        /// Reads a set of bytes into a byte array. The checkendian bool can be used to auto-reverse the array based on the endian settings of your CPU and the setting you gave while opening this stream.
        /// </summary>
        /// <returns>The byte array.</returns>
        /// <param name="number">Number bytesto be read.</param>
        /// <param name="checkendian">If set to <c>true</c> checkendian.</param>
        public byte[] ReadBytes(int number, bool checkendian = false) {
            if (checkendian) return GrabBytes(number);
            byte[] ret = new byte[number];
            mystream.Read(ret, 0, number);
            truepos += number;
            return ret;
        }


        /// <summary>
        /// Reads a string like a C null terminated string
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        public string ReadNullTerminatedString(int len) {
            var r = new StringBuilder(len);
            var b = ReadBytes(len);
            for (int i = 0; i < len && b[i] != 0; i++) r.Append((char)b[i]);
            return r.ToString();
        }



        /// <summary>
        /// Reads a 32bit integer from the stream.
        /// </summary>
        /// <returns>The int.</returns>
        public int ReadInt() { return BitConverter.ToInt32(GrabBytes(4), 0); }



        /// <summary>
        /// Reads a 64bit integer from the stream
        /// </summary>
        /// <returns>The long.</returns>
        public long ReadLong() { return BitConverter.ToInt64(GrabBytes(8), 0); }


        /// <summary>
        /// Reads a single byte.
        /// </summary>
        /// <returns>The byte.</returns>
        public byte ReadByte() {
            byte[] b = new byte[1];
            mystream.Read(b, 0, 1);
            truepos++;
            return b[0];
        }

        public string ReadString(int length = 0) {
            int l = length;
            if (l == 0) { l = ReadInt(); }
            var ret = new byte[l];
            for (int i = 0; i < l; i++) { ret[i] = ReadByte(); }
            //return Encoding.Default.GetString(ret);
            return Encoding.ASCII.GetString(ret);
        }



        /// <summary>
        /// Reads a line and returns it as a string. 
        /// This function should be compatible with both Unix and Windows new-line markers.
        /// </summary>
        /// <returns>Line as a string</returns>
        public string ReadLine() {
            /* faster but doesn't work
            var o = Position;
            var x = ReadByte();
            while ((!EOF) && x != 10 && x != 13) {
                x = ReadByte();
                Position++;
            }
            var e = Position;
            var c = e;
            if (!EOF) {
                e = Position - 1;
                x = ReadByte();
                if (x == 10 || x == 13) c++;
            }
            Position = o;
            int l = (int)e - (int)o;
            string ret = ReadString(l);
            if (ret.Contains("\n") || ret.Contains("\r"))
                throw new Exception($"INTERNAL ERROR! -- ReadLine output not correct!\nPosition {Position} / {ret} / \\n{ret.IndexOf('\n')} / \\r{ret.IndexOf('\r')} / o{o} / c{c} / e{e} / l{l}");
            return ret;
            */


            // This is very extremely slow, but at least it should work... right?
            var ret = new StringBuilder();
            byte x;
            byte x2;
            while (true) {
                x = ReadByte();
                if (x == 10 || x == 13 || EOF) break;
                ret.Append($"{(char)x}"); //(qstr.Chr(x)); 
            }
            if (!EOF) {
                x2 = ReadByte();
                if (x != 13 || x != 10 || x == x2) Position--;
            }
            return ret.ToString();
        }



        public bool ReadBoolean() { return ReadByte() > 0; }
        public uint ReadUnSignedInt() { return BitConverter.ToUInt32(GrabBytes(4), 0); }
        public ulong ReadUnSignedLong() { return BitConverter.ToUInt64(GrabBytes(8), 0); }
        public string ReadNullString() { // Reads a null-terminated string, which is a very common way to end a string in C
            var np = Position;
            var ln = 0;
            byte ch;
            List<byte> chs = new List<byte>();
            do {
                ch = ReadByte();
                if (ch > 0) {
                    ln++;
                    chs.Add(ch);
                }
            } while (ch > 0);
            //return Encoding.Default.GetString(chs.ToArray());
            return Encoding.ASCII.GetString(chs.ToArray());
        }



        void PutBytes(byte[] bytes) {
            switch (Endian) {
                case 1:
                    if (!IsLittleEndian) { Array.Reverse(bytes); }
                    break;
                case 2:
                    if (IsLittleEndian) { Array.Reverse(bytes); }
                    break;
            }
            mystream.Write(bytes, 0, bytes.Length);
            truepos += bytes.Length;
        }



        /// <summary>
        /// Writes a byte array into a stream. The checkendian bool can be used to auto-reverse the array based on the endian settings of your CPU and the setting you gave while opening this stream.
        /// </summary>
        /// <param name="bytes">Bytes.</param>
        /// <param name="checkendian">If set to <c>true</c> checkendian.</param>
        public void WriteBytes(byte[] bytes, bool checkendian = false) {
            if (checkendian) { PutBytes(bytes); return; }
            mystream.Write(bytes, 0, bytes.Length);
            truepos += bytes.Length;
        }



        /// <summary>
        /// Writes a byte to the file
        /// </summary>
        /// <param name="b">The blue component.</param>
        public void WriteByte(byte b) {
            byte[] ba = new byte[1]; ba[0] = b;
            mystream.Write(ba, 0, 1);
            truepos++;
        }

        /// <summary>
        /// Writes a 32bit integer to the file
        /// </summary>
        /// <param name="i">The index.</param>
        public void WriteInt(int i) {
            byte[] bytes = BitConverter.GetBytes(i);
            PutBytes(bytes);
        }


        /// <summary>
        /// Writes a 64bit integer to the file
        /// </summary>
        /// <param name="i">The index.</param>
        public void WriteLong(long i) {
            byte[] bytes = BitConverter.GetBytes(i);
            PutBytes(bytes);
        }

        /// <summary>
        /// Writes a string. When "raw" is set to true, WriteString will only write the string itself. When set to false, it will prefix it with an 32bit integer containing the length of the string. (the byteorder of this interger depends on the Endian Settings given when opening the stream).
        /// </summary>
        /// <param name="s">S.</param>
        /// <param name="raw">If set to <c>true</c> raw.</param>
        public void WriteString(string s, bool raw = false) {
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            if (!raw) WriteInt(s.Length);
            PutBytes(bytes);
        }


        /// <summary>
        /// Writes a boolean value as a byte (1 for true and 0 for <see langword="false"/>)
        /// </summary>
        /// <param name="b">If set to <c>true</c> b.</param>
        public void WriteBool(bool b) {
            if (b) { WriteByte(1); } else { WriteByte(0); }
            truepos++;
        }


        /// <summary>
        /// Closes the QuickStream
        /// </summary>
        public void Close() {
            if (!closed) { mystream.Close(); }
            closed = true;
        }


        ~QuickStream() { Close(); }


        /// <summary>
        /// Opens the first entry it can find embedded in the Assembly suffixed with the given suffix as a Quickstream.
        /// </summary>
        /// <param name="suffix"></param>
        /// <param name="Endian"></param>
        /// <returns></returns>
        static public QuickStream OpenEmbedded(string suffix, byte Endian = LittleEndian) {
            foreach (string name in System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames()) {
                //System.Diagnostics.Debug.WriteLine($"Emb>{name}");
                if (name.EndsWith(suffix, StringComparison.InvariantCultureIgnoreCase)) {
                    Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
                    return new QuickStream(stream, Endian);

                }
            }
            throw new Exception($"No embedded entries suffixed {suffix} found, in this Assembly!");
        }

        /// <summary>
        /// Finds all embeds with a certain suffix in this assembly and returns them as a string array.
        /// </summary>
        /// <param name="suffix"></param>
        /// <returns></returns>
        static public string[] EmbedList(string suffix = "") {
            var ret = new List<string>();
            foreach (string name in System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames()) {
                //System.Diagnostics.Debug.WriteLine($"Emb>{name}");
                if (name.EndsWith(suffix, StringComparison.InvariantCultureIgnoreCase)) {
                    ret.Add(name);
                }
            }
            return ret.ToArray();
        }

                static public string StringFromEmbed(string suffix) {
            var bt = OpenEmbedded(suffix);
            var ret = bt.ReadString((int)bt.Size);
            bt.Close();
            return ret;
        }

        // former QOpen class
        public const byte NoEndian = 0;
        public const byte LittleEndian = 1;
        public const byte BigEndian = 2;
        public static Stack<string> PushedDirs = new Stack<string>();

        public static void Hello() {
            MKL.Version("Tricky Units for C# - qstream.cs","20.08.16");
            MKL.Lic    ("Tricky Units for C# - qstream.cs","ZLib License");
        } // Basically does nothing, but it forces the MKL data to be parsed when called.


        public static void PushDir() => PushedDirs.Push(Directory.GetCurrentDirectory());
        public static void PopDir() => Directory.SetCurrentDirectory(PushedDirs.Pop());

        /// <summary>
        /// Opens a file for quick readon
        /// </summary>
        /// <returns>The file as a QuickStream.</returns>
        /// <param name="filename">File name.</param>
        /// <param name="EndianCode">Endian code.</param>
        public static QuickStream ReadFile(string filename, byte EndianCode = LittleEndian) {
            var s = File.OpenRead(filename);
            return new QuickStream(s, EndianCode);
        }

        public static QuickStream AppendFile(string filename, byte EndianCode = LittleEndian) {
            var s = File.Open(filename, FileMode.Append);
            var r = new QuickStream(s, EndianCode, true);
            //r.Position = r.Size;
            return r;
        }



        /// <summary>
        /// Opens a file for writing
        /// </summary>
        /// <returns>The file as a QuickStream.</returns>
        /// <param name="filename">Filename.</param>
        /// <param name="EndiancCode">Endianc code.</param>
        public static QuickStream WriteFile(string filename, byte EndiancCode = LittleEndian) {
            if (File.Exists(filename)) { File.Delete(filename); }
            var s = File.OpenWrite(filename);
            return new QuickStream(s, EndiancCode);
        }


        /// <summary>
        /// Creates a QuickStream from a byte buffer
        /// </summary>
        /// <returns>The QuickStream</returns>
        /// <param name="buffer">Buffer.</param>
        /// <param name="Endian">Endian.</param>
        public static QuickStream StreamFromBytes(byte[] buffer, byte Endian = LittleEndian) {
            return new QuickStream(new MemoryStream(buffer), Endian);
        }



        /// <summary>
        /// Saves an entire string as a file
        /// </summary>
        /// <param name="filename">Filename.</param>
        /// <param name="thestring">The string.</param>
        public static void SaveString(string filename, string thestring) {
            File.WriteAllText(filename, thestring);
            // var bt = WriteFile(filename);
            // bt.WriteString(thestring, true);
            //bt.Close();
        }

        public static void SaveBytes(string filename, byte[] buf) => File.WriteAllBytes(filename, buf);

        /// <summary>
        /// Loads an entire file as a string
        /// </summary>
        /// <returns>The string.</returns>
        /// <param name="filename">Filename.</param>
        public static string LoadString(string filename) {
            var bt = ReadFile(filename);
            var st = bt.ReadString((int)bt.Size);
            bt.Close();
            return st;
        }

        /// <summary>
        /// Lists out a text file in which each array item contains a line. This routine is compatible with both Windows/DOS based text files as well as Unix based text files (as used on Mac, Linux, and other systems inspired by Unix).
        /// </summary>
        /// <param name="filename">The file to examine.</param>
        /// <param name="winfile">Will be assinged "true" if the file was detected to be a Windows file. Otherwise it will return false.</param>
        /// <param name="crash">If set to true your program will throw an exception when the file is detected as a binary file. When set to false, it just returns 'null'</param>
        /// <returns>The text as a string[]</returns>
        public static string[] LoadLines(string filename, ref bool winfile, bool crash = true) {
            var ls = LoadString(filename);
            var i = ls.IndexOf((char)26);
            if (i < ls.Length && i >= 0) {
                if (crash) throw new Exception("LoadLines does not work on a binary file!");
                return null;
            }
            i = ls.IndexOf((char)13);
            if (i >= 0 && i < ls.Length - 1) {
                // detected as Windows text, so let's parse it as such
                winfile = true;
                return ls.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            }
            // When not detected as Windows, let's assume we're just working on Unix
            winfile = false;
            return ls.Split('\n');
        }
        public static string[] LoadLines(string filename) { bool ignore = false; return LoadLines(filename, ref ignore); }


        /// <summary>
        /// A routine that loads a stringmap file into a full string Dictionary.
        /// </summary>
        /// <returns>The string map.</returns>
        /// <param name="filename">Filename.</param>
        public static Dictionary<string, string> LoadStringMap(string filename) {
            var bt = ReadFile(filename);
            var ret = new Dictionary<string, string>();
            while (!bt.EOF) {
                var key = bt.ReadString();
                var value = bt.ReadString();
                ret[key] = value;
            }
            bt.Close();
            return ret;
        }



        /// <summary>
        /// Loads a file into a byte array
        /// </summary>
        /// <returns>The file.</returns>
        /// <param name="filename">Filename.</param>
        public static byte[] GetFile(string filename) {
            var bt = ReadFile(filename);
            var ret = bt.ReadBytes((int)bt.Size);
            bt.Close();
            return ret;
        }
    }









    // for deprecation notes



    /// <summary>
    /// Deprecated! Do not use! Use the QuickStream class in stead!
    /// </summary>
    class QOpen {
        public const byte NoEndian = 0;
        public const byte LittleEndian = 1;
        public const byte BigEndian = 2;

        static void d() { Console.Beep(); Console.WriteLine($"{((char)7).ToString()}WARNING!!!\nA call to the deprecated QOpen class was done! Notify the coder of this program to have it replaced by QuickStream"); }


        public static void Hello() { d(); QuickStream.Hello(); }
        public static QuickStream ReadFile(string filename, byte EndianCode = LittleEndian) { d(); return QuickStream.ReadFile(filename, EndianCode); }
        public static QuickStream WriteFile(string filename, byte EndiancCode = LittleEndian) { d(); return QuickStream.WriteFile(filename, EndiancCode); }
        public static QuickStream StreamFromBytes(byte[] buffer, byte Endian = LittleEndian) { d(); return QuickStream.StreamFromBytes(buffer, Endian); }
        public static void SaveString(string filename, string thestring) { d(); QuickStream.SaveString(filename, thestring); }
        public static string LoadString(string filename) { d(); return QuickStream.LoadString(filename); }
        public static Dictionary<string, string> LoadStringMap(string filename) { d(); return QuickStream.LoadStringMap(filename); }
        public static byte[] GetFile(string filename) { d(); return QuickStream.GetFile(filename); }

        static QOpen() => d();

    }

}