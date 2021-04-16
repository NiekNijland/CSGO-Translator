using CsgoTranslator.Helpers;

namespace CsgoTranslator.Models
{
    public class TransCommand : Command
    {
        private ChatType ExportChatType { get; }
        private string Translation { get; set; }

        public TransCommand(string rawString, ChatType exportChatType, ChatType chatType, string name, string message, string commandParams = null) : base(rawString, chatType, name, message, commandParams) 
        {
            ExportChatType = exportChatType;
        }

        public override void Execute()
        {
            Translation = Translator.Translate(Message, LangParam).Trim();
            
            if(Translation != Message && Translation != "")
                TelnetHelper.SendInChat(ExportChatType, Translation);

            Executed = true;
        }
    }
}
