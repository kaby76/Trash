namespace LanguageServer
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using System;
    using System.Collections.Generic;

    public interface IParserDescription
    {
        string Name { get; }
        Antlr4.Runtime.Parser Parser { get; }
        Antlr4.Runtime.Lexer Lexer { get; }
        int QuietAfter { get; set; }
        void Parse(ParsingResults pd, bool bail);
        void Parse(string code,
            out CommonTokenStream TokStream,
            out Parser Parser,
            out Lexer Lexer,
            out IParseTree ParseTree);
        Dictionary<IToken, int> ExtractComments(string code);
        bool CanNextRule { get; }
        string[] Map { get; }
        List<bool> CanFindAllRefs { get; }
        List<bool> CanRename { get; }
        List<bool> CanGotodef { get; }
        List<bool> CanGotovisitor { get; }
        bool CanReformat { get; }
        Func<IParserDescription, Dictionary<IParseTree, IList<Domemtech.Symtab.CombinedScopeSymbol>>, IParseTree, int> Classify { get; }
        bool IsFileType(string ffn);
        string FileExtension { get; }
        string StartRule { get; }
        bool DoErrorSquiggles { get; }
        List<Func<ParsingResults, IParseTree, string>> PopUpDefinition { get; }

    }
}
