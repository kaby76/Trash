using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.CodeAnalysis.Host;
using org.eclipse.wst.xml.xpath2.processor.@internal.ast;

namespace Trash
{
    public class Config
    {
        [Option('a', "arithmetic", Required = false, HelpText = "Generate arithmetic example from templates.")]
        public bool arithmetic { get; set; }

        [Value(0)]
        public IEnumerable<string> Files { get; set; }

        [Option('x', "profile", Required = false, HelpText = "Add in Antlr profiling code.")]
        public bool? profile { get; set; }

        [Option('s', "start-rule", Required = false, HelpText = "Start rule name.")]
        public string start_rule { get; set; }

        [Option('g', "grammar-name", Required = false, HelpText = "Grammar for parse.")]
        public string grammar_name { get; set; }

        [Option('t', "target", Required = false, HelpText = "The target language for the project.")]
        public string target
        {
            get { return _backing_target; }
            set
            {
                _backing_target = value;
                this.output_directory = "Generated" + value;
            }
        }
        private string _backing_target;

        [Option("antlr-tool-path", Required = false)]
        public string antlr_tool_path { get; set; }

        [Option('o', "output-directory", Required = false, HelpText = "The output directory for the project.")]
        public string output_directory { get; set; }

        [Option('p', "package", Required = false)]
        public string name_space { get; set; }

        [Option("template-sources-directory", Required = false)]
        public string template_sources_directory { get { return _backing_template_sources_directory; } set { _backing_template_sources_directory = Path.GetFullPath(value); } }
        private string _backing_template_sources_directory;

        [Option('v', "verbose", Required = false)]
        public bool Verbose { get; set; }


        public IEnumerable<string> antlr_tool_args { get; set; }
        public OSType? env_type { get; set; }
        public bool? flatten { get; set; }
        public LineTranslationType? line_translation { get; set; }
        public bool pom { get; set; }
        public bool desc { get; set; }
        public PathSepType? path_sep { get; set; }
        public int? watchdog_timeout { get; set; }
        public string SetupFfn = ".trgen.rc";
        public string root_directory;

        public List<Test> Tests;

        public Config()
        {
            this.antlr_tool_path = Command.GetAntlrToolPath();
            this.arithmetic = false;
            this.desc = true;
            this.env_type = Command.GetEnvType();
            this.Files = new List<string>();
            this.flatten = false;
            this.grammar_name = null; // means find using parsing and xpath of grammars.
            this.line_translation = Command.GetLineTranslationType();
            this.name_space = null;
            this.output_directory = "Generated";
            this.path_sep = Command.GetPathSep();
            this.pom = false;
            this.root_directory = Environment.CurrentDirectory.Replace('\\', '/') + "/";
            this.start_rule = null; // means find using parsing and xpath of grammars.
            this.target = null;
            this.Tests = new List<Test>();
            this.watchdog_timeout = 60;
        }

        public Config(Config copy)
        {
            var ty = typeof(Config);
            foreach (var prop in ty.GetProperties())
            {
                if (prop.GetValue(copy, null) != null)
                {
                    prop.SetValue(this, prop.GetValue(copy, null));
                    this.name_space = this.target == "Go" ? "parser" : null;
                }
            }
        }

        public void OverrideWithTrgenRc()
        {
            // Add in defaults from .trgen.rc
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
                        this.name_space = this.target == "Go" ? "parser" : null;
                    }
                }
            }
        }

        public static readonly Config DEFAULT = new Config();
    }
}
