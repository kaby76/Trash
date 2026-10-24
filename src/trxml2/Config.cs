using CommandLine;
using System.Collections.Generic;

namespace Trash;

public class Config
{
    [Option('v', "verbose", Required = false)]
    public bool Verbose { get; set; }

    [Option("version", Required = false)]
    public string Version { get; set; } = "4.8.0";

    [Option("file-name", Required = false, HelpText = "Prefix each output line with the input file name.")]
    public bool FileName { get; set; }

    [Value(0)] public IEnumerable<string> Files { get; set; }
}
