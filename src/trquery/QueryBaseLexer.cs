using Antlr4.Runtime;
using System.IO;

public abstract class QueryBaseLexer : Lexer
{
	public QueryBaseLexer(ICharStream input, TextWriter output, TextWriter errorOutput) : base(input) { }

	public bool AllowReturns { get; set; }
}
