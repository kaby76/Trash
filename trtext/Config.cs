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

		[Option("line-number", Required = false)]
		public bool? LineNumber { get; set; }
	}
}
