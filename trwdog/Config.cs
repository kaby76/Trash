using System.Collections.Generic;
using CommandLine;

namespace Trash
{
    public class Config
    {
        [Option('t', "timeout", Required = false, HelpText = "Max time in seconds for command to run")]
        public int? Timeout {get; set; }
    }
}
