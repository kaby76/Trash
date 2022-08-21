using System;
using System.Collections.Generic;
using Antlr4.Runtime;

namespace LanguageServer
{
    public class BailErrorHandler : BailErrorStrategy
    {
        public bool had_error;

        public override void ReportError(Parser recognizer, RecognitionException e)
        {
            had_error = true;
            base.ReportError(recognizer, e);
        }

        public override void Recover(Parser recognizer, RecognitionException e)
        {
            had_error = true;
            base.Recover(recognizer, e);
        }

        public override IToken RecoverInline(Parser recognizer)
        {
            had_error = true;
            return base.RecoverInline(recognizer);
        }
    }
}
