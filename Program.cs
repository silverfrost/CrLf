using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crlf
{
    enum Ending
    {
        None,
        WindowsCrLf,
        MacLf,
        LinuxCr
    }

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("crlf file [/w] [/m] [/l] [/o:file1]");
                return;
            }
            string infile = args[0];
            if (!File.Exists(infile))
            {
                Console.WriteLine($"Could not open {infile}");
                return;
            }
            // read the file and see if the line ending can be determined
            string txt = File.ReadAllText(infile);
            Ending e = Ending.None;
            for (int i = 0; i < 1000 && i<txt.Length; i++)
            {
                char ch = txt[i];
                if (ch < ' ')
                {
                    if (ch == '\r')
                    {
                        e = Ending.LinuxCr;
                        if (i < txt.Length + 1 && txt[i + 1] == '\n')
                            e = Ending.WindowsCrLf;
                        break;
                    }
                    if (ch == '\n')
                    {
                        e = Ending.MacLf;
                        break;
                    }
                }
            }
            if (e == Ending.None)
            {
                Console.WriteLine("Line ending could not be determined");
                return;
            }
            if (args.Length == 1)
            {
                Console.WriteLine($"File has a line ending of {EndingTxt(e)}");
                return;
            }
            string destfile = infile;
            Ending destend = Ending.None;
            for (int i = 1; i < args.Length; i++)
            {
                string sw = args[i];
                if (sw[0] != '/' || sw.Length < 2)
                {
                    Console.WriteLine($"Unknown switch: {sw}");
                    break;
                }
                switch (sw.Substring(0, Math.Min(sw.Length, 3)))
                {
                    case "/w":
                    case "/pc":
                        destend = Ending.WindowsCrLf;
                        break;
                    case "/m":
                    case "/lf":
                    case "/n":
                    case "/10":
                        destend = Ending.MacLf;
                        break;
                    case "/l":
                    case "/cr":
                    case "/r":
                    case "/13":
                        destend = Ending.LinuxCr;
                        break;

                    case "/o:":
                        destfile = sw.Substring(3);
                        break;
                    default:
                        Console.WriteLine("Unknown switch " + sw);
                        return;
                }
            }
            if (destend == e)
            {
                Console.WriteLine($"Line endings are already {EndingTxt(e)}, nothing done");
                return;
            }
            if (e == Ending.WindowsCrLf)
            {
                if (destend == Ending.MacLf)
                    txt = txt.Replace("\r", "");
                else if (destend == Ending.LinuxCr)
                    txt = txt.Replace("\n", "");
            }
            else if ( e == Ending.MacLf)
            {
                if (destend == Ending.WindowsCrLf)
                    txt = txt.Replace("\n", "\r\n");
                else if (destend == Ending.LinuxCr)
                    txt = txt.Replace("\n", "\r");
            }
            else if (e == Ending.LinuxCr)
            {
                if (destend == Ending.WindowsCrLf)
                    txt = txt.Replace("\r", "\r\n");
                else if (destend == Ending.MacLf)
                    txt = txt.Replace("\r", "\n");
            }
            File.WriteAllText(destfile, txt);
            Console.WriteLine($"{destfile} written with {EndingTxt(destend)}");
        }

        private static string EndingTxt(Ending E)
        {
            switch (E)
            {
                case Ending.None:
                    break;
                case Ending.WindowsCrLf:
                    return "\\r\\n, 13+10, Cr+Lf, Windows";
                case Ending.MacLf:
                    return "\\n, 10, Lf, Mac";
                case Ending.LinuxCr:
                    return "\\r, 13, Cr, Linux";
            }
            return "Unknown";
        }
    }
}
