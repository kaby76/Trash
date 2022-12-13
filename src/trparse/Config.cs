using System.Collections.Generic;
using CommandLine;

namespace Trash
{
    public class Config
    {
        [Value(0)]
        public IEnumerable<string> Files { get; set; }

        [Option('i', "input", Required = false)]
        public string Input { get; set; }

        [Option('t', "type", Required = false)]
        public string Type { get; set; }

        [Option('s', "start-rule", Required = false, HelpText = "Start rule name.")]
        public string start_rule { get; set; }

        [Option('p', "parser", Required = false, HelpText = "Location of pre-built parser (aka the trgen Generated/ directory)")]
        public string ParserLocation { get; set; }

        [Option('e', "no-prs", Required = false, HelpText = "Output parse errors only. No parsing result sets.")]
        public bool NoParsingResultSets { get; set; }

        [Option('g', "encoding", Required = false, HelpText = "Set the encoding for the grammar.")]
        public string Encoding { get; set; }

        [Option('q', "--quiet", Required =false, Default = false, HelpText = "Do not output anything; only set error code.")]
        public bool Quiet { get; set; }

        [Option('v', "verbose", Required = false)]
        public bool Verbose { get; set; }

        [Option('x', Required = false)]
        public bool ReadFileNameStdin { get; set; }

        [Option('d', "dll", Required = false, HelpText = "Search for parser in dll with this specified name.")]
        public string Dll { get; set; } = "Test";
    }
}
