namespace LanguageServer
{
    using Antlr4.Runtime;
    using System.IO;
    using System.Text;

    public class ErrorListenerBison<S> : ConsoleErrorListener<S>
    {
        public bool had_error;
        private readonly Parser _parser;
        private readonly Lexer _lexer;
        private readonly CommonTokenStream _token_stream;
        private readonly StringBuilder _errors;

        public ErrorListenerBison(Parser parser, Lexer lexer, CommonTokenStream token_stream, StringBuilder errors)
        {
            _parser = parser;
            _lexer = lexer;
            _token_stream = token_stream;
            _errors = errors;
        }

        public override void SyntaxError(TextWriter output, IRecognizer recognizer, S offendingSymbol, int line,
            int col, string msg, RecognitionException e)
        {
            if (!had_error)
            {
                _errors.AppendLine("Parse of Bison file "
                    + _token_stream.SourceName
                    + " failed. Please correct and try again.");
            }
            had_error = true;
        }
    }
}