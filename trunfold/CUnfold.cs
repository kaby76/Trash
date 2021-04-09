namespace Trash
{
	using Antlr4.Runtime;
	using Antlr4.Runtime.Tree;
	using AntlrJson;
	using LanguageServer;
	using org.eclipse.wst.xml.xpath2.processor.util;
	using System.Linq;
	using System.Text.Json;

	class CUnfold
	{
		public void Help()
		{
			System.Console.WriteLine(@"
This program is part of the Trash toolkit.

trunfold <string>
The unfold command applies the unfold transform to a collection of terminal nodes
in the parse tree, which is identified with the supplied xpath expression. Prior
to using this command, you must have the file parsed. An unfold operation substitutes
the right-hand side of a parser or lexer rule into a reference of the rule name that
occurs at the specified node. The resulting code is parsed and placed on the top of
stack.

Example:
    trparse A.g4 | trunfold ""//parserRuleSpec//labeledAlt//RULE_REF[text() = 'markerAnnotation']""
");
		}

		public void Execute(Config config)
		{
			var expr = config.Expr.First();
			System.Console.Error.WriteLine("Expr = '" + expr + "'");
			string lines = null;
			for (; ; )
			{
				lines = System.Console.In.ReadToEnd();
				if (lines != null && lines != "") break;
			}
			var serializeOptions = new JsonSerializerOptions();
			serializeOptions.Converters.Add(new AntlrJson.ParseTreeConverter());
			serializeOptions.WriteIndented = false;
			AntlrJson.ParsingResultSet parse_info = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet>(lines, serializeOptions);
			var text = parse_info.Text;
			var fn = parse_info.FileName;
			var atrees = parse_info.Nodes;
			var parser = parse_info.Parser;
			var lexer = parse_info.Lexer;
			var tokstream = parse_info.Stream;
			var doc = Docs.Class1.CreateDoc(parse_info);
			org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
			IParseTree root = atrees.First().Root();
			var ate = new AntlrTreeEditing.AntlrDOM.ConvertToDOM();
			using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = ate.Try(root, parser))
			{
				var nodes = engine.parseExpression(expr,
						new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
					.Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree as TerminalNodeImpl).ToList();
				var results = LanguageServer.Transform.Unfold(nodes, doc);
				Docs.Class1.EnactEdits(results);

				var pr = ParsingResultsFactory.Create(doc);
				IParseTree pt = pr.ParseTree;
				var tuple = new ParsingResultSet()
				{
					Text = doc.Code,
					FileName = doc.FullPath,
					Stream = pr.TokStream,
					Nodes = new IParseTree[] { pt },
					Lexer = pr.Lexer,
					Parser = pr.Parser
				};
				string js1 = JsonSerializer.Serialize(tuple, serializeOptions);
				System.Console.WriteLine(js1);
			}
		}
	}
}
