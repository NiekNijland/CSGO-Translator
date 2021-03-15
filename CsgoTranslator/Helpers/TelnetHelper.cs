using System;
using MinimalisticTelnet;

namespace CsgoTranslator
{
    static class TelnetHelper
    {
        private static TelnetConnection _telnetConnection;

        public static bool Connected
        {
            get {
                return (_telnetConnection != null && _telnetConnection.Connected);
            }
        }

        public static bool Connect()
        {
            _telnetConnection = new TelnetConnection();
            return _telnetConnection.Connected;
        }

        public static bool ExecuteCsgoCommand(string command)
        {
            if (!Connected) return false;

            _telnetConnection.WriteLine(command);
            return true;
        }

        public static bool SendInTeamChat(string message)
        {
            return ExecuteCsgoCommand($"say_team !. {message}");
        }

        public static bool SendInAllChat(string message)
        {
            return ExecuteCsgoCommand($"say !. {message}");
        }

        public static bool SendTranslationInTeamChat(Chat chat)
        {
            return SendInTeamChat($"{chat.Name} : {chat.Translation}");
        }
    }
}
