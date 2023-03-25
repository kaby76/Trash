// Template generated code from Antlr4BuildTasks.Template v 8.1
namespace XmlDOM
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Misc;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class ErrorListener<S> : ConsoleErrorListener<S>
    {
        public bool had_error;
        private readonly Parser _parser;
        private readonly Lexer _lexer;
        private readonly CommonTokenStream _token_stream;
        private bool _first_time;

        public bool UseNfaInterpForErrors
        {
            get;
            set;
        }

        public ErrorListener(Parser parser, Lexer lexer, CommonTokenStream token_stream)
        {
            _parser = parser;
            _lexer = lexer;
            _token_stream = token_stream;
            _first_time = true;
            UseNfaInterpForErrors = true;
        }

        public override void SyntaxError(TextWriter output, IRecognizer recognizer, S offendingSymbol, int line,
            int col, string msg, RecognitionException e)
        {
            had_error = true;
            if (_first_time && UseNfaInterpForErrors)
            {
                try
                {
                    _first_time = false;
                    LASets la_sets = new LASets();
                    IntervalSet set = la_sets.Compute(_parser, _token_stream, line, col);
                    List<string> result = new List<string>();
                    foreach (int r in set.ToList())
                    {
                        string rule_name = _parser.Vocabulary.GetSymbolicName(r);
                        result.Add(rule_name);
                    }
                    if (result.Any())
                    {
                        System.Console.Error.WriteLine("Parse error line/col " + line + "/" + col
                                                       + ", expecting "
                                                       + string.Join(", ", result));
                    }
                    else
                    {
                        base.SyntaxError(output, recognizer, offendingSymbol, line, col, msg, e);
                    }
                    return;
                }
                catch (Exception)
                {
                }
            }
            base.SyntaxError(output, recognizer, offendingSymbol, line, col, msg, e);
        }
    }
}