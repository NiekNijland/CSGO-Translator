namespace CsgoTranslator.Exceptions
{
    public class GoogleTranslateTimeoutException : TranslatorException
    {
        public GoogleTranslateTimeoutException() : base("Google translate timeout") { }
    }
}