using System.Collections.Generic;

namespace Trash
{
    class PerGrammar
    {
        public List<string> tool_grammar_files = null;
        public List<string> generated_files = null;
        public HashSet<string> tool_src_grammar_files = null;
        public List<GrammarTuple> tool_grammar_tuples = null; 
        public List<string> all_target_files = null;
        public List<string> all_source_files = null;
        internal string current_directory;
        public string source_directory;
        public string fully_qualified_lexer_name { get; set; }
        public string fully_qualified_parser_name { get; set; }
        public string fully_qualified_go_parser_name { get; set; }
        public string fully_qualified_go_lexer_name { get; set; }
        public string grammar_name { get; set; }
        public string lexer_grammar_file_name = null;
        public string parser_grammar_file_name = null;
        public string example_files { get; set; }
        public string start_rule { get; set; }
        public string package;
        public CaseInsensitiveType? case_insensitive_type { get; set; } = null;
        public string ignore_string = null;
        public string antlr_encoding { get; set; } = "utf-8";
    }
}
