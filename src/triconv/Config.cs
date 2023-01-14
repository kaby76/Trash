using System.Collections.Generic;
using CommandLine;

namespace Trash
{
    public class Config
    {
        [Option('f', "from-code", Required = false)]
        public string FromCode { get; set; }

        [Option('t', "to-code", Required = false)]
        public string ToCode { get; set; }

        [Value(0)]
        public IEnumerable<string> Files { get; set; }
    }
}
