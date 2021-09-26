using System.Collections.Generic;
using CommandLine;

namespace Trash
{
    public class Config
    {
        [Option('f', "file", Required = false)]
        public string File { get; set; }

        [Value(0)]
        public IEnumerable<string> Expr { get; set; }
    }
}
