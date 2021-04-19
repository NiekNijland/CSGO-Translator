using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsgoTranslator.Enums;
using CsgoTranslator.Helpers;
using CsgoTranslator.Models;

namespace CsgoTranslator.Controllers
{
    internal static class LogsController
    {
        public static LinkedList<Log> Logs { get; set; }
        public static List<Chat> Chats { get; set; }
        public static List<Command> Commands { get; set; }

        /// <summary>
        /// Loads the amount rows from the console.log file. stores the output in Chats & Commands
        /// </summary>
        /// <param name="amount"></param>
        public static void LoadLogs(int amount)
        {
            var logs = new LinkedList<Log>();

            var lines = GetLastLines(amount);

            if (lines != null && lines.Count() != 0)
            {
                var (rawStrings, chatTypes, names, rawMessage) = LineCleaner(lines);

                for (var i = 0; i < rawStrings.Count(); i++)
                {
                    if (rawMessage[i][0] == '!')
                    {
                        var possCommand = CommandsController.BuildCommand(rawStrings[i], chatTypes[i], names[i], rawMessage[i]);
                        if(possCommand != null) logs.AddLast(possCommand);
                    }
                    else
                    {
                        logs.AddLast(new Chat(rawStrings[i], chatTypes[i], names[i], rawMessage[i]));
                    }
                }

                var addList = new List<Log>();

                //if logs where found in the file.
                if(logs.Last != null)
                {
                    /*
                        loop backwards over the array with the discovered logs.
                        Check for each node if the node is identical to the last added node in the system.
                        When the last added node was found, add all nodes that were looped over allready because they are new.
                    */
                    for (var node = logs.Last; node != null; node = node.Previous)
                    {
                        if(Logs.Last == null)
                        {
                            addList.Insert(0, node.Value);
                        }
                        else if (!Compare(Logs.Last, node))
                        {
                            addList.Insert(0, node.Value);
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                foreach(var log in addList)
                {
                    SaveLog(log);
                }
            }

            static void SaveLog(Log log)
            {
                Logs.AddLast(log);
                switch (log)
                {
                    case Chat chat:
                        Chats.Insert(0, chat);
                        TelnetHelper.SendChatTranslation(ChatType.Team, chat);
                        break;
                    case Command command:
                        Commands.Insert(0, command);
                        break;
                }
            }

            static bool Compare(LinkedListNode<Log> lastAddedNode, LinkedListNode<Log> newNode)
            {
                var lastAddedNodeRelative = lastAddedNode;
                var newNodeRelative = newNode;

                if (lastAddedNodeRelative.Value.RawString != newNodeRelative.Value.RawString) return false;
                
                for (var i = 0; i < 3; i++)
                {
                    lastAddedNodeRelative = lastAddedNodeRelative.Previous;
                    newNodeRelative = newNodeRelative.Previous;

                    if (lastAddedNodeRelative == null || newNodeRelative == null) return true;
                    if (lastAddedNodeRelative.Value.RawString != newNodeRelative.Value.RawString) return false;
                }
                
                return true;
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
            if (!File.Exists($@"{Properties.Settings.Default.Path}\csgo\console.log")) return null;
            
            var count = 0;
            var buffer = new byte[1];
            var consoleLines = new List<string>();

            using var fs = new FileStream($@"{Properties.Settings.Default.Path}\csgo\console.log", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            fs.Seek(0, SeekOrigin.End);

            while (count < amount)
            {
                fs.Seek(-1, SeekOrigin.Current);
                fs.Read(buffer, 0, 1);
                if (buffer[0] == '\n') count++;

                fs.Seek(-1, SeekOrigin.Current); // fs.Read(...) advances the position, so we need to go back again
            }
            fs.Seek(1, SeekOrigin.Current); // go past the last '\n'

            using var sr = new StreamReader(fs);
            string line;
            while ((line = sr.ReadLine()) != null)
                consoleLines.Add(line);

            return consoleLines;
        }

        /// <summary>
        /// Helper function for LoadLogs
        /// function that checks all console log lines for chat messages and cleans them up. then returns 3 lists (ChatTypes, names & messages)
        /// </summary>
        /// <param name="lines"></param>
        /// <returns>tuple with chat-types names and messages for discovered chatlogs</returns>
        private static (List<string> rawStrings, List<ChatType> chatTypes, List<string> names, List<string> messages) LineCleaner(IEnumerable<string> lines)
        {
            var returnRawStrings = new List<string>();
            var returnChatTypes = new List<ChatType>();
            var returnNames = new List<string>();
            var returnMessages = new List<string>();

            foreach (var l in lines)
            {
                //filter the lines on chat message syntax
                if (!l.Contains(" : ") || l.Contains("  : ") || l.Contains("!.") ||
                    l.Trim().StartsWith("Duplicate :          ")) continue;
                var temp = l.Split(new string[] { " : " }, 2, StringSplitOptions.None);

                var chatType = ChatType.All;
                var namePart = temp[0].Trim();
                var messagePart = temp[1].Trim();

                //removal of *DEAD* chat prefix.
                if (namePart.StartsWith("*DEAD*"))
                    namePart = namePart.Substring(6).Trim();

                //removal of team-chat prefix.
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

                //removal of the team-chat location info.
                if(chatType == ChatType.Team)
                {
                    var idx = namePart.LastIndexOf('@');

                    if (idx != -1)
                        namePart = namePart.Substring(0, idx).Trim();
                }

                returnRawStrings.Add(l);
                returnChatTypes.Add(chatType);
                returnNames.Add(namePart);
                returnMessages.Add(messagePart);
            }

            return (rawStrings: returnRawStrings, chatTypes: returnChatTypes, names: returnNames, messages: returnMessages);
        }
    }
}
