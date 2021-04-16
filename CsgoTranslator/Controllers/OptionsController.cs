namespace CsgoTranslator.Controllers
{
    internal static class OptionsController
    {
        public static void CheckIfSet()
        {
            if(Properties.Settings.Default.Path.Length == 0)
            {
                Properties.Settings.Default.Path = @"C:\Program Files (x86)\Steam\steamapps\common\Counter-Strike Global Offensive";
                Properties.Settings.Default.Save();
            }

            if (Properties.Settings.Default.Lang.Length != 0) return;
            
            Properties.Settings.Default.Lang = "en";
            Properties.Settings.Default.Save();
        }

        public static void SaveDefault()
        {
            SaveLang("en");
            SavePath(@"C:\Program Files (x86)\Steam\steamapps\common\Counter-Strike Global Offensive");
        }
        public static void SavePath(string path)
        {
            Properties.Settings.Default.Path = path;
            Properties.Settings.Default.Save();
        }

        public static string GetPath()
        {
            return Properties.Settings.Default.Path;
        }

        public static void SaveLang(string lang)
        {
            Properties.Settings.Default.Lang = lang;
            Properties.Settings.Default.Save();
        }

        public static string GetLang()
        {
            return Properties.Settings.Default.Lang;
        }
    }
}
