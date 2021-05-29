using System.Collections.Generic;
using CommandLine;

namespace Trash
{
    public class Config
    {
        [Option('i', "input", Required = false)]
        public string Input { get; set; }

        [Option('f', "file", Required = false)]
        public string File { get; set; }

        [Option('t', "type", Required = false)]
        public string Type { get; set; }
    }
}
