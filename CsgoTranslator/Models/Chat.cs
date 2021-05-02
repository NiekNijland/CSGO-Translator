using CsgoTranslator.Enums;
using CsgoTranslator.Helpers;

namespace CsgoTranslator.Models
{
    public class Chat : Log
    {
        public ChatType ChatType { get; }
        public string Name { get; }
        public Translation Translation => _translation ??= Translate();
        public string Message { get; }

        private Translation _translation;
        
        public Chat(string rawString, ChatType chatType, string name, string message) : base(rawString)
        {
            Name = name;
            ChatType = chatType;
            Message = message;
        }

        private Translation Translate()
        {
            return Translator.GetTranslation(Message);
        }
    }
}
