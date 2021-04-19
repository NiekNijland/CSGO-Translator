using System;
using CsgoTranslator.Enums;
using CsgoTranslator.Models;

namespace CsgoTranslator.Controllers
{
    public static class CommandsController
    {
        /// <summary>
        /// Helper function for LoadLogs
        /// function checks if given message contains a valid command and will return the correct command or null.
        /// </summary>
        public static Command BuildCommand(string rawString, ChatType chatType, string name, string rawMessage)
        {
            string message;
            ChatType exportChatType;

            #region command validation

            if (rawMessage.StartsWith("!all"))
            {
                message = rawMessage.Substring(4).Trim();
                exportChatType = ChatType.All;
            }
            else if (rawMessage.StartsWith("!team"))
            {
                message = rawMessage.Substring(5).Trim();
                exportChatType = ChatType.Team;
            }
            else
            {
                return null;
            }

            #endregion

            #region language param checking

            //checking if there is a language param.
            var temp = message.Split(new char[] { ' ' }, 2);
            var possLang = temp[0].Trim();

            if (possLang[0] != '-' || possLang.Length != 3)
                return message.Length > 0
                    ? new TransCommand(rawString, exportChatType, chatType, name, message)
                    : null;
            
            var lang = possLang.Substring(1);
            message = temp[1].Trim();

            #endregion

            return message.Length > 0 ? new TransCommand(rawString, exportChatType, chatType, name, message, lang) : null;
        }
    }
}
