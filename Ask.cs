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
		
	}

}
