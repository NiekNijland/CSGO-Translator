using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsgoTranslator
{
    public static class CommandsController
    {
        /// <summary>
        /// Helper function for LoadLogs
        /// function checks if given message contains a valid command and will return the correct command or null.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static Command BuildCommand(Log previousLog, string rawString, ChatType chatType, string name, string rawMessage)
        {
            string message = null;
            string lang = null;
            TransType transType;


            #region command validation

            if (rawMessage.StartsWith("!all"))
            {
                message = rawMessage.Substring(4).Trim();
                transType = TransType.TransAllCommand;
            }
            else if (rawMessage.StartsWith("!team"))
            {
                message = rawMessage.Substring(5).Trim();
                transType = TransType.TransTeamCommand;
            }
            else
            {
                return null;
            }

            #endregion

            #region language param checking

            //checking if there is a language param.
            string[] temp = message.Split(new char[] { ' ' }, 2);
            string possLang = temp[0].Trim();

            if (possLang[0] == '-' && possLang.Length == 3)
            {
                lang = possLang.Substring(1);
                message = temp[1].Trim();
            }

            #endregion

            Console.WriteLine("------------");
            Console.WriteLine($"build command: {chatType} {name} {possLang} {message}");
            Console.WriteLine("------------");

            if (message.Length > 0 && transType != TransType.Undefined)
            {
                return new TransCommand(previousLog, rawString, transType, chatType, name, message, lang);
            }
            else
            {
                return null;
            }
        }
    }
}
