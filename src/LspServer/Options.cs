using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Opt
    {
        public string LanguageId { get; set; }
        public string ParserLocation { get; set; }
        public List<Tuple<string, string>> ClassesAndClassifiers { get; set; }
    }
}
