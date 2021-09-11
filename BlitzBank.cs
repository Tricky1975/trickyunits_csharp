// Lic:
// BlitzBank.cs
// Blitz Bank
// version: 21.09.11
// Copyright (C) 2019, 2021 Jeroen P. Broks
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
namespace TrickyUnits {

    /*
     This is just a fake class, to make conversions from Blitz to C# easier
     For now only LittleEndian is supported, however if I need it, this may be expanded in the future.
    */

    enum BlitzEndian { None, Little, Big }

    class BlitzBank {

        byte[] Buffer;

        readonly BlitzEndian WantEndian;
        BlitzEndian _Endian = BlitzEndian.None;
        public BlitzEndian Endian {
            get {
                if (_Endian == BlitzEndian.None) {
                    var q = BitConverter.GetBytes((int)200);
                    if (q[0] == 200)
                        _Endian = BlitzEndian.Little;
                    else
                        _Endian = BlitzEndian.Big;
                }
                return _Endian;
            }
        }


        public BlitzBank(int size, BlitzEndian Endian = BlitzEndian.Little) { Buffer = new byte[size]; WantEndian = Endian; }
        public BlitzBank(byte[] LBuf,BlitzEndian Endian=BlitzEndian.Little, bool cpylink = false) {
            WantEndian = Endian;
            if (cpylink) { Buffer = LBuf; return; } // Faster, friendlier on your RAM but can get nasty results if you are not sure about what you are doing!
            Buffer = new byte[LBuf.Length];
            for (int i = 0; i < LBuf.Length; ++i) Buffer[i] = LBuf[i];
        }

        public void ResizeBank(int size) {
            Buffer = new byte[size];
        }

        public void Poke(int a, byte i) {
            if (a < 0 || a >= Buffer.Length) throw new Exception($"Poke({a},{i}): Out of bounds (0-{Buffer.Length - 1})!");
            Buffer[a] = i;
        }

        public void PokeShort(int addr, int i) {
            if (Endian != BlitzEndian.Little) throw new Exception($"BlitzBank.PokeShort({addr},{i}): Requires Little Endian processor!");
            var bf = BitConverter.GetBytes(i);
            Poke(addr + 0, bf[0]);
            Poke(addr + 1, bf[1]);
        }

        public byte Peek(int a) {
            if (a < 0 || a >= Buffer.Length) throw new Exception($"Peek({a}): Out of bounds (0-{Buffer.Length - 1})!");
            return Buffer[a];
        }

        public int PeekShort32(int addr) {
            if (Endian != BlitzEndian.Little) throw new Exception($"BlitzBank.PeekShort({addr}): Requires Little Endian processor!");
            var bf = new byte[4];
            bf[0] = Peek(addr + 0);
            bf[1] = Peek(addr + 1);
            bf[2] = 0;
            bf[3] = 0;
            return BitConverter.ToInt32(bf, 0);
        }

        public int PeekInt(int addr) {
            if (Endian != BlitzEndian.Little) throw new Exception($"BlitzBank.PeekShort({addr}): Requires Little Endian processor!");
            var bf = new byte[4];
            bf[0] = Peek(addr + 0);
            bf[1] = Peek(addr + 1);
            bf[2] = Peek(addr + 2);
            bf[3] = Peek(addr + 3);
            return BitConverter.ToInt32(bf, 0);
        }


        public short PeekShort(int addr) {
            if (Endian != BlitzEndian.Little) throw new Exception($"BlitzBank.PeekShort({addr}): Requires Little Endian processor!");
            var bf = new byte[2];
            bf[0] = Peek(addr + 0);
            bf[1] = Peek(addr + 1);
            return BitConverter.ToInt16(bf, 0);
        }

        public long PeekLong(int addr) {
            if (Endian != BlitzEndian.Little) throw new Exception($"BlitzBank.PeekShort({addr}): Requires Little Endian processor!");
            try {
                var bf = new byte[8];
                bf[0] = Peek(addr + 0);
                bf[1] = Peek(addr + 1);
                bf[2] = Peek(addr + 2);
                bf[3] = Peek(addr + 3);
                bf[4] = Peek(addr + 4);
                bf[5] = Peek(addr + 5);
                bf[6] = Peek(addr + 6);
                bf[7] = Peek(addr + 7);
                return BitConverter.ToInt64(bf, 0);
            } catch (Exception E) {
                Console.WriteLine($".NET error! {E.Message}\n{addr}/{Buffer.Length}");
#if DEBUG
                Console.WriteLine(E.StackTrace);
#endif
                return 0;
            }
        }
    }
}