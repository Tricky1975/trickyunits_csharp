// Lic:
// Ask.cs
// Ask
// version: 23.08.30
// Copyright (C) 2023 Jeroen P. Broks
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
using System.Runtime.Serialization;

namespace TrickyUnits{

	class TAsk {

		public readonly GINIE GINIEDATA;
		public TAsk(GINIE GD) {  GINIEDATA = GD;}

		public static string Ask(GINIE GD,string cat,string key,string question,string defaultvalue = "") {
			while (GD[cat, key] == "") {
				if (defaultvalue != "") QCol.Magenta($"[{defaultvalue}] ");
				QCol.Yellow(question);
				QCol.Cyan("");
				var rl = Console.ReadLine();
				if (rl == null) rl = "";
				GD[cat, key] = rl;
				if (GD[cat, key] == "") GD[cat, key] = defaultvalue;
			}
			return GD[cat, key];
		}

		public string Ask(string cat, string key, string question, string defaultvalue = "") => Ask(GINIEDATA, cat, key, question, defaultvalue);

		public static int AskInt(GINIE GD, string cat, string key, string question, int defaultvalue) => qstr.ToInt(Ask(GD, cat, key, question, defaultvalue.ToString()));
		public static int AskInt(GINIE GD, string cat, string key, string question) => qstr.ToInt(Ask(GD, cat, key, question));

		public int AskInt(string cat, string key, string question, int defaultvalue) => AskInt(GINIEDATA, cat, key, question, defaultvalue);
		public int AskInt(string cat, string key, string question) => AskInt(GINIEDATA, cat, key, question);

		static public List<string> NewList(GINIE GD,string cat, string key,string question) {
			if (GD.HasList(cat,key)) return GD.List(cat,key);
			var ret = GD.List(cat, key);
			if (ret.Count == 0) {
				QCol.Yellow($"{question}\n");				
				string? i;
				do {
					QCol.Yellow("> "); QCol.Cyan("");
					i = Console.ReadLine();
					if (i != null && i != "") {
						ret.Add(i);
					} else break;
				} while (true);
			}
			return ret;
		}

		public List<string> NewList(string cat,string key, string question) => NewList(GINIEDATA,cat,key,question);
		
		public bool Yes(string cat, string key, string question) {
			if (GINIEDATA[cat,key]=="") {
				QCol.Yellow(question);
				QCol.Cyan(" ? ");
				QCol.Magenta("<Y/N> ");
				Ignore:
				var a = Console.ReadKey(true);
				switch (a.Key) {
					case ConsoleKey.Y:
					case ConsoleKey.J:
						QCol.Green("Yes!\n");
						GINIEDATA[cat, key] = "YES";
						break;
					case ConsoleKey.N:
						QCol.Red("NO!\n");
						GINIEDATA[cat, key] = "NO";
						break;
					default:
						goto Ignore;
				}
			}
			return GINIEDATA[cat, key].ToUpper().Trim() == "YES";
		}
	}

}