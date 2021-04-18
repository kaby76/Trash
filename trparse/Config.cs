using System.Collections.Generic;
using CommandLine;

namespace Trash
{
    public class Config
    {
        [Option('i', "input")]
        public string Input { get; set; }

        [Option('f', "file", Required = true)]
        public string File { get; set; }

        [Option('t', "type", Required = false, HelpText = "If Generated/ exists, then the default is to run a parser defined in the directory. To parse a file as a grammar, use this option, one of 'antlr4', 'antlr3', 'antlr2', ...")]
        public string? Type { get; set; }
    }
}
