using CommandLine;
using System.Collections.Generic;

namespace Trash
{
    public class Config
    {
        [Option('f', "file", Required = false, HelpText = "The name of an input file to parse.")]
        public string? File { get; set; }
    }
}
