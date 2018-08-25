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
using System;



namespace TrickyUnits
{
    class QuickStream {
        Stream mystream;
        int position;
        bool LittleEndian = BitConverter.IsLittleEndian;
        byte Endian = 1; // 0 = Do not check, leave it to the CPU (NOT recommended), 1 = Always LittleEndian (default), 2 = Always big Endian;
        public long size { get { return mystream.Length; } }
        public bool EOF { get { return position >= size; } }

        public QuickStream(Stream setstream, byte EndianCode=1){
            mystream = setstream;
            position = 0;
            Endian = EndianCode;
        }

        byte[] GrabBytes(int num){
            byte[] ret = new byte[num];
            mystream.Read(ret, position, num);
            position += num;
            switch Endian {
                case 1:
                    if (!LittleEndian) { Array.Reverse(ret); }
                    break;
                case 2:
                    if (LittleEndian) { Array.Reverse(ret); }
                    break;
            }
            return ret;
        }

        public int ReadInt()  { return BitConverter.ToInt32(GrabBytes(4), 0); }
        public long ReadLong() { return BitConverter.ToInt64(GrabBytes(8), 0); }
        public byte ReadByte() {
            byte[] b = new byte[1];
            mystream.Read(b, position, 1);
            position++;
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

    }
    class QOpen
    {

        static QOpen()
        {
            MKL.Version("Tricky Units for C# - qstream.cs","18.08.25");
            MKL.Lic    ("Tricky Units for C# - qstream.cs","ZLib License");
        }
    }
}

