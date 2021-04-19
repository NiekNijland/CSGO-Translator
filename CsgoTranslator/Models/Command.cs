using System;
using CsgoTranslator.Enums;

namespace CsgoTranslator.Models
{
    public abstract class Command : Log
    {
        public ChatType ChatType { get; }
        public string Name { get; }
        protected string Message { get; }
        protected string LangParam { get; }
        public bool Executed { get; set; }

        protected Command(string rawLog, ChatType chatType, string name, string message, string langParam = null) : base(rawLog)
        {
            this.ChatType = chatType;
            this.Name = name;
            this.Message = message;
            this.LangParam = langParam;
            this.Executed = false;
        }


        public virtual void Execute() { }
    }
}
