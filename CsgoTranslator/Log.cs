using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CsgoTranslator
{
    public class Log
    {
        public string Name { get; private set; }
        public string Message { get; private set; }
        public string OriginalMessage { get; private set; }

        public Log(string name, string message)
        {
            this.Name = name;
            this.OriginalMessage = message;
        }

        public void Translate()
        {
            this.Message = Translator.Translate(this.OriginalMessage);
        }
    }
}
