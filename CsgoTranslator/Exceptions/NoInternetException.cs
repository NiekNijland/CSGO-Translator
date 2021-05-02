namespace CsgoTranslator.Exceptions
{
    public class NoInternetException : TranslatorException
    {
        public NoInternetException() : base("No internet") { }
    }
}