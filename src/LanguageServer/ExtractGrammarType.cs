using Antlr4.Runtime.Misc;

namespace LanguageServer
{
    public class ExtractGrammarType : ANTLRv4ParserBaseListener
    {
        public enum GrammarType
        {
            Combined,
            Parser,
            Lexer,
            NotAGrammar
        }

        public GrammarType Type;

        public ExtractGrammarType()
        {
        }

        public override void EnterGrammarType([NotNull] ANTLRv4Parser.GrammarTypeContext context)
        {
            if (context.GetChild(0).GetText() == "parser")
            {
                Type = GrammarType.Parser;
            }
            else if (context.GetChild(0).GetText() == "lexer")
            {
                Type = GrammarType.Lexer;
            }
            else
            {
                Type = GrammarType.Combined;
            }
        }
    }
}
