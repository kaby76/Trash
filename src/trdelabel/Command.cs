namespace Trash
{
    using Antlr4.Runtime.Tree;
    using AntlrJson;
    using System.Collections.Generic;
    using System.IO;
	using System.Text.Json;
	using System.Linq;

	class Command
	{
		public string Help()
		{
			using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trdelabel.readme.md"))
			using (StreamReader reader = new StreamReader(stream))
			{
				return reader.ReadToEnd();
			}
		}

		public void Execute(Config config)
		{
			string lines = null;
			if (!(config.File != null && config.File != ""))
			{
				if (config.Verbose)
				{
					System.Console.Error.WriteLine("reading from file >>>" + config.File + "<<<");
				}
				for (; ; )
				{
					lines = System.Console.In.ReadToEnd();
					if (lines != null && lines != "") break;
				}
				lines = lines.Trim();
			}
			else
			{
				lines = File.ReadAllText(config.File);
			}
			var serializeOptions = new JsonSerializerOptions();
			serializeOptions.Converters.Add(new AntlrJson.ParseTreeConverter());
			serializeOptions.WriteIndented = true;
			var data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
			var results = new List<ParsingResultSet>();
			foreach (var parse_info in data)
			{
				var text = parse_info.Text;
				var fn = parse_info.FileName;
				var atrees = parse_info.Nodes.Select(t => t as AltAntlr.MyParserRuleContext).ToList();
				var parser = parse_info.Parser as AltAntlr.MyParser;
				var lexer = parse_info.Lexer as AltAntlr.MyLexer;
				var tokstream = parse_info.Stream as AltAntlr.MyTokenStream;
				LanguageServer.Transform.Delabel(parser, lexer, tokstream, atrees);
				var tuple = new ParsingResultSet()
				{
					Text = tokstream.Text,
					FileName = fn,
					Stream = tokstream,
					Nodes = atrees.ToArray(),
					Lexer = lexer,
					Parser = parser,
				};
				results.Add(tuple);
			}
			string js1 = JsonSerializer.Serialize(results.ToArray(), serializeOptions);
			System.Console.WriteLine(js1);
		}
	}
}
