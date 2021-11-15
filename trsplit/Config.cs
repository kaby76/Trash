namespace Trash
{
    using CommandLine;

    public class Config
    {
        [Option('f', "file", Required = false)]
        public string File { get; set; }

        [Option('v', "verbose", Required = false)]
        public bool Verbose { get; set; }
    }
}
