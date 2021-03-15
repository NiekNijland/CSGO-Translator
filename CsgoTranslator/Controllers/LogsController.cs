﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CsgoTranslator
{
    static class LogsController
    {
        static public List<Chat> Chats { get; set; }
        static public List<Command> Commands { get; set; }

        /// <summary>
        /// Loads the amount rows from the console.log file. stores the output in Chats & Commands
        /// </summary>
        /// <param name="amount"></param>
        static public void LoadLogs(int amount)
        {
            List<string> lines = GetLastLines(amount);

            if (lines != null && lines.Count() != 0)
            {
                var (chatTypes, names, messages) = LineCleaner(lines);

                for (int i = 0; i < names.Count(); i++)
                {
                    if (messages[i].StartsWith("!"))
                    {
                        var possCommand = MakeCommand(chatTypes[i], names[i], messages[i]);

                        if(possCommand != null)
                        {
                            Commands.Add(possCommand);
                        }
                    }
                    else if(Chats.Where(x => x.Message == messages[i] && x.Name == names[i]).Count() == 0)
                    {
                        Chat chat = new Chat(chatTypes[i], names[i], messages[i]);
                        Chats.Add(chat);
                        if(chat.ChatType == ChatType.All)
                        {
                            TelnetHelper.SendTranslationInTeamChat(chat);
                        }
                        Console.WriteLine($"New chat message: {messages[i]}");
                    }
                }
            }
        }

        /// <summary>
        /// Helper function for LoadLogs
        /// Gets the latest amount rows from the console.log file.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns>list with string of each line</returns>
        private static List<string> GetLastLines(int amount)
        {
            //check if file exists, if not return null so an error can be displayed
            if (File.Exists($@"{Properties.Settings.Default.Path}\csgo\console.log"))
            {
                int count = 0;
                byte[] buffer = new byte[1];
                List<string> consoleLines = new List<string>();

                using (FileStream fs = new FileStream($@"{Properties.Settings.Default.Path}\csgo\console.log", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    fs.Seek(0, SeekOrigin.End);

                    while (count < amount)
                    {
                        fs.Seek(-1, SeekOrigin.Current);
                        fs.Read(buffer, 0, 1);
                        if (buffer[0] == '\n')
                        {
                            count++;
                        }

                        fs.Seek(-1, SeekOrigin.Current); // fs.Read(...) advances the position, so we need to go back again
                    }
                    fs.Seek(1, SeekOrigin.Current); // go past the last '\n'

                    string line;
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        while ((line = sr.ReadLine()) != null)
                            consoleLines.Add(line);
                    }
                }
                return consoleLines;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Helper function for LoadLogs
        /// function that checks all console log lines for chat messages and cleans them up. then returns 3 lists (ChatTypes, names & messages)
        /// </summary>
        /// <param name="lines"></param>
        /// <returns>tuple with chattypes names and messages for discovered chatlogs</returns>
        private static (List<ChatType> chatTypes, List<string> names, List<string> messages) LineCleaner(List<string> lines)
        {
            List<ChatType> returnChatTypes = new List<ChatType>();
            List<string> returnNames = new List<string>();
            List<string> returnMessages = new List<string>();

            foreach (string l in lines)
            {
                //filter the lines on chat message syntax
                if (l.Contains(" : ") && !l.Contains("  : ") && !l.Contains("!."))
                {
                    string[] temp = l.Split(new string[] { " : " }, 2, StringSplitOptions.None);

                    ChatType chatType = ChatType.All;
                    string namePart = temp[0].Trim();
                    string messagePart = temp[1].Trim();

                    //removal of *DEAD* chat prefix.
                    if (namePart.StartsWith("*DEAD*"))
                    {
                        namePart = namePart.Substring(6).Trim();
                    }

                    //removal of teamchat prefix.
                    if (namePart.StartsWith("(Counter-Terrorist)"))
                    {
                        namePart = namePart.Substring(19).Trim();
                        chatType = ChatType.Team;
                    }

                    if (namePart.StartsWith("(Terrorist)"))
                    {
                        namePart = namePart.Substring(11).Trim();
                        chatType = ChatType.Team;
                    }

                    //removal of the teamchat location info.
                    if(chatType == ChatType.Team)
                    {
                        int idx = namePart.LastIndexOf('@');

                        if (idx != -1)
                        {
                            namePart = namePart.Substring(0, idx).Trim();
                        }
                    }

                    returnChatTypes.Add(chatType);
                    returnNames.Add(namePart);
                    returnMessages.Add(messagePart);
                }
            }

            return (chatTypes: returnChatTypes, names: returnNames, messages: returnMessages);
        }

        /// <summary>
        /// Helper function for LoadLogs
        /// function checks if given message contains a valid command and will return the correct command or null.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        private static Command MakeCommand(ChatType chatType, string name, string rawMessage)
        {
            string message = null;
            string lang = null;

            #region command validation

            if (rawMessage.StartsWith("!trans_all"))
            {
                message = rawMessage.Substring(10).Trim();
            }
            else if (rawMessage.StartsWith("!trans_team"))
            {
                message = rawMessage.Substring(11).Trim();
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

            if (Commands.Where(x => x.Message == message && x.Name == name).Count() == 0)
            {
                Console.WriteLine($"imported command: {message}");
                Console.WriteLine($"All commands: {Commands.Count}");

                if (message.Length > 0)
                {
                    if (rawMessage.StartsWith("!trans_all"))
                    {
                        if (lang != null)
                        {
                            return new TransAllCommand(chatType, name, message, lang);
                        }
                        else
                        {
                            return new TransAllCommand(chatType, name, message);
                        }
                    }
                    else if (rawMessage.StartsWith("!trans_team"))
                    {
                        if (lang != null)
                        {
                            return new TransTeamCommand(chatType, name, message, lang);
                        }
                        else
                        {
                            return new TransTeamCommand(chatType, name, message);
                        }
                    }
                }
            }
            return null;
        }
    }
}