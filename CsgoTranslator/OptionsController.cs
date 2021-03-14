using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsgoTranslator
{
    static class OptionsController
    {
        static public void CheckIfSet()
        {
            if(Properties.Settings.Default.Path.Length == 0)
            {
                Properties.Settings.Default.Path = @"C:\Program Files (x86)\Steam\steamapps\common\Counter-Strike Global Offensive";
                Properties.Settings.Default.Save();
            }
            if (Properties.Settings.Default.Lang.Length == 0)
            {
                Properties.Settings.Default.Lang = "en";
                Properties.Settings.Default.Save();
            }
        }

        static public void SaveDefault()
        {
            SaveLang("en");
            SavePath(@"C:\Program Files (x86)\Steam\steamapps\common\Counter-Strike Global Offensive");
        }
        static public void SavePath(string path)
        {
            Properties.Settings.Default.Path = path;
            Properties.Settings.Default.Save();
        }

        static public string GetPath()
        {
            return Properties.Settings.Default.Path;
        }

        static public void SaveLang(string lang)
        {
            Properties.Settings.Default.Lang = lang;
            Properties.Settings.Default.Save();
        }

        static public string GetLang()
        {
            return Properties.Settings.Default.Lang;
        }
    }
}
