using CommandLine;
using System.Collections.Generic;
using Newtonsoft.Json;
using org.eclipse.wst.xml.xpath2.processor.@internal.function;

namespace Trash
{
	public class Config
	{
		[Option('f', "file", Required = false)]
		public string File { get; set; }

		[Option('p', "prefix", Required = false)]
        public bool Prefix { get; set; }

        [Option('v', "verbose", Required = false)]
		public bool Verbose { get; set; }
    }
}
