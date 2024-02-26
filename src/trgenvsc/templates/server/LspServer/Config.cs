using CommandLine;
using System.Collections.Generic;

namespace Server
{
    public class Config
    {
        [Value(0)] public IEnumerable<string> Files { get; set; }

        [Option("fmt", Required = false, HelpText = "Output formatted parsing results set.")]
        public bool Format { get; set; }

        [Option('t', "type", Required = false)]
        public string Type { get; set; }

        [Option('s', "start-rule", Required = false, HelpText = "Start rule name.")]
        public string start_rule { get; set; }

        [Option('p', "parser", Required = false,
            HelpText = "Location of pre-built parser (aka the trgen Generated/ directory)")]
        public string ParserLocation { get; set; }

        [Option('v', "verbose", Required = false)]
        public bool Verbose { get; set; }

        [Option('d', "dll", Required = false, HelpText = "Search for parser in dll with this specified name.")]
        public string Dll { get; set; } = "Test";
    }
}
