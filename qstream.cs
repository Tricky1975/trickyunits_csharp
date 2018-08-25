// Lic:
//   qstream.cs
//   
//   version: 18.08.25
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
using System.IO;
using System.Text;
using System.Collections.Generic;
using System;



namespace TrickyUnits
{
    class QuickStream {
        Stream mystream;
        int truepos;
        public long Size { get { return mystream.Length; } }
        public int Position { set { truepos = value; if (EOF) truepos = (int)Size; } get { return truepos; }}
        readonly bool LittleEndian = BitConverter.IsLittleEndian;
        readonly byte Endian = 1; // 0 = Do not check, leave it to the CPU (NOT recommended), 1 = Always LittleEndian (default), 2 = Always big Endian;
        public bool EOF { get { return Position >= Size; } }

        public QuickStream(Stream setstream, byte EndianCode=1){
            mystream = setstream;
            Position = 0;
            Endian = EndianCode;
        }

        byte[] GrabBytes(int num){
            byte[] ret = new byte[num];
            mystream.Read(ret, Position, num);
            Position += num;
            switch (Endian) {
                case 1:
                    if (!LittleEndian) { Array.Reverse(ret); }
                    break;
                case 2:
                    if (LittleEndian) { Array.Reverse(ret); }
                    break;
            }
            return ret;
        }

        byte[] ReadBytes(int number,bool checkendian=false){
            if (checkendian) return GrabBytes(number);
            byte[] ret = new byte[number];
            mystream.Read(ret, Position, number);
            Position += number;
            return ret;
        }

        public int ReadInt()  { return BitConverter.ToInt32(GrabBytes(4), 0); }
        public long ReadLong() { return BitConverter.ToInt64(GrabBytes(8), 0); }
        public byte ReadByte() {
            byte[] b = new byte[1];
            mystream.Read(b, Position, 1);
            Position++;
            return b[0];
        }
        public string ReadString(int length=0){
            int l = length;
            if (l == 0) { l = ReadInt(); }
            var ret = new byte[l];
            for (int i = 0; i < l; i++) { ret[i] = ReadByte(); }
            return Encoding.Default.GetString(ret);
        }
        public bool ReadBoolean() { return ReadByte() > 0; }
        public uint ReadUnSignedInt() { return BitConverter.ToUInt32(GrabBytes(4), 0); }
        public ulong ReadUnSignedLong() { return BitConverter.ToUInt64(GrabBytes(8), 0); }
        public string ReadNullString(){ // Reads a null-terminated string, which is a very common way to end a string in C
            var np = Position;
            var ln = 0;
            byte ch;
            List<byte> chs = new List<byte>();
            do
            {
                ch = ReadByte();
                if (ch > 0)
                {
                    ln++;
                    chs.Add(ch);
                }
            } while (ch > 0);
            return Encoding.Default.GetString(chs.ToArray());
        }

        public void Close() { mystream.Close();  }

    }
    static class QOpen
    {
        public const byte NoEndian = 0;
        public const byte LittleEndian = 1;
        public const byte BigEndian = 2;

        static QOpen()
        {
            MKL.Version("Tricky Units for C# - qstream.cs","18.08.25");
            MKL.Lic    ("Tricky Units for C# - qstream.cs","ZLib License");
        }

        public static QuickStream ReadFile(string filename,byte EndianCode=LittleEndian){
            var s = File.OpenRead(filename);
            return new QuickStream(s, EndianCode);
        }
    }
}

