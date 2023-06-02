using CommandLine;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Newtonsoft.Json;
using org.eclipse.wst.xml.xpath2.processor.@internal.function;

namespace Trash
{
    public class Config
    {
        [Option('f', "file", Required = false, HelpText = "Read parse tree data from file instead of stdin.")]
        public string File { get; set; }

        [Option('H', "with-filename", Required = false, HelpText = "Print the file name for each match.")]
        public bool WithFileName { get; set; }
                
        [Option('p', "prefix", Required = false)]
        public bool Prefix { get; set; }

        [Option('v', "verbose", Required = false)]
        public bool Verbose { get; set; }
    }
}
