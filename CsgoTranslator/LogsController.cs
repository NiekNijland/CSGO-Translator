using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsgoTranslator
{
    static class LogsController
    {
        static private List<string> GetLastLines(int amount)
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

                    while (count < amount && fs.Position > 0)
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

        //function that checks all console log lines for chat messages and cleans them up. then returns 2 lists (1 with usernames, 1 with messages)
        static private List<List<string>> LineCleaner(List<string> lines)
        {
            List<string> returnNames = new List<string>();
            List<string> returnMessages = new List<string>();

            foreach (string l in lines)
            {
                //filter the lines on chat message syntax
                if (l.Contains(" : ") && !l.Contains("  : "))
                {
                    if (l.Contains(" :  "))
                        if (!l.Contains("(Counter-Terrorist)") && !l.Contains("(Terrorist)"))
                            continue;

                    bool name = true;
                    foreach (string s in l.Split(new string[] { " : " }, 2, StringSplitOptions.None))
                    {
                        if (name)
                        {
                            string n = s;

                            if (n.Contains("(Counter-Terrorist)"))
                            {
                                n = n.Replace("(Counter-Terrorist)", "(CT)");
                            }
                            if (n.Contains("(Terrorist)"))
                            {
                                n = n.Replace("(Terrorist)", "(T)");
                            }

                            //removal of *DEAD* chat prefix
                            if (n.Contains("*DEAD*"))
                            {
                                returnNames.Add(n.Substring(6));
                            }
                            else
                            {
                                returnNames.Add(n);
                            }

                            name = false;
                        }
                        else
                        {
                            returnMessages.Add(s);
                        }
                    }
                }
            }

            return new List<List<string>>() { returnNames, returnMessages };
        }

        static public List<Log> GetLogs(int amount)
        {
            List<Log> returnList = new List<Log>();
            List<string> lines = GetLastLines(amount);

            if (lines != null && lines.Count() != 0)
            {
                List<List<string>> rawLogs = LineCleaner(lines);

                for (int i = 0; i < rawLogs[0].Count(); i++)
                {
                    returnList.Add(new Log(rawLogs[0][i], rawLogs[1][i]));
                }

                return returnList;
            }
            else
            {
                return null;
            }

        }
    }
}
