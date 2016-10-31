using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qt1
{
    public class CommandLineArgs
    {
        public bool noIcons { get; }    // /noicon
        public bool noPathName { get; } // /nopathname
        public int top { get; }         // /top
        public int left { get; }        // /left
        public string exePath { get; }  // exeのパス

        public CommandLineArgs()
        {
            noIcons = false;
            noPathName = false;
            top = -1;
            left = -1;
            exePath = string.Empty;

            string[] args = System.Environment.GetCommandLineArgs();
            args[0] = ""; //最初の要素は必ず自分自身のパスなので削除
            foreach(string arg in args)
            {
                if (arg.ToLower() == "/noicon")
                    noIcons = true;
                if (arg.ToLower() == "/nopathname")
                    noPathName = true;
                if (arg.ToLower().Contains("/top="))
                {
                    int value = -1;
                    if (int.TryParse(arg.Split('=')[1], out value))
                        top = value;                       
                }
                if (arg.ToLower().Contains("/left="))
                {
                    int value = -1;
                    if (int.TryParse(arg.Split('=')[1], out value))
                        left = value;
                }
                if (arg.Contains(".exe"))
                    exePath = arg;
            }
        }


    }
}
