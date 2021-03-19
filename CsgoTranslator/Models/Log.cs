using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsgoTranslator
{
    public class Log
    {
        public Log PreviousLog { get; set; }

        public string RawString { get; private set; }

        public Log(Log previousLog, string rawString)
        {
            this.PreviousLog = previousLog;
            this.RawString = rawString;
        }
    }
}
