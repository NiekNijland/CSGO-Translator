namespace CsgoTranslator
{
    public class TransAllCommand : Command
    {
        public TransAllCommand(ChatType chatType, string name, string message, string commandParams = null) : base(chatType, name, message, commandParams) { }

        public override void Execute()
        {
            TelnetHelper.SendInAllChat(Translator.Translate(this.Message, this.LangParam));
            this.Executed = true;
        }
    }
}
