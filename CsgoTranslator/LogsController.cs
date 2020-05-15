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
            if(File.Exists($@"{Properties.Settings.Default.Path}\csgo\console.log"))
            {
                //copying console.log as console2.log so CSGO Translator can acces it.
                File.Copy($@"{Properties.Settings.Default.Path}\csgo\console.log", @"C:\Program Files (x86)\Steam\steamapps\common\Counter-Strike Global Offensive\csgo\console2.log");

                //TODO: optimize the linereading so that not all lines are retrieved if there are more then 100

                //if file contains atleast a 100 rows, only return the most recent 100
                var Lines = File.ReadLines($@"{Properties.Settings.Default.Path}\csgo\console2.log");
                List<string> returnList;
                if(Lines.Count() <= 100)
                {
                    returnList = Lines.ToList();
                }
                else
                {
                    returnList = Lines.Reverse().Take(amount).Reverse().ToList();
                }

                //delete console2.log
                File.Delete($@"{Properties.Settings.Default.Path}\csgo\console2.log");
                return returnList;
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
                if(l.Contains(" : ") && !l.Contains("  : ") && !l.Contains(" :  "))
                {
                    bool name = true;
                    foreach(string s in l.Split(':'))
                    {
                        if(name)
                        {
                            //removal of *DEAD* chat prefix
                            if (s.Contains("*DEAD*"))
                            {
                                returnNames.Add(s.Substring(0, s.Length - 1).Substring(7));
                            }
                            else
                            {
                                returnNames.Add(s.Substring(0, s.Length - 1));
                            }

                            name = false;
                        }
                        else
                        {
                            returnMessages.Add(s.Substring(1));
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

            if(lines != null)
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
