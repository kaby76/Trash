using CommandLine;
using System.Collections.Generic;

namespace Trash
{
    public class Config
    {
        [Option("all_source_pattern", Required = false, HelpText = "R.E. for all source files to use.")]
        public string all_source_pattern { get; set; }

        [Option("antlr-encoding", Required = false, Default = "utf-8")]
        public string antlr_encoding { get; set; }

        [Option("antlr-tool-args", Required = false)]
        public IEnumerable<string> antlr_tool_args { get; set; }

        [Option("antlr-tool-path", Required = false)]
        public string antlr_tool_path { get; set; }

        [Option("case-insensitive-type", Required = false)]
        public CaseInsensitiveType? case_insensitive_type { get; set; }

        [Option("env-type", Required = false)]
        public EnvType? env_type { get; set; }

        [Option('f', "file", Required = false, HelpText = "The name of an input file to parse.")]
        public string InputFile { get; set; }

        [Option("flatten", Required = false, HelpText = "Flatten files in target into non-nested directory.")]
        public bool? flatten { get; set; }


        [Option("go-lexer-name", Required = false, HelpText = "The name of the lexer.")]
        public string fully_qualified_go_lexer_name { get; set; }

        [Option('e', "line-translation", Required = false)]
        public LineTranslationType? line_translation { get; set; }

        [Option('m', "maven", Required = false, HelpText = "Read Antlr pom file and convert.")]
        public bool? maven { get; set; }

        [Option('n', "name-space", Required = false, HelpText = "The namespace for all generated files.")]
        public string name_space { get; set; }

        [Option('o', "output-directory", Required = false, HelpText = "The output directory for the project.")]
        public string output_directory { get; set; }

        [Option("go-parser-name", Required = false)]
        public string fully_qualified_go_parser_name { get; set; }

        [Option("path-sep", Required = false)]
        public PathSepType? path_sep { get; set; }

        [Option('x', "profile", Required = false, HelpText = "Add in Antlr profiling code.")]
        public bool? profile { get; set; }

        [Option('k', "skip-list", Required = false, Separator = ',', HelpText = "A skip list for pom.xml.")]
        public IEnumerable<string> skip_list { get; set; }

        [Option('s', "start-rule", Required = false, HelpText = "Start rule name.")]
        public string start_rule { get; set; }

        [Option('t', "target", Required = false, HelpText = "The target language for the project.")]
        public string target { get; set; }

        [Option("template-sources-directory", Required = false)]
        public string template_sources_directory { get; set; }

        [Option("todo-pattern", Required = false, HelpText ="Replacement for skip-list. R.E. on what to do, what not to do, of the grammars in the poms.")]
        public string todo_list { get; set; }

        [Option('g', "tool-grammar-files-pattern", Required = false, HelpText = "A list of vertical bar separated grammar file paths.")]
        public string tool_grammar_files_pattern { get; set; }

        [Option("watchdog-timeout", Required = false, Default = 60)]
        public int? watchdog_timeout { get; set; }

        [Option('v', "verbose", Required = false)]
        public bool Verbose { get; set; }
    }
}
