using CsgoTranslator.Enums;
using CsgoTranslator.MinimalisticTelnet;
using CsgoTranslator.Models;

namespace CsgoTranslator.Helpers
{
    internal static class TelnetHelper
    {
        private static TelnetConnection _telnetConnection;

        public static bool Connected => (_telnetConnection != null && _telnetConnection.Connected);

        public static bool Connect()
        {
            _telnetConnection = new TelnetConnection();
            return _telnetConnection.Connected;
        }

        private static bool ExecuteCsgoCommand(string command)
        {
            if (!Connected) return false;

            _telnetConnection.WriteLine(command);
            
            return true;
        }

        public static bool SendInChat(ChatType chatType, string message)
        {
            return chatType switch
            {
                ChatType.All => ExecuteCsgoCommand($"say_team !. {message}"),
                ChatType.Team => ExecuteCsgoCommand($"say !. {message}"),
                _ => false,
            };
        }

        public static bool SendChatTranslation(ChatType chatType, Chat chat)
        {
            return SendInChat(chatType, $"{chat.Name} : {chat.Translation}");
        }
    }
}
