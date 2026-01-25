using CommandLine;
using System.Collections.Generic;

namespace Trash;

public class Config
{
    [Value(0)] public IEnumerable<string> Files { get; set; }

    [Option('c', "columns", Required = false, HelpText = "Columns to output.")]
    public string Columns { get; set; } = "FdriTkmfaetc";

    [Option('i', "input", Required = false, HelpText = "Parse input string.")]
    public string Input { get; set; }

    [Option('h', "header-names", Required = false, HelpText = "Output header names.")]
    public bool HeaderNames { get; set; } = false;

    [Option('m', "heat-map ", Required = false, HelpText = "Output heat map.")]
    public bool HeatMap { get; set; } = false;

    [Option('p', "parser", Required = false,
        HelpText = "Location of pre-built parser (aka the trgen Generated/ directory)")]
    public string ParserLocation { get; set; }

    [Option('t', "type", Required = false, HelpText = "Override type of parse.")]
    public string Type { get; set; }

    [Option('v', "verbose", Required = false)]
    public bool Verbose { get; set; }

    [Option('x', Required = false, HelpText = "Read input file names from stdin.")]
    public bool ReadFileNameStdin { get; set; }

    [Option("version", Required = false)]
    public string Version { get; set; } = "0.23.40";
}
