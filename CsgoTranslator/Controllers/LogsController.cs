using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CsgoTranslator
{
    static class LogsController
    {
        static public LinkedList<Log> Logs { get; set; }
        static public List<Chat> Chats { get; set; }
        static public List<Command> Commands { get; set; }

        /// <summary>
        /// Loads the amount rows from the console.log file. stores the output in Chats & Commands
        /// </summary>
        /// <param name="amount"></param>
        static public void LoadLogs(int amount)
        {
            LinkedList<Log> logs = new LinkedList<Log>();

            List<string> lines = GetLastLines(amount);

            if (lines != null && lines.Count() != 0)
            {
                var (rawStrings, chatTypes, names, rawMessage) = LineCleaner(lines);

                for (int i = 0; i < rawStrings.Count(); i++)
                {
                    if (rawMessage[i][0] == '!')
                    {
                        var possCommand = CommandsController.BuildCommand(logs.Last.Value, rawStrings[i], chatTypes[i], names[i], rawMessage[i]);

                        if(possCommand != null)
                        {
                            logs.AddLast(possCommand);
                        }
                    }
                    else
                    {
                        logs.AddLast(new Chat(logs.Last?.Value, rawStrings[i], chatTypes[i], names[i], rawMessage[i]));
                    }
                }

                /*
                for (LinkedListNode<Log> node = logs.Last; node != null; node = node.Previous)
                {
                    Console.WriteLine((node.Value as Chat ).Message);
                }
                */

                bool breakie = false;
                List<Log> addList = new List<Log>();

                if (Logs.Last != null)
                {
                    Console.WriteLine($"Lasted added value: {Logs.Last.Value.RawString}");
                }
                else
                {
                    Console.WriteLine("Lasted added value: null");
                }

                //if logs where found in the file.
                if(logs.Last != null)
                {
                    //if the application doesn't contain any logs yet, or the latest found log is not identical as the latest added log.
                    if (Logs.Last == null || logs.Last.Value.RawString != Logs.Last.Value.RawString)
                    {
                        //add the new latest found log to the array that will be added to the system.
                        Console.WriteLine($"Add because 0: {logs.Last.Value.RawString}");
                        addList.Insert(0, logs.Last.Value);

                        //loop back over the other logs to see how many others are new aswell.
                        for (LinkedListNode<Log> node = logs.Last.Previous; node != null && !breakie; node = node.Previous)
                        {
                            if (Logs.Last == null || node.Value.RawString != Logs.Last.Value.RawString)
                            {
                                Console.WriteLine($"Add because 1:  {node.Value.RawString}");
                                addList.Insert(0, node.Value);
                            }
                            else if (CompareByParents(node, Logs.Last))
                            {
                                Console.WriteLine($"Add because 2: {node.Value.RawString}");
                                addList.Insert(0, node.Value);
                            }
                            else
                            {
                                break;
                            }

                            Console.WriteLine("_______");
                        }
                    }
                }




                Console.WriteLine("=======");

                foreach(Log log in addList)
                {
                    SaveLog(log);
                }
            }

            static void SaveLog(Log log)
            {
                Logs.AddLast(log);
                if (log is Chat)
                {
                    Chats.Insert(0, log as Chat);
                }
                else if(log is Command)
                {
                    Commands.Insert(0, log as Command);
                    Console.WriteLine($"Imported command: {(log as Command).Message}");
                    Console.WriteLine($"Total commands: {Commands.Count}");
                }
            }

            static bool CompareByParents(LinkedListNode<Log> currentNode, LinkedListNode<Log> newNode)
            {
                LinkedListNode<Log> currentTreeMember = currentNode;
                LinkedListNode<Log> newTreeMember = newNode;

                for (int i = 0; i < 5; i++)
                {
                    if (newTreeMember != currentTreeMember)
                    {
                        return false;
                    }

                    newTreeMember = newTreeMember.Previous;
                    currentTreeMember = currentTreeMember.Previous;

                    if (currentTreeMember == null || newTreeMember == null)
                    {
                        return false;
                    }
                }
                return true;
            }
        }


        /*
                        if (chat.ChatType == ChatType.All && chat.Translation != chat.Message && chat.Translation != "")
                        {
                            TelnetHelper.SendTranslationInTeamChat(chat);
                        }
                        Console.WriteLine($"New chat message: {rawMessage[i]}");
        */

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
        private static (List<string> rawStrings, List<ChatType> chatTypes, List<string> names, List<string> messages) LineCleaner(List<string> lines)
        {
            List<string> returnRawStrings = new List<string>();
            List<ChatType> returnChatTypes = new List<ChatType>();
            List<string> returnNames = new List<string>();
            List<string> returnMessages = new List<string>();

            foreach (string l in lines)
            {
                //filter the lines on chat message syntax
                if (l.Contains(" : ") && !l.Contains("  : ") && !l.Contains("!.") && !l.Trim().StartsWith("Duplicate :          "))
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

                    returnRawStrings.Add(l);
                    returnChatTypes.Add(chatType);
                    returnNames.Add(namePart);
                    returnMessages.Add(messagePart);
                }
            }

            return (rawStrings: returnRawStrings, chatTypes: returnChatTypes, names: returnNames, messages: returnMessages);
        }
    }
}
