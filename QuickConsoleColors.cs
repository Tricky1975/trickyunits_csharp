namespace TrickyUnits{
    static class QCol {
        static QCol(){
            MKL.Version();
            MKL.Lic();            
        }

        /// <summary>Does nothing at all. But calling it forces C# to load this class making sure the version data is up-to-date!</summary>
        static public void Hello();

static              ConsoleColor obcl = Console.BackgroundColor;
      static  ConsoleColor ofcl = Console.ForegroundColor;

           public static void OriCol() { Console.ForegroundColor = ofcl;Console.BackgroundColor = obcl; }

        public static  void ColWrite(ConsoleColor c, string m) { Console.ForegroundColor = c; Console.Write(m); }

public static         void Red(string m) => ColWrite(ConsoleColor.Red, m);
public static         void Magenta(string m) => ColWrite(ConsoleColor.Magenta, m);
public static         void Yellow(string m) => ColWrite(ConsoleColor.Yellow, m);
public static         void Cyan(string m) => ColWrite(ConsoleColor.Cyan, m);
public static         void White(string m) => ColWrite(ConsoleColor.White, m);
public static         void Green(string m) => ColWrite(ConsoleColor.Green, m);


    }
}