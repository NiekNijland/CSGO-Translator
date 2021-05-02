namespace CsgoTranslator.Models
{
    public class Translation
    {
        public string Message { get; }
        public string Lang { get; }

        public Translation(string lang, string message = "---")
        {
            Lang = lang;
            Message = message;
        }
    }
}