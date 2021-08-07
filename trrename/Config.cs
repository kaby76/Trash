using System.Collections.Generic;
using CommandLine;

namespace Trash
{
    public class Config
    {
        [Option('f', "file", Required = false)]
        public string File { get; set; }

        [Option('r', "rename-map", Required = true)]
        public string RenameMap { get; set; }
    }
}
