using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace qt1
{
    public class Settings
    {
        public string[] exceptWords
        {
            get
            {
                string _exceptWords = ConfigurationManager.AppSettings["exceptWords"];
                if (_exceptWords == null)
                    return new string[1] { "" };
                else
                    return _exceptWords.Split(',');
            }
        }

        public string leftClick
        {
            get
            {
                string _leftClick = ConfigurationManager.AppSettings["leftClick"];
                return _leftClick == null ? "Terminate" : _leftClick;
            }
        }

        public string rightClick
        {
            get
            {
                string _rightClick = ConfigurationManager.AppSettings["rightClick"];
                return _rightClick == null ? "Explorer.exe" : _rightClick;
            }
        }

        public string middleClick
        {
            get
            {
                string _middleClick = ConfigurationManager.AppSettings["middleClick"];
                return _middleClick == null ? "Explorer.exe" : _middleClick;
            }
        }
    }
}
