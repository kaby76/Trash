using CommandLine;
using System.Collections.Generic;

namespace Trash;

public class Config
{
    [Option('f', "file", Required = false, HelpText = "Read from file instead of stdin.")]
    public string File { get; set; }

    [Option('v', "verbose", Required = false)]
    public bool Verbose { get; set; }

    [Value(0)] public IEnumerable<string> Files { get; set; }
}
