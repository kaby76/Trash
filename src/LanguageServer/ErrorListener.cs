namespace LanguageServer
{
    using Antlr4.Runtime;
    using System.IO;

    public class ErrorListener<S> : ConsoleErrorListener<S>
    {
        public bool had_error;
        private Parser _parser;
        private Lexer _lexer;
        private int _quiet_after;
        public int _number_of_errors;

        public ErrorListener(Parser parser, Lexer lexer, int quiet_after)
        {
            _parser = parser;
            _lexer = lexer;
            _quiet_after = quiet_after;
            _number_of_errors = 0;
        }

        public override void SyntaxError(TextWriter output, IRecognizer recognizer, S offendingSymbol, int line,
            int col, string msg, RecognitionException e)
        {
            had_error = true;
            _quiet_after--;
            _number_of_errors++;
            if (_quiet_after <= 0) return;
            base.SyntaxError(output, recognizer, offendingSymbol, line, col, msg, e);
        }
    }
}
