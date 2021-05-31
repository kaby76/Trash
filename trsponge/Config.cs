using CommandLine;
using System.Collections.Generic;

namespace Trash
{
    public class Config
    {
        [Option('o', "overwrite", Required = false)]
        public bool? Overwrite { get; set; }
    }
}
