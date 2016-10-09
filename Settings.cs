using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Runtime.InteropServices;

namespace qt1
{
    public class Settings
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern int GetPrivateProfileString(
            string lpApplicationName,
            string lpKeyName,
            string lpDefault,
            StringBuilder lpReturnedstring,
            int nSize,
            string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern int WritePrivateProfileString(
            string lpApplicationName,
            string lpKeyName,
            string lpstring,
            string lpFileName);

        string filePath;
        string sectionName = "Settings";

        public Settings()
        {
            filePath = AppDomain.CurrentDomain.BaseDirectory + @"qt1.ini";

            if (!System.IO.File.Exists(filePath))
            {
                this[sectionName, "exceptWords"] = string.Empty;
                this[sectionName, "leftClick"] = "Terminate";
                this[sectionName, "rightClick"] = "Explorer";
                this[sectionName, "middleClick"] = "Reboot";
            }
        }

        private string this[string section, string key, string def = ""]
        {
            set
            {
                WritePrivateProfileString(section, key, value, filePath);
            }
            get
            {
                StringBuilder sb = new StringBuilder(256);
                GetPrivateProfileString(section, key, def, sb, sb.Capacity, filePath);
                return sb.ToString();
            }
        }

        public string[] exceptWords
        {
            get
            {
                return this[sectionName, "exceptWords"].Split(',');
            }
        }

        public string leftClick
        {
            get
            {
                return this[sectionName, "leftClick"];
            }
        }

        public string rightClick
        {
            get
            {
                return this[sectionName, "rightClick"];
            }
        }

        public string middleClick
        {
            get
            {
                return this[sectionName, "middleClick"];
            }
        }
    }
}
