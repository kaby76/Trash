using CommandLine;
using System.Collections.Generic;

namespace Trash
{
    public class Config
    {
        [Option('f', "file", Required = false)]
        public string File { get; set; }

        [Option('v', "verbose", Required = false)]
        public bool Verbose { get; set; }

        [Value(0, Min = 1)]
        public IEnumerable<string> Expr { get; set; }
    }
}
