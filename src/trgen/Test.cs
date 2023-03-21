using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trash
{
    public class Test
    {
        public List<string> grammar_directory_source_files = null;
        public List<string> all_target_files = null;
        public string antlr_encoding { get; set; } = "utf-8";
        public CaseInsensitiveType? case_insensitive_type { get; set; } = null;
        public string current_directory;
        public string example_files { get; set; } = "examples/";
        public string fully_qualified_lexer_name { get; set; }
        public string fully_qualified_parser_name { get; set; }
        public string fully_qualified_go_parser_name { get; set; }
        public string fully_qualified_go_lexer_name { get; set; }
        public string grammar_name { get; set; }
        public List<string> generated_files = null;
        public string ignore_string = null;
        public string lexer_grammar_file_name = null;
        public string package { get; set; } = "";
        public string parser_grammar_file_name = null;
        public string parsing_type { get; set; }
        public string start_rule { get; set; }
        public string target { get; set; }
        public string test_name { get; set; } = null;
        public List<string> tool_grammar_files = null;
        public List<GrammarTuple> tool_grammar_tuples = null;
        public HashSet<string> tool_src_grammar_files = null;
        public List<string> os_targets { get; set; } = null;
        public Test() {}
    }
}
