using CommandLine;

namespace Trash;

public class Config
{
    [Option('f', "file", Required = false, HelpText = "Read parse tree data from file instead of stdin.")]
    public string File { get; set; }

    [Option('v', "verbose", Required = false)]
    public bool Verbose { get; set; }

    [Option('l', "files-with-matches", Required = false, HelpText = "print only names of FILEs with selected lines")]
    public bool FilesWithMatches { get; set; }

    [Option('n', "line-number", Required = false, HelpText = "print line number with output lines")]
    public bool LineNumber { get; set; }

    [Option('L', "files-without-match", Required = false,
        HelpText = "print only names of FILEs with no selected lines")]
    public bool FilesWithoutMatch { get; set; }

    [Option('c', "count", Required = false, HelpText = "print only a count of selected lines per FILE")]
    public bool Count { get; set; }
}
