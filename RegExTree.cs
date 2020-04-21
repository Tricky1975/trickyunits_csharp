/* RegExTree
 * Please not when using this class the next classes MUST be present!
 * - Dirry
 * - FileList
 * - MKL
 */ 

using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using System;

namespace TrickyUnits {

    static class RegExTree {

        public enum Type { FilesOnly, DirsOnly, All }
        public enum CheckCase { Auto, Yes, No }

        static List<string> GetMe(string Expression, Type t, bool hidden) {
            var ret = new List<string>();
            return ret;
        }

        static int gt(Type t) {
            switch (t) {
                case Type.FilesOnly: return 0;
                case Type.DirsOnly: return 2;
                case Type.All: return 1;
                default:
                    throw new Exception($"Invalid type {t}");
            }
        }

        //static StringBuilder StaticLevelBuilder = new StringBuilder(); // I chose this way in order not to have to allocate and release too much memory.... Slows things down, needlessly!
        static public List<string> Tree(string Expression, CheckCase CC = CheckCase.Auto, Type t = Type.FilesOnly, bool hidden = false) {
            var ret = new List<string>();
            var Ex = Dirry.AD(Expression);
            var ExA = Ex.Split('/');
            //var start = 0;
            var root = false;
            var drive = false;
            /* Maybe this wasn't the right approach... It's always a puzzle :)
            string level(int l) {
                StaticLevelBuilder.Clear();
                for (int i = 0; i <= l && i < ExA.Length) {
                    if (i > 0) StaticLevelBuilder.Append('/');
                    StaticLevelBuilder.Append(ExA[i]);
                }
                return StaticLevelBuilder.ToString();
            } */
            void ArrayPrefix(string[] d,string Prefix) {
                for (int i = 0; i < d.Length; i++) d[i] = $"{Prefix}{d[i]}";
            }
            try {
                string[] VL=null;
                if (ExA.Length == 0) return ret; // No need to do anything!
                if (ExA[0].Length == 2 && ExA[0][1] == ':') { ret.Add(ExA[0]); drive = true; /*start = 1;*/ VL = FileList.GetTree($"{ExA[0]}/",true,hidden); }
                else if (ExA[0].Length == 0) { root = true; /*start = 1;*/ VL = FileList.GetTree("/",true,hidden); }
                else { //if ((!root) && (!drive)) {                    
                    Debug.WriteLine($"No root or drive, so let's get 'em ... Reading tree {Directory.GetCurrentDirectory()}");
                    VL = FileList.GetTree(Directory.GetCurrentDirectory(), true, hidden);
                    Debug.WriteLine($"{VL.Length} entries found");
                }
                foreach(string entry in VL) {
                    //Debug.WriteLine($"Matching: {entry} / {Expression}");
                    if (Regex.IsMatch(entry,$"^{Expression}$"))
#if DEBUG
                        {
                        Debug.WriteLine($"  MATCH FOR: {entry} / {Expression}");
#endif
                        ret.Add(entry);
#if DEBUG
                    } else {
                        Debug.WriteLine($"NO MATCH FOR: {entry} / {Expression}");
                    }
#endif
                }
            } catch (Exception e) {
                Console.Beep(); Console.Beep(); Console.Beep();
                Debug.WriteLine($"RegExTree(\"{Expression}\",CheckCase.{CC},Type.{t},{hidden}): Exception thrown {e.Message}");
            }

            return ret;
        }

        static public string[] TreeArray(string Expression, CheckCase CC = CheckCase.Auto, Type t = Type.FilesOnly, bool hidden = false) => Tree(Expression, CC, t, hidden).ToArray();

    }

}