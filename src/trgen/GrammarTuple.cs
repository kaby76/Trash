using Antlr4.Runtime.Tree;
using XmlDOM;

namespace Trash
{
    public class GrammarTuple
    {
        public enum Type { Parser = 1, Lexer = 2, Combined = 3 }

        public GrammarTuple()
        {
        }

        public string AntlrArgs { get; set; }
        public string GeneratedFileName { get; set; }
        public string GeneratedIncludeFileName { get; set; }
        public string GrammarAutomName { get; set; }
        public string GrammarFileNameTarget { get; set; }
        public string GrammarFileNameSource { get; set; }
        public string GrammarGoNewName { get; set; }
        public string GrammarName { get; set; }
        public string GrammarNameOriginating { get; set; }
        public bool IsTopLevel { get; set; }
        public string OriginalSourceFileName { get; set; }
        public AntlrJson.ParsingResultSet ParsingResultSet { get; set; }
        public string StartSymbol { get; set; }
        public Type WhatType { get; set; }
        public Type WhatTypeOriginating { get; set; }
    }

}
