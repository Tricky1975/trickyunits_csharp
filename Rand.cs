// Lic:
// Rand.cs
// Quick randomizer
// version: 20.04.11
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


    static class Rand {
        static Rand() {
            MKL.Lic    ("Tricky Units for C# - Rand.cs","ZLib License");
            MKL.Version("Tricky Units for C# - Rand.cs","20.04.11");
        }

        static Random work = new Random();
        static public int Int(int min, int max) => work.Next(min, max+1);
        static public int Int(int max) => work.Next(0, max+1);

        static public int StepInt(int min,int max,int step) {
            var r = Int(min, max);
            return Math.Max(r - (r % step),0);
        }
    }

}