using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trash
{
	public class Def
	{
		public string Name { get; set; }
		public string ToVSCodeName { get; set; }
		public string Classifier { get; set; }
		public string NodeName { get; set; }
	}
	
	public class Sem
	{
		public string Suffix { get; set; }
		public string ParserLocation { get; set; }
		public List<Def> Classes { get; set; }
	}
}
