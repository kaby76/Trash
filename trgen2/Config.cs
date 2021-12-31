using CommandLine;
using System.Collections.Generic;

namespace Trash
{
    public class Config
    {
        [Option('f', "file", Required = false)]
        public string File { get; set; }

        [Option('v', "verbose", Required = false)]
        public bool Verbose { get; set; }


        [Option("all_source_pattern", Required = false, HelpText = "R.E. for all source files to use.")]
        public string all_source_pattern { get; set; }

        [Option('c', "config", Required = false, HelpText = "The name of the template parameterization file to read.")]
        public string InputFile { get; set; }

        [Option('o', "output-directory", Required = false, HelpText = "The output directory for the project.")]
        public string output_directory { get; set; }

	    [Option('s', "templates", Required = false)]
        public string templates { get; set; }

        [Option('t', "target", Required = true, HelpText = "The template to instantiate.")]
        public string template { get; set; }
    }
}
