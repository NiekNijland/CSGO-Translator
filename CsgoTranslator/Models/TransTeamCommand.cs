namespace CsgoTranslator
{
    public class TransTeamCommand : Command
    {
        public TransTeamCommand(ChatType chatType, string name, string message, string commandParams = null) : base(chatType, name, message, commandParams) { }

        public override void Execute()
        {
            string translation = Translator.Translate(this.Message, this.LangParam).Trim();
            
            if(translation != this.Message && translation != "")
            {
                TelnetHelper.SendInTeamChat(translation);
            }

            this.Executed = true;
        }
    }
}
