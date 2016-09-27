﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qt1
{
    public class CommandLineArgs
    {
        public bool noIcons { get; }
        public bool noPathName { get; }
        public int top { get; }
        public int left { get; }

        public CommandLineArgs()
        {
            noIcons = false;
            noPathName = false;
            top = -1;
            left = -1;

            string[] args = System.Environment.GetCommandLineArgs();
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
            }
        }


    }
}
