using CommandLine;
using System.Collections.Generic;

namespace Trash
{
    public class Config
    {
        [Option('f', "file", Required = false)]
        public string File { get; set; }

        [Option('o', "overwrite", Required = false)]
        public bool? Overwrite { get; set; }
    }
}
