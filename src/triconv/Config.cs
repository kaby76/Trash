using CommandLine;
using System.Collections.Generic;

namespace Trash;

public class Config
{
    [Option('f', "from-code", Required = false)]
    public string FromCode { get; set; }

    [Option('t', "to-code", Required = false)]
    public string ToCode { get; set; }

    [Option("version", Required = false)]
    public string Version { get; set; } = "0.23.35";

    [Value(0)] public IEnumerable<string> Files { get; set; }
}
