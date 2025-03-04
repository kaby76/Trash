using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Trash
{
    public class Config
    {
        [Option("antlr-tool-path", Required = false)]
        public string antlr_tool_path { get; set; }

        [Option("arithmetic", Required = false, HelpText = "Generate arithmetic example from templates.")]
        public bool generateArithmeticExample { get; set; }

        [Option('e', "os-targets", Required = false, HelpText = "Set os target type.")]
        public IEnumerable<string> os_targets { get; set; } = new List<string>() { Command.GetOSTarget() };

        [Option("force", Required = false, HelpText = "Force the generation of a target.")]
        public bool force { get; set; }

        [Option('g', "generator", Required = false, HelpText = "Name of generator: 'Official', 'Antlr-ng', etc.")]
        public string generator_name { get; set; }

        [Option("grammar-name", Required = false, HelpText = "Grammar for parse.")]
        public string grammar_name { get; set; }

	    [Option('i', "ignore", Required = false, Separator = ',', HelpText = "Ignored files or directories for generating app.")]
	    public IEnumerable<string> ignore { get; set; } = new List<string>();
	
        [Option('o', "output-directory", Required = false, HelpText = "The output directory for the project.")]
        public string output_directory { get; set; }

        [Option('p', "package", Required = false)]
        public string name_space { get; set; }

        [Option('s', "start-rule", Required = false, HelpText = "Start rule name.")]
        public string start_rule { get; set; }

        [Option("template-sources-directory", Required = false)]
        public string template_sources_directory { get { return _backing_template_sources_directory; } set { _backing_template_sources_directory = Path.GetFullPath(value); } }
        private string _backing_template_sources_directory;

        [Option('t', "targets", Required = false, HelpText = "The target language for the project.")]
        public IEnumerable<string> targets { get; set; }

        [Option('v', "verbose", Required = false)]
        public bool Verbose { get; set; }

	    [Option('x', "profile", Required = false, HelpText = "Add in Antlr profiling code.")]
        public bool? profile { get; set; }

	    [Value(0)]
        public IEnumerable<string> Files { get; set; }

        public IEnumerable<string> antlr_tool_args { get; set; }
        public bool? flatten { get; set; }
        public LineTranslationType? line_translation { get; set; }
        public string parsing_type { get; set; }
        public bool hasDesc { get; set; }
        public PathSepType? path_sep { get; set; }
        public int? watchdog_timeout { get; set; }
        public string SetupFfn = ".trgen.rc";
        public string root_directory;
        public string example_files { get; set; }

        public List<Test> Tests;

        public string ignore_list_of_files = ".trgen-ignore";
        public List<string> imports { get; set; } = new List<string>();

        public Config()
        {
            this.antlr_tool_path = Command.GetAntlrToolPath();
            this.generateArithmeticExample = false;
            string file_name = Environment.CurrentDirectory + Path.DirectorySeparatorChar + @"desc.xml";
            this.hasDesc = File.Exists(file_name);
            this.os_targets = new List<string>() { Command.GetOSTarget() };
            this.Files = new List<string>();
            this.flatten = false;
            this.grammar_name = null; // null means find using parsing and xpath of grammars.
            this.line_translation = Command.GetLineTranslationType();
            this.name_space = null;
            this.output_directory = "./";
            this.parsing_type = null;
            this.path_sep = Command.GetPathSep();
            this.root_directory = Environment.CurrentDirectory.Replace('\\', '/') + "/";
            this.start_rule = null; // means find using parsing and xpath of grammars.
            this.targets = null;
            this.generator_name = "official";
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
                    }
                }
            }
        }
    }
}
