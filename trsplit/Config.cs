namespace Trash
{
    using CommandLine;

    public class Config
    {
        [Option('f', "file", Required = false)]
        public string File { get; set; }
    }
}
