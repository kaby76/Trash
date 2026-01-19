using CommandLine;
using System.Collections.Generic;

namespace tranalyze
{
    public class Config
    {
        [Option('f', "file", Required = false, HelpText = "Read parse tree data from file instead of stdin.")]
        public string File { get; set; }

        [Option("fmt", Required = false, HelpText = "Output formatted parsing results set.")]
        public bool Format { get; set; }

        [Option('s', "start-rules", Required = false, HelpText = "Start rule names.")]
        public IEnumerable<string> start_rules { get; set; }

        [Option('v', "verbose", Required = false)]
        public bool Verbose { get; set; }

	[Option("version", Required = false)]
	public string Version { get; set; } = "0.23.37";
    }
}
