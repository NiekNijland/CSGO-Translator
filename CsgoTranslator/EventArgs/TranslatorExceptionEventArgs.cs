using System;
using CsgoTranslator.Exceptions;

namespace CsgoTranslator.EventArgs
{
    public class TranslatorExceptionEventArgs : System.EventArgs
    {
        public TranslatorException Exception;
    }
}