using System.IO;

namespace xpath.org.eclipse.wst.xml.xpath2.processor.@internal
{
    using Antlr4.Runtime;

    public abstract class XPath31BaseLexer : Lexer
    {
        public XPath31BaseLexer(ICharStream input, TextWriter output, TextWriter errorOutput) : base(input) { }

        public bool AllowReturns { get; set; }
    }
}
