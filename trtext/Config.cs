using CommandLine;
using System.Collections.Generic;

namespace Trash
{
	public class Config
	{
		[Option("line-number", Required = false)]
		public bool? LineNumber { get; set; }
	}
}
