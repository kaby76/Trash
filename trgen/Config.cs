using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Trash
{
    public class Config
    {
        [Option('x', "profile", Required = false, HelpText = "Add in Antlr profiling code.")]
        public bool? profile { get; set; }

        [Option('s', "start-rule", Required = false, HelpText = "Start rule name.")]
        public string start_rule { get; set; }

        private string _backing_target;
        [Option('t', "target", Required = false, HelpText = "The target language for the project.")]
        public string target
        {
            get { return _backing_target; }
            set {
                _backing_target = value;
                all_source_pattern = "^(?!.*(" +
                    (ignore_string != null ? ignore_string + "|" : "")
                    + "ignore/|Generated/|target/|examples/|.git/|.gitignore|"
                    + Command.AllButTargetName(this.target)
                    + "/)).+"
                    + "$";
            }
        }

        [Option('f', "file", Required = false, HelpText = "The name of an input file to parse.")]
        public string InputFile { get; set; }

        [Option("antlr-tool-path", Required = false)]
        public string antlr_tool_path { get; set; }

        [Option('o', "output-directory", Required = false, HelpText = "The output directory for the project.")]
        public string output_directory { get; set; }

        private string _backing_template_sources_directory;
        [Option("template-sources-directory", Required = false)]
        public string template_sources_directory { get { return _backing_template_sources_directory; } set { value = Path.GetFullPath(value); } }

        [Option("skip-pattern", Required = false, HelpText = "Replacement for skip-list. R.E. on what to do, what not to do, of the grammars in the poms.")]
        public string skip_pattern { get; set; }

        [Option("todo-pattern", Required = false, HelpText = "Replacement for todo-list. R.E. on what to do, what not to do, of the grammars in the poms.")]
        public string todo_pattern { get; set; }

        [Option('v', "verbose", Required = false)]
        public bool Verbose { get; set; }



        private string _backing_all_source_pattern;
        public string all_source_pattern {
            get { return _backing_all_source_pattern; }
            set { _backing_all_source_pattern = value; }
        }
        public string antlr_encoding { get; set; }
        public IEnumerable<string> antlr_tool_args { get; set; }
        public CaseInsensitiveType? case_insensitive_type { get; set; }
        public EnvType? env_type { get; set; }
        public bool? flatten { get; set; }
        public string fully_qualified_go_lexer_name { get; set; }
        public LineTranslationType? line_translation { get; set; }
        public bool? maven { get; set; }
        public string name_space { get; set; }
        public string fully_qualified_go_parser_name { get; set; }
        public PathSepType? path_sep { get; set; }
        public string tool_grammar_files_pattern { get; set; }
        public int? watchdog_timeout { get; set; }
        public string ignore_string = null;
        public string SetupFfn = ".trgen.rc";
        public string root_directory;

        public Config()
        {
            // Get default from OS, or just default.
            this.line_translation = Command.GetLineTranslationType();
            this.env_type = Command.GetEnvType();
            this.path_sep = Command.GetPathSep();
            this.antlr_tool_path = Command.GetAntlrToolPath();
            this.target = "CSharp";
            this.tool_grammar_files_pattern = "^(?!.*(/Generated|/target|/examples)).+g4$";
            this.output_directory = "Generated/";
            this.flatten = false;
            this.antlr_encoding = "utf-8";
            this.all_source_pattern =
                     "^(?!.*(" +
                      (ignore_string != null ? ignore_string + "|" : "")
                      + "ignore/|Generated/|target/|examples/|.git/|.gitignore|"
                      + Command.AllButTargetName(this.target)
                      + "/)).+"
                      + "$"; // Get any defaults from ~/.trgen.rc
            this.watchdog_timeout = 60;
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (System.IO.File.Exists(home + Path.DirectorySeparatorChar + SetupFfn))
            {
                var jsonString = System.IO.File.ReadAllText(home + Path.DirectorySeparatorChar + SetupFfn);
                var o = JsonSerializer.Deserialize<Config>(jsonString);
                var ty = typeof(Config);
                foreach (var prop in ty.GetProperties())
                {
                    if (prop.GetValue(o, null) != null)
                    {
                        prop.SetValue(this, prop.GetValue(o, null));
                    }
                }
            }
            var path = Environment.CurrentDirectory;
            string cd = Environment.CurrentDirectory.Replace('\\', '/') + "/";
            this.root_directory = cd;
            if (this.maven == null)
            {
                maven = File.Exists(cd + Path.DirectorySeparatorChar + @"pom.xml");
            }
        }

        public Config(Config copy)
        {
            var ty = typeof(Config);
            foreach (var prop in ty.GetProperties())
            {
                if (prop.GetValue(copy, null) != null)
                {
                    prop.SetValue(this, prop.GetValue(copy, null));
                }
            }
        }
    }
}
