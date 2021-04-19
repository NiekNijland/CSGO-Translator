using CsgoTranslator.Enums;

namespace CsgoTranslator.Helpers
{
    internal static class OptionsManager
    {
        public static string InstallationPath
        {
            get => Properties.Settings.Default.Path;
            set => Properties.Settings.Default.Path = value;
        }
        public static string Language
        {
            get => Properties.Settings.Default.Lang;
            set => Properties.Settings.Default.Lang = value;
        }
        public static string OwnUsername
        {
            get => Properties.Settings.Default.OwnUsername;
            set => Properties.Settings.Default.OwnUsername = value;
        }
        public static bool IgnoreOwnMessages
        {
            get => Properties.Settings.Default.IgnoreOwnMessages;
            set => Properties.Settings.Default.IgnoreOwnMessages = value;
        }
        public static int TelnetPort
        {
            get => Properties.Settings.Default.TelnetPort;
            set => Properties.Settings.Default.TelnetPort = value;
        }
        public static TelnetGrant SendTranslationsTo
        {
            get => Properties.Settings.Default.SendTranslationsTo;
            set => Properties.Settings.Default.SendTranslationsTo = value;
        } 
        public static TelnetGrant SendTranslationsFrom
        {
            get => Properties.Settings.Default.SendTranslationsFrom;
            set => Properties.Settings.Default.SendTranslationsFrom = value;
        } 
        public static TelnetGrant AllowCommandsFrom 
        {
            get => Properties.Settings.Default.AllowCommandsFrom;
            set => Properties.Settings.Default.AllowCommandsFrom = value;
        } 

        public static void ValidateSettings()
        {
            if (InstallationPath.Length == 0 || Language.Length == 0 || TelnetPort == 0)
                SetDefault();
        }

        public static void SetDefault()
        {
            Language = "en";
            InstallationPath = @"C:\Program Files (x86)\Steam\steamapps\common\Counter-Strike Global Offensive";
            OwnUsername = "";
            IgnoreOwnMessages = false;
            TelnetPort = 2121;
            SendTranslationsTo = TelnetGrant.TeamChat;
            SendTranslationsFrom = TelnetGrant.AllChat;
            AllowCommandsFrom = TelnetGrant.TeamChat; 
            
            Save();
        }

        public static void Save()
        {
            Properties.Settings.Default.Save();
        }
    }
}
