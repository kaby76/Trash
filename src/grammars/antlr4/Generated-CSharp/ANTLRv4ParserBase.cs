using System;
using System.Collections.Generic;
using System.IO;
using Antlr4.Runtime;

public abstract class ANTLRv4ParserBase : Parser
{
	protected ANTLRv4ParserBase(ITokenStream input)
			: base(input)
	{
	}

	protected ANTLRv4ParserBase(ITokenStream input, TextWriter output, TextWriter errorOutput)
			: base(input, output, errorOutput)
	{
	}

	protected bool AllowParserRules()
	{
		return true;
	}
}
