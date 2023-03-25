namespace XmlDOM
{
    using Antlr4.Runtime.Tree;
    using Antlr4.Runtime;

    public class Parse
    {
        public static (IParseTree, Parser, Lexer) Try(string input)
        {
            var str = new AntlrInputStream(input);
            var lexer = new XMLLexer(str);
            var tokens = new CommonTokenStream(lexer);
            var parser = new XMLParser(tokens);
            var listener = new ErrorListener<IToken>(parser, lexer, tokens);
            parser.AddErrorListener(listener);
            var tree = parser.document();
            if (listener.had_error)
            {
                System.Console.Error.WriteLine("error in parse.");
            }
            else
            {
                System.Console.Error.WriteLine("parse completed.");
            }
            System.Console.Error.WriteLine(tokens.OutputTokens(lexer));
            System.Console.Error.WriteLine(tree.OutputTree(tokens));
            return (tree, parser, lexer);
        }
    }
}
