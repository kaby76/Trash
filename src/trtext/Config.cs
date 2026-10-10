using CommandLine;

namespace Trash;

public class Config
{
    [Option('f', "file", Required = false, HelpText = "Read parse tree data from file instead of stdin.")]
    public string File { get; set; }

    [Option('v', "verbose", Required = false)]
    public bool Verbose { get; set; }

    [Option('l', "files-with-matches", Required = false, HelpText = "In --text mode, print only names of files with selected nodes.")]
    public bool FilesWithMatches { get; set; }

    [Option('n', "line-number", Required = false, HelpText = "Legacy text-mode option (currently has no effect).")]
    public bool LineNumber { get; set; }

    [Option('L', "files-without-match", Required = false,
        HelpText = "In --text mode, print only names of files with no selected nodes.")]
    public bool FilesWithoutMatch { get; set; }

    [Option('c', "count", Required = false, HelpText = "In --text mode, print the selected-node count per file.")]
    public bool Count { get; set; }

    [Option("bundle", Required = false, HelpText = "Compatibility alias for the default PAX/tar bundle output.")]
    public bool Bundle { get; set; }

    [Option("text", Required = false, HelpText = "Write human-readable reconstructed text instead of a PAX/tar bundle.")]
    public bool Text { get; set; }

    [Option("version", Required = false)]
    public string Version { get; set; } = "4.2.0";
}
