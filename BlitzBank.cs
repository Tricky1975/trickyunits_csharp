// Lic:
// BlitzBank.cs
// Blitz Bank
// version: 19.08.12
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
namespace TrickyUnits {

    /*
     This is just a fake class, to make conversions from Blitz to C# easier
    */

        enum BlitzEndian { None,Little,Big }

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


        public BlitzBank(int size,BlitzEndian Endian=BlitzEndian.Little) { Buffer = new byte[size]; WantEndian = Endian; }

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

        public int PeekShort(int addr) {
            if (Endian != BlitzEndian.Little) throw new Exception($"BlitzBank.PeekShort({addr}): Requires Little Endian processor!");
            var bf = new byte[4];
            bf[0] = Peek(addr + 0);
            bf[1] = Peek(addr + 1);
            bf[2] = 0;
            bf[3] = 0;
            return BitConverter.ToInt32(bf,0);
        }
    }

