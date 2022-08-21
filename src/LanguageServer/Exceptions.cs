using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServer
{
    public class LanguageServerException : Exception
    {
        public LanguageServerException(string message)
            : base(message)
        { }
    }
}
