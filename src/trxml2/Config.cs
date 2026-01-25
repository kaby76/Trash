using CommandLine;
using System.Collections.Generic;

namespace Trash;

public class Config
{
    [Option('v', "verbose", Required = false)]
    public bool Verbose { get; set; }

    [Option("version", Required = false)]
    public string Version { get; set; } = "0.23.41";

    [Value(0)] public IEnumerable<string> Files { get; set; }
}
