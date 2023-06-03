using CommandLine;
using System.Collections.Generic;

namespace Trash
{
    public class Config
    {
        [Value(0)]
        public IEnumerable<string> Files { get; set; }

        [Option('d', "dll", Required = false, HelpText = "Search for parser in dll with this specified name.")]
        public string Dll { get; set; } = "Test";

        [Option('f', "file", Required = false, HelpText = "Read parse tree data from file instead of stdin.")]
        public string File { get; set; }

        [Option('i', "input", Required = false, HelpText = "Parse input string.")]
        public string Input { get; set; }

        [Option('p', "parser", Required = false, HelpText = "Location of pre-built parser (aka the trgen Generated/ directory)")]
        public string ParserLocation { get; set; }

        [Option('q', "quiet", Required = false, Default = false, HelpText = "Do not output anything; only set error code.")]
        public bool Quiet { get; set; }

        [Option('t', "type", Required = false, HelpText = "Override type of parse.")]
        public string Type { get; set; }

        [Option('v', "verbose", Required = false)]
        public bool Verbose { get; set; }

        [Option('x', Required = false, HelpText = "Read input file names from stdin.")]
        public bool ReadFileNameStdin { get; set; }
    }
}
