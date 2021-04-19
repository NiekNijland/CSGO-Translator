using CsgoTranslator.Enums;
using CsgoTranslator.Helpers;

namespace CsgoTranslator.Models
{
    public class Chat : Log
    {
        public ChatType ChatType { get; }
        public string Name { get; }
        public string Translation => _translation ??= Translate();
        private string Message { get; }

        private string _translation;
        
        public Chat(string rawString, ChatType chatType, string name, string message) : base(rawString)
        {
            Name = name;
            ChatType = chatType;
            Message = message;
        }

        private string Translate()
        {
            return Translator.Translate(Message);
        }
    }
}
