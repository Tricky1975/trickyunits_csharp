using System.Collections.Generic;
using System;

namespace TrickyUnits
{
    class FlagParse
    {
        string[] basis;
        Dictionary<string, string> def_string = new Dictionary<string, string>();
        Dictionary<string, int> def_int = new Dictionary<string, int>();
        Dictionary<string, bool> def_bool = new Dictionary<string, bool>();
        Dictionary<string, string> val_string = new Dictionary<string, string>();
        Dictionary<string, int> val_int = new Dictionary<string, int>();
        Dictionary<string, bool> val_bool = new Dictionary<string, bool>();
        Dictionary<string, bool> hsd_string = new Dictionary<string, bool>();
        Dictionary<string, bool> hsd_bool = new Dictionary<string, bool>();
        Dictionary<string, bool> hsd_int = new Dictionary<string, bool>();
        List<string> StrayArgs = new List<string>();
        bool parsed = false;

        public FlagParse(string[] parameters)
        {
            MKL.Version("", "1.2.3");
            MKL.Lic("", "");
            basis = parameters;
        }

        /// <summary>
        /// Creates string readout flag. When not set up by startup, default value will be created.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultvalue"></param>
        public void CrString(string key, string defaultvalue)
        {
            def_string[key] = defaultvalue;
            if (!hsd_string.ContainsKey(key)) hsd_string[key] = true;
        }
        /// <summary>
        /// Creates string readout flag. User will be required to give a value.
        /// </summary>
        /// <param name="key"></param>
        public void CrString(string key)
        {
            hsd_string[key] = false;
            CrString(key, "");
        }

        /// <summary>
        /// Creates int readout flag. When not set up by startup, default value will be created.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultvalue"></param>
        public void CrInt(string key, int defaultvalue)
        {
            def_int[key] = defaultvalue;
            if (!hsd_int.ContainsKey(key)) hsd_int[key] = true;
        }
        /// <summary>
        /// Creates int readout flag. User will be required to give a value.
        /// </summary>
        /// <param name="key"></param>
        public void CrInt(string key)
        {
            hsd_int[key] = false;
            CrInt(key, 0);
        }

        /// <summary>
        /// Creates bool readout flag. When not set up by startup, default value will be created.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultvalue"></param>
        public void CrBool(string key, bool defaultvalue)
        {
            def_bool[key] = defaultvalue;
            if (!hsd_bool.ContainsKey(key)) hsd_bool[key] = true;
        }

        /// <summary>
        /// Parses everything out!
        /// </summary>
        /// <param name="chat">When set to true, error messages will be shown</param>
        /// <returns>'true' if succesful</returns>
        public bool Parse(bool chat=false)
        {
            if (parsed) {
                throw new Exception("Already parsed!");
            }
            bool expecting = false;
            string lastflag = "";
            byte lastype = 0; // 1 = string, 2 = int
            bool ret = true;
            // Default values
            foreach (string k in hsd_string.Keys) val_string[k] = def_string[k];
            foreach (string k in hsd_int.Keys) val_int[k] = def_int[k];
            foreach (string k in hsd_bool.Keys) val_bool[k] = def_bool[k];
            // Actual parsing
            foreach (string w in basis) {
                if (expecting) {
                    switch (lastype) {
                        case 1:
                            val_string[lastflag] = w;
                            break;
                        case 2:
                            try {
                                val_int[lastflag] = Int32.Parse(w);
                            } catch {
                                if (chat) Console.Write($"Invalid value on flag {lastflag}. I was expecting an integer number.");
                                return false;
                            }
                            break;
                        default:
                            throw new Exception($"TrickyUnits.FlagParse.Parse(): Internal error! lastype = {lastype}. I don't understand that. Please report!");
                    }
                    expecting = false;
                } else if (qstr.Prefixed(w,"-")) {
                    lastflag = qstr.RemPrefix(w, "-");
                    if (def_string.ContainsKey(lastflag) || def_int.ContainsKey(lastflag))
                        expecting = true;
                    else if (def_bool.ContainsKey(lastflag))
                        val_bool[lastflag] = !def_bool[lastflag];
                } else {
                    StrayArgs.Add(w);
                }
            }
            // Expected value not there, but the end has been reached?
            if (expecting) {
                if (chat) Console.WriteLine($"Flag {lastflag} was expecting a value");
                return false;
            }

            // Make sure all flags have a value
            foreach (string k in def_string.Keys) { if (!val_string.ContainsKey(k)) { if (chat) Console.WriteLine($"Flag {k} undefined!"); ret = false; } }
            foreach (string k in def_int.Keys) { if (!val_string.ContainsKey(k)) { if (chat) Console.WriteLine($"Flag {k} undefined!"); ret = false; } }

            // Return our success! (If it is a success)
            parsed = ret;
            return ret;
        }

        /// <summary>
        /// Returns string value of a flag
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetString(string key) {
            if (!parsed) throw new Exception("Flags not yet parsed!");
            if (!val_string.ContainsKey(key)) throw new Exception($"I have no string flag named {key}");
            return val_string[key];
        }

        /// <summary>
        /// Returns int value of a flag
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetInt(string key)
        {
            if (!parsed) throw new Exception("Flags not yet parsed!");
            if (!val_int.ContainsKey(key)) throw new Exception($"I have no int flag named {key}");
            return val_int[key];
        }

        /// <summary>
        /// Returns bool value of a flag
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool GetBool(string key)
        {
            if (!parsed) throw new Exception("Flags not yet parsed!");
            if (!val_bool.ContainsKey(key)) throw new Exception($"I have no bool flag named {key}");
            return val_bool[key];
        }

        public string[] Args {
            get
            {
                if (!parsed) throw new Exception("Arguments not yet parsed");
                return StrayArgs.ToArray();
            }
        }

    }
}
