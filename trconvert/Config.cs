using CommandLine;

namespace Trash
{
    public class Config
    {
        [Option('f', "file", Required = false)]
        public string File { get; set; }

        [Option('t', "type", Required = false)]
        public string Type { get; set; }

    }
}
