// Lic:
// GINIE.cs
// GINIE Is Not INI Either
// version: 21.09.11
// Copyright (C) 2020, 2021 Jeroen P. Broks
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
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace TrickyUnits {

	class GINIE_IllegalTag : Exception {
		public GINIE_IllegalTag() { }

		override public string Message => "Illegal tag in source. Was \"[\" closed by a proper \"]\"?";
		
	}

	class GINIE_SysRequestNotUnderstood : Exception {
		public GINIE_SysRequestNotUnderstood(string req) { this.req = req; }
		private string req;

		override public string Message => $"System request not understood => {req}";

	}

	class GINIE_IllegalEnd : Exception {
		public GINIE_IllegalEnd() { }

		override public string Message => "*END instruction unexpected";

	}

	class GINIE_NoTag : Exception {
		public GINIE_NoTag() { }
		public override string Message => "No tag";
	}

	class GINIE_IllegalDefinition : Exception {
		public GINIE_IllegalDefinition(string d) { this.d = d; }
		string d;
		public override string Message => $"Illegal Defintiion: {d}";
	}

	class GINIE_ByteNotGINIE : Exception {
		public GINIE_ByteNotGINIE() { }
		public override string Message => "Byte code is NOT GINIE";
	}

	class GINIE_ByteUnkTag : Exception {
		byte tag = 0;
		public GINIE_ByteUnkTag(byte _tag) { tag = _tag; }
		public override string Message => $"Unknown instruction tag in byte code: ({tag})";
	}

	// GINIE is not INI, either!
	/// <summary>
	/// GINIE = GINIE is not INIE, either! A class almost compatible with INI, but with a few different twists.
	/// </summary>
	public class GINIE {

		static public void Hello() {
			MKL.Lic    ("Tricky Units for C# - GINIE.cs","ZLib License");
			MKL.Version("Tricky Units for C# - GINIE.cs","21.09.11");
		}

		private GINIE() { Hello(); }


		/// <summary>
		///  Parse GINIE source into data
		/// </summary>
		/// <param name="source">Source code</param>
		/// <returns></returns>
		static public GINIE FromSource(string[] source) {
			var ret = new GINIE();
			var tag = "";
			var list = "";
			//if (!merge) Clear();
			foreach (string l in source) {
				var line = l.Trim();
				if (line != "") {
					switch (line[0]) {
						case '\\':
							line = line.Substring(1);
							goto default;
						case '#':
						case ';':
							if (list != "") {
								ret.List(tag, list).Add(line);
								break;
							}
							break;
						case '[':
							if (list != "") {
								ret.List(tag, list).Add(line);
								break;
							}
							if (line[line.Length - 1] != ']') {
								Debug.WriteLine($"Line=\"{line}\"; #line={line.Length}; 0='{line[0]}' {line.Length-1}='{line[line.Length-1]}'");
								throw new GINIE_IllegalTag();
							}
							tag = line.Substring(1, line.Length - 2).Trim().ToUpper();
							break;
						case '*':
							if (qstr.Prefixed(line.ToUpper(), "*LIST:")) {
								if (tag == "") throw new GINIE_NoTag();
								list = line.Substring(6);
								ret.List(tag, list); // Make sure the list exists!
								break;
							}
							if (line.ToUpper()=="*END") {
								if (list!="") { list = "";break; }
								throw new GINIE_IllegalEnd();
							}
							throw new GINIE_SysRequestNotUnderstood(line);
						default:
							if (tag == "") {
#if DEBUG
								Console.WriteLine($"No tag in line '{line}'");
#endif
								throw new GINIE_NoTag();
							}
							if (tag == "*LIC*" || tag == "*REM*") break;
							if (list != "") {
								ret.List(tag, list).Add(line);
								break;
							}
							var p = line.IndexOf('=');
							if (p <= -1) throw new GINIE_IllegalDefinition(line);
							var key = line.Substring(0, p);
							var val = line.Substring(p + 1);
							for(int c = 0; c < 255; c++) {
								key = key.Replace($"\\{c.ToString("03d")}", $"{(char)c}");
								val = val.Replace($"\\{c.ToString("03d")}", $"{(char)c}");
							}
							ret[tag, key] = val;
							break;
					}
				}
			}
			return ret;
		}
		
		static public GINIE FromSource(string source) => FromSource(source.Split('\n'));
		static public GINIE FromSource(List<string> source) => FromSource(source.ToArray());

		static public GINIE FromSource(byte[] source) => FromSource(Encoding.Default.GetString(source));

		static public GINIE FromAuto(byte[] Buf) {			
			const string Head = "GENIE\x1b";
			bool ByteCode = Buf.Length == Head.Length;
			for (byte i = 0; i < Head.Length; i++) ByteCode = ByteCode && Head[i] == Buf[i];
			if (ByteCode) {
				var ret = new GINIE();
				ret.FromBytes(Buf);
				return ret;
			} else {
				return FromSource(Buf);
			}
				
		}

		public void Auto(byte[] Buf) {
			const string Head = "GENIE\x1b";
			bool ByteCode = Buf.Length == Head.Length;
			for (byte i = 0; i < Head.Length; i++) ByteCode = ByteCode && Head[i] == Buf[i];
			if (ByteCode) {
				var ret = new GINIE();
				ret.FromBytes(Buf);
				/*
				Lists.Clear();
				Values.Clear();
				foreach(var c in ret.Lists) {
					foreach (var k in c.Value)
						List(c.Key, k.Key).AddRange(ret.List(c.Key, k.Key));
				}
				*/
				Lists = ret.Lists;
				Values = ret.Values;
			} else {
				FromSource(Buf);
			}
		}

		static public GINIE FromFile(string file,bool allownonexistent = true) {
			if (!File.Exists(file)) {
				if (allownonexistent)
					return new GINIE();
				else
					return null;                    
			}
			// TODO: Auto-detect binary form if existent
			return FromSource(QuickStream.LoadString(file));
		}

		static public GINIE Empty() => new GINIE();

		SortedDictionary<string, SortedDictionary<string, string>> Values = new SortedDictionary<string, SortedDictionary<string, string>>();
		SortedDictionary<string, SortedDictionary<string, List<string>>> Lists  = new SortedDictionary<string, SortedDictionary<string, List<string>>>();
		public string AutoSaveSource = "";

		public void ClearCat(string sec) {
			sec = sec.ToUpper();
			if (Lists.ContainsKey(sec)) Lists[sec].Clear();
			if (Values.ContainsKey(sec)) Values[sec].Clear();
			if (AutoSaveSource != "") SaveSource(AutoSaveSource);
		}

		public void KillCat(string sec) {
			if (Lists.ContainsKey(sec)) Lists.Remove(sec);
			if (Values.ContainsKey(sec)) Values.Remove(sec);
			if (AutoSaveSource != "") SaveSource(AutoSaveSource);
		}

		public List<string> List(string sec, string key) {
			sec = sec.ToUpper();
			key = key.ToUpper();
			if (!Lists.ContainsKey(sec)) Lists[sec] = new SortedDictionary<string, List<string>>();
			if (!Lists[sec].ContainsKey(key)) Lists[sec][key] = new List<string>();
			return Lists[sec][key];
		}

		public bool HasList(string sec,string key) {
			sec = sec.ToUpper();
			key = key.ToUpper();
			//Console.WriteLine($"HASLIST[{sec},{key}] -> {Lists.ContainsKey(sec)}"); // debug only
			//foreach(var dbg in Lists) { Console.WriteLine($"{dbg.Key} -> {dbg.Value.Count} items"); } // debug only
			if (!Lists.ContainsKey(sec)) return false;
			return Lists[sec].ContainsKey(key);
		}

		public void ListAdd(string sec,string key,string value,bool sort = true) {
			List(sec, key).Add( value);
			if (sort) List(sec, key).Sort();
			if (AutoSaveSource != "") SaveSource(AutoSaveSource);
		}

		public void ListRemove(string sec, string key, string value, bool keeptrying = true) {
			var L = List(sec, key);
			if (!L.Contains(value)) return;
			do {
				L.Remove(value);
			} while (keeptrying && L.Contains(value));
			if (AutoSaveSource != "") SaveSource(AutoSaveSource);
		}

		public void ListAddNew(string sec,string key,string value,bool sort=true) {
			if (!List(sec, key).Contains(value)) ListAdd(sec, key,value,sort);
		}

		public SortedDictionary<string, SortedDictionary<string, string>>.KeyCollection EachSections => Values.Keys;
		public bool HasSection(string sec) => Values.ContainsKey(sec.ToUpper()) || Lists.ContainsKey(sec.ToUpper());

		public SortedDictionary<string, string>.KeyCollection Each(string sec) {
			sec = sec.ToUpper();
			if (!Values.ContainsKey(sec)) return null;            
			return Values[sec].Keys;
		}

		

		public string this[string sec, string key] {
			get {
				sec = sec.ToUpper();
				key = key.ToUpper();
				if (!Values.ContainsKey(sec)) return "";
				if (!Values[sec].ContainsKey(key)) return "";
				return Values[sec][key];
			}
			set {
				sec = sec.ToUpper();
				key = key.ToUpper();
				if (!Values.ContainsKey(sec)) Values[sec] = new SortedDictionary<string, string>();
				Values[sec][key] = value;
				if (AutoSaveSource != "") {
					SaveSource(AutoSaveSource);
				}
			}
		}
		/*
		System.Collections.Generic.List<string> List(string sec, string key) {
			sec = sec.ToUpper();
			key = key.ToUpper();
			if (!Lists.ContainsKey(sec)) Lists[sec] = new SortedDictionary<string, System.Collections.Generic.List<string>>();
			if (!Lists[sec].ContainsKey(key)) Lists[sec][ key] = new System.Collections.Generic.List<string>();
			return Lists[sec][key];
		}
		*/

		public string ToSource() {
			var Done = new List<string>();
			var ret = new StringBuilder();
			foreach (string k in Values.Keys) {
				ret.Append($"[{k}]\n");
				Done.Add(k);
				foreach (string key in Values[k].Keys) ret.Append($"{qstr.SafeString(key)}={qstr.SafeString(Values[k][key])}\n");
				if (Lists.ContainsKey(k)) {
					foreach (string key in Lists[k].Keys) {
						ret.Append($"*list:{qstr.SafeString(key)}\n");
						foreach (string item in Lists[k][key]) ret.Append($"\t{qstr.SafeString(item)}\n");
						ret.Append($"*end\n");
					}
				}
			}
			foreach (string k in Lists.Keys) {
				if (!Done.Contains(k)) {
					ret.Append($"[{k}]\n");
					foreach (string key in Lists[k].Keys) {
						ret.Append($"*list:{qstr.SafeString(key)}\n");
						foreach (string item in Lists[k][key]) {
							if (item.Length > 0 && item[0] == '#')
								ret.Append($"\t\\{qstr.SafeString(item)}\n");
							else
								ret.Append($"\t{qstr.SafeString(item)}\n");
						}
						ret.Append($"*end\n");
					}
				}
			}
			return ret.ToString();
		}

		/// <summary>
		/// Clears all data inside the GINIE object
		/// </summary>
		public void Clear() {
			Values.Clear();
			Lists.Clear();
		}

		/// <summary>
		/// Reading GINIE data from byte code
		/// </summary>
		/// <param name="buf">Byte code</param>
		/// <param name="merge">If set to true it merges from existing Data</param>
		public void FromBytes(byte[] buf,bool merge=false) {
			const string Head = "GENIE\x1b";
			var bt = new QuickStream(new MemoryStream(buf));
			if (buf.Length < 7) throw new GINIE_ByteNotGINIE();
			if (!merge) Clear();
			for (byte i = 0; i < Head.Length; ++i) {
				var b = bt.ReadByte();
#if DEBUG
				Debug.WriteLine($"GINIE HEADER {i}: {b}.{(byte)Head[i]}({(char)b}/{Head[i]})");
#endif

				if (b != Head[i]) throw new GINIE_ByteNotGINIE();
			}
			string cat = "";
			while (!bt.EOF) {
				var wtag = bt.ReadByte();
				switch (wtag) {
					case 1:
						cat = bt.ReadString64().ToUpper();
						break;
					case 2:
						this[cat, bt.ReadString64().ToUpper()] = bt.ReadString64();
						break;
					case 3: {
							var len = bt.ReadULong();
							var key = bt.ReadString64().ToUpper();
							if (!Lists.ContainsKey(cat)) Lists[cat] = new SortedDictionary<string, List<string>>();
							if (!Lists[cat].ContainsKey(key)) Lists[cat][key] = new List<string>();
							for (ulong i = 0; i < len; i++) Lists[cat][key].Add(bt.ReadString());
						}
						break;
					case 0:
					case 255:
						bt.Close();
						return;
					default:
						throw new GINIE_ByteUnkTag(wtag);                        
				}
			}
		}

		/// <summary>
		/// Converts GINIE code into byte code
		/// </summary>
		/// <returns>The buffer containing the byte code</returns>
		public byte[] ToBytes() {
			byte[] head = new byte[] { (byte)'G', (byte)'E', (byte)'N', (byte)'I', (byte)'E', (byte)'\x1b' };
			byte[] buf;
			var ms = new MemoryStream();
			var bt = new QuickStream(ms);
			foreach (var h in head) bt.Write(h);
			bt.Position = bt.Size;
			var Done = new Dictionary<string, bool>();
			foreach (var cat in Values) {
				bt.WriteByte(1);
				bt.WriteString64(cat.Key);
				foreach (var v in cat.Value) {
					bt.WriteByte(2);
					bt.WriteString64(v.Key);
					bt.WriteString64(v.Value);
				}
				if (Lists.ContainsKey(cat.Key)) {
					foreach (var v in Lists[cat.Key]) {
						bt.WriteByte(3);
						bt.Write((ulong)Lists[cat.Key][v.Key].Count);
						foreach (var e in Lists[cat.Key][v.Key]) bt.WriteString64(e);
						Done[$"{cat.Key}:{v.Key}"] = true;
					}					
				}
			}
			foreach (var cat in Lists) {
				foreach (var v in Lists[cat.Key]) {
					if ((!Done.ContainsKey($"{cat.Key}:{v.Key}")) || (!Done[$"{cat.Key}:{v.Key}"])) {
						bt.WriteByte(3);
						bt.Write((ulong)Lists[cat.Key][v.Key].Count);
						foreach (var e in Lists[cat.Key][v.Key]) bt.WriteString64(e);
					}
				}
			}
			buf = ms.ToArray();
			bt.Close();
			return buf;
		}


		public void SaveSource(string file) {
			Directory.CreateDirectory(qstr.ExtractDir(file));
			QuickStream.SaveString(file, ToSource());
		}
		
	}
}