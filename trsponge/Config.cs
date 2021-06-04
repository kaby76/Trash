using CommandLine;
using System.Collections.Generic;

namespace Trash
{
    public class Config
    {
        [Option('f', "file", Required = false)]
        public string File { get; set; }

        [Option('c', "clobber", Required = false)]
        public bool? Clobber { get; set; }

        [Option('o', "output-directory", Required = false)]
        public string OutputDirectory { get; set; }
    }
}
