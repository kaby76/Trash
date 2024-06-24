using Antlr4.Runtime.Tree;
using XmlDOM;

namespace Trash
{
    public class GrammarTuple
    {
        public enum Type { Parser = 1, Lexer = 2, Combined = 3 }

        public GrammarTuple()
        {
            IsTopLevel = false;
        }

        public GrammarTuple(Type type,
            string source_grammar_file_name,
            string target_grammar_file_name,
            string grammar_name,
            string start_symbol,
            AntlrJson.ParsingResultSet pr,
            string generated_file_name,
            string generated_include_file_name,
            string grammar_autom_name,
            string grammar_go_new_name,
            string antlr_args)
        {
            AntlrArgs = antlr_args;
            GeneratedFileName = generated_file_name;
            GeneratedIncludeFileName = generated_include_file_name;
            GrammarAutomName = grammar_autom_name;
            GrammarFileName = target_grammar_file_name;
            GrammarGoNewName = grammar_go_new_name;
            GrammarName = grammar_name;
            IsTopLevel = false;
            OriginalSourceFileName = source_grammar_file_name;
            ParsingResultSet = pr;
            StartSymbol = start_symbol;
            WhatType = type;
        }
        public string AntlrArgs { get; set; }
        public string GeneratedFileName { get; set; }
        public string GeneratedIncludeFileName { get; set; }
        public string GrammarAutomName { get; set; }
        public string GrammarFileName { get; set; }
        public string GrammarGoNewName { get; set; }
        public string GrammarName { get; set; }
        public bool IsTopLevel { get; set; }
        public string OriginalSourceFileName { get; set; }
        public AntlrJson.ParsingResultSet ParsingResultSet { get; set; }
        public string StartSymbol { get; set; }
        public Type WhatType { get; set; }
    }

}
