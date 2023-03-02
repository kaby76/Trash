using Antlr4.Runtime.Tree;
using XmlDOM;

namespace Trash
{
    public class GrammarTuple
    {
        public enum Type { Parser = 1, Lexer = 2, Combined = 3 }
        public GrammarTuple(Type type, string source_grammar_file_name, string target_grammar_file_name, string grammar_name, string generated_file_name, string generated_include_file_name, string grammar_autom_name, string grammar_go_new_name, string antlr_args)
        {
            WhatType = type;
            OriginalSourceFileName = source_grammar_file_name;
            GrammarFileName = target_grammar_file_name;
            GrammarName = grammar_name;
            GeneratedFileName = generated_file_name;
            GeneratedIncludeFileName = generated_include_file_name;
            GrammarAutomName = grammar_autom_name;
            GrammarGoNewName = grammar_go_new_name;
            AntlrArgs = antlr_args;
        }
        public string OriginalSourceFileName;
        public Type WhatType { get; set; }
        public string GrammarFileName { get; set; }
        public string GrammarName { get; set; }
        public string GeneratedFileName { get; set; }
        public string GeneratedIncludeFileName { get; set; }
        public string GrammarAutomName { get; set; }
        public string GrammarGoNewName { get; set; }
        public string AntlrArgs { get; set; }
    }

}
