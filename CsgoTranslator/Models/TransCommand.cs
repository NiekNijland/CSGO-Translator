namespace CsgoTranslator
{
    public class TransCommand : Command
    {
        public ChatType ExportChatType { get; private set; }
        public string Translation { get; private set; }

        public TransCommand(Log previousLog, string rawString, ChatType exportChatType, ChatType chatType, string name, string message, string commandParams = null) : base(previousLog, rawString, chatType, name, message, commandParams) 
        {
            this.ExportChatType = exportChatType;

        }

        public void Translate()
        {
            this.Translation = Translator.Translate(this.Message, this.LangParam).Trim();
        }

        public override void Execute()
        {
            Translate();
            
            if(this.Translation != this.Message && this.Translation != "")
            {
                TelnetHelper.SendInChat(this.ExportChatType, this.Translation);
            }

            this.Executed = true;
        }
    }
}
