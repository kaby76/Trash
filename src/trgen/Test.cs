using System;
using System.Collections.Generic;
using System.IO;
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
        public string fully_qualified_listener_name { get; set; }
        public string fully_qualified_parser_name { get; set; }
        public string fully_qualified_go_parser_name { get; set; }
        public string fully_qualified_go_lexer_name { get; set; }
        public string grammar_name { get; set; }
        //public List<string> generated_files = null;
        public string ignore_string = null;
        public string lexer_grammar_file_name = null;
        public string package { get; set; } = "";
        public string parser_grammar_file_name = null;
        public string parsing_type { get; set; }
        public string start_rule { get; set; }
        public string target { get; set; }
        public string output_directory { get; set; }
        public string test_name { get; set; } = null;
        public List<string> tool_grammar_files = null;
        public List<GrammarTuple> tool_grammar_tuples = null;
        public HashSet<string> tool_src_grammar_files = null;
        public string os_target { get; set; }

        public string file_encoding = null;
        public bool? binary = null;

        public bool IsWindows
        {
            get { return this.os_target == "Windows"; }
        }
        public bool IsLinux
        {
            get { return this.os_target == "Linux"; }
        }
        public bool IsMac
        {
            get { return this.os_target == "OSX"; }
        }

        public bool isTargetCSharp
        {
            get { return this.target == "CSharp"; }
        }
        public bool IsTargetCpp
        {
            get { return this.target == "Cpp"; }
        }
        public bool IsTargetDart2
        {
            get { return this.target == "Dart2"; }
        }
        public bool IsTargetGo
        {
            get { return this.target == "Go"; }
        }
        public bool IsTargetJava
        {
            get { return this.target == "Java"; }
        }
        public bool IsTargetJavaScript
        {
            get { return this.target == "JavaScript"; }
        }
        public bool IsTargetPHP
        {
            get { return this.target == "PHP"; }
        }
        public bool IsTargetPython3
        {
            get { return this.target == "Python3"; }
        }
        public bool IsTargetTypeScript
        {
            get { return this.target == "TypeScript"; }
        }

        public Test(Config config)
        {
            this.ignore_string = null;

            // Check for existence of .trgen-ignore file.
            // If there is one, read and create pattern of what to ignore.
            if (File.Exists(config.ignore_list_of_files))
            {
                var ignore = new StringBuilder();
                var lines = File.ReadAllLines(config.ignore_list_of_files);
                var ignore_lines = lines.Where(l => !l.StartsWith("//")).ToList();
                this.ignore_string = string.Join("|", ignore_lines);
            }
            if (config.ignore.Any())
            {
                var t = string.Join("|", config.ignore);
                if (this.ignore_string != null)
                {
                    this.ignore_string += "|" + t;
                }
                else
                {
                    this.ignore_string = t;
                }
            }
            
        }
    }
}
