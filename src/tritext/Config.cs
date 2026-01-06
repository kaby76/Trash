using CommandLine;
using System.Collections.Generic;

namespace Trash;

public class Config
{
    [Value(0)] public IEnumerable<string> Files { get; set; }

    [Option('v', "verbose", Required = false)]
    public bool Verbose { get; set; }

    [Option('m', "markup", Required = false)]
    public bool OutputMarkup { get; set; }

    [Option('p', "pre", Required = false)] public bool OutputPreflight { get; set; }

    [Option('f', "filter", Required = false)]
    public string Filter { get; set; } = "";

    [Option("version", Required = false)]
    public string Version { get; set; } = "0.23.34";
}
