namespace CsgoTranslator
{
    public class TransTeamCommand : Command
    {
        public TransTeamCommand(ChatType chatType, string name, string message, string commandParams = null) : base(chatType, name, message, commandParams) { }

        public override void Execute()
        {
            TelnetHelper.SendInTeamChat(Translator.Translate(this.Message));
            this.Executed = true;
        }
    }
}
