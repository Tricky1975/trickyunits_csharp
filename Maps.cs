// Lic:
// Maps.cs
// Alternate Maps
// version: 19.08.05
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
using System.Diagnostics;
using System.Collections.Generic;

namespace TrickyUnits {

    delegate void MapError(string Message);



    class StringMap {
        public readonly SortedDictionary<string, string> Map;
        public bool MustExist = false;
        MapError Err = delegate (string Msg) { Console.WriteLine(Msg); Debug.WriteLine(Msg); };
        public MapError Error {
            set {
                Err = value ?? throw new Exception("NULL not allowed for error handler!");
            }
            get {
                throw new Exception("Error field is write-only!");
            }
        }

        string this[string key] {
            get {
                if (!Map.ContainsKey(key)) {
                    if (MustExist)
                        Err?.Invoke($"StringMap does not contain key {key}");
                    return "";
                }
                return Map[key];
            }
            set => Map[key] = value;
        }

        SortedDictionary<string, string>.KeyCollection Keys => Map.Keys;


        public StringMap() { Map = new SortedDictionary<string, string>(); }
        public StringMap(SortedDictionary<string, string> M) { Map = M; }
        public StringMap(Dictionary<string, string> M) {
            Map = new SortedDictionary<string, string>();
            foreach (string k in M.Keys) Map[k] = M[k];
        }
    }

    class TMap<MapKey, MapValue> {
        public readonly Dictionary<MapKey, MapValue> Map;
        public bool MustExist = false;
        MapError Err = delegate (string Msg) { Console.WriteLine(Msg); Debug.WriteLine(Msg); };
        public MapError Error {
            set {
                Err = value ?? throw new Exception("NULL not allowed for error handler!");
            }
            get {
                throw new Exception("Error field is write-only!");
            }
        }

        MapValue this[MapKey key] {
            set { Map[key] = value; }
            get {
                if (!Map.ContainsKey(key)) {
                    if (MustExist)
                        Err?.Invoke($"StringMap does not contain key {key.ToString()}");
                    return default(MapValue);
                }
                return Map[key];
            }
        }
        Dictionary<MapKey, MapValue>.KeyCollection Keys => Map.Keys;
    }
}
