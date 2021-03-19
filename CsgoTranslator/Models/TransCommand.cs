namespace CsgoTranslator
{
    public enum TransType
    {
        Undefined,
        TransAllCommand,
        TransTeamCommand,
    }

    public class TransCommand : Command
    {
        public TransType TransType { get; private set; }
        public string Translation { get; private set; }

        public TransCommand(Log previousLog, string rawString, TransType transType, ChatType chatType, string name, string message, string commandParams = null) : base(previousLog, rawString, chatType, name, message, commandParams) 
        {
            this.TransType = transType;

        }

        public override void Execute()
        {
            string translation = Translator.Translate(this.Message, this.LangParam).Trim();
            
            if(translation != this.Message && translation != "")
            {
                this.Translation = translation;
                TelnetHelper.SendTransCommand(this);
            }

            this.Executed = true;
        }
    }
}
