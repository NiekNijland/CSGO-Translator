namespace CsgoTranslator
{
    public class Chat : Log
    {
        public ChatType ChatType { get; private set; }
        public string Name { get; private set; }

        private string _translation;
        public string Translation
        {
            get
            {
                if (_translation == null)
                {
                    _translation = this.Translate();
                }
                return _translation;
            }
        }
        public string Message { get; private set; }

        public Chat(Log previousLog, string rawString, ChatType chatType, string name, string message) : base(previousLog, rawString)
        {
            this._translation = null;
            this.Name = name;
            this.Message = message;
            this.ChatType = chatType; 
        }

        private string Translate()
        {
            return Translator.Translate(this.Message);
        }
    }
}
