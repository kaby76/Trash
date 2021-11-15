using System.Collections.Generic;
using CommandLine;

namespace Trash
{
    public class Config
    {
        [Option('f', "file", Required = false)]
        public string File { get; set; }

        [Option('a', "after", Required = false, Default = false)]
        public bool After { get; set; }

        [Option('v', "verbose", Required = false)]
        public bool Verbose { get; set; }

        [Value(0, Min = 1)]
        public IEnumerable<string> Expr { get; set; }
    }
}
