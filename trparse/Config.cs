using System.Collections.Generic;
using CommandLine;

namespace Trash
{
    public class Config
    {
        [Option('i', "input")]
        public string Input { get; set; }

        [Option('f', "file")]
        public string File { get; set; }

        [Option("type", Required = false)]
        public string? Type { get; set; }
    }
}
