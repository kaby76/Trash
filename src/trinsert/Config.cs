using System.Collections.Generic;
using CommandLine;

namespace Trash
{
    public class Config
    {
        [Option('f', "file", Required = false, HelpText = "Read parse tree data from file instead of stdin.")]
        public string File { get; set; }

        [Option("fmt", Required = false, HelpText = "Output formatted parsing results set.")]
        public bool Format { get; set; }

        [Option('a', "after", Required = false, Default = false)]
        public bool After { get; set; }

        [Option('t', "as-tree", Required = false, Default = false)]
        public bool AsTree { get; set; }

        [Option('v', "verbose", Required = false)]
        public bool Verbose { get; set; }

        [Value(0, Min = 1)]
        public IEnumerable<string> Expr { get; set; }
    }
}
