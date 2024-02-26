using System;

namespace Server
{
    public class LanguageServerException : Exception
    {
        public LanguageServerException(string message)
            : base(message)
        { }
    }
}
