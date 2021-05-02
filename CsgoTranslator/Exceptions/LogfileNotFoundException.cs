using System;

namespace CsgoTranslator.Exceptions
{
    public class LogfileNotFoundException : TranslatorException
    {
        public LogfileNotFoundException() : base("Logfile not found") { }
    }
}