using CommandLine;
using System.Collections.Generic;

namespace Trash
{
    public class Config
    {
        [Option('f', "file", Required = false, HelpText = "Read parse tree data from file instead of stdin.")]
        public string File { get; set; }

        [Option('t', "target", Required = false, HelpText = "The target language for the project.")]
        public TargetType? target { get; set; }

        [Option("template-sources-directory", Required = false)]
        public string template_sources_directory { get; set; }

        [Option('o', "output-directory", Required = false, HelpText = "The output directory for the project.")]
        public string output_directory { get; set; }

        [Option('v', "verbose", Required = false)]
        public bool Verbose { get; set; }

	[Option("version", Required = false)]
	public string Version { get; set; } = "0.23.39";
    }
}
