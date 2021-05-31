namespace Trash
{
    using Antlr4.Runtime.Tree;
    using AntlrJson;
    using LanguageServer;
    using System.Collections.Generic;
    using System.IO;
	using System.Text.Json;

	class CDelabel
	{
		public string Help()
		{
			using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("tranalyze.readme.md"))
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
				for (; ; )
				{
					lines = System.Console.In.ReadToEnd();
					if (lines != null && lines != "") break;
				}
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
				var atrees = parse_info.Nodes;
				var parser = parse_info.Parser;
				var lexer = parse_info.Lexer;
				var tokstream = parse_info.Stream;
				var doc = Docs.Class1.CreateDoc(parse_info);
				var res = LanguageServer.Transform.Delabel(doc);
				foreach (var r in res)
				{
					var doc2 = Docs.Class1.CreateDoc(r.Key, r.Value);
					Docs.Class1.ParseDoc(doc2, 10);
					var pr = ParsingResultsFactory.Create(doc2);
					var pt = pr.ParseTree;
					var tuple = new ParsingResultSet()
					{
						Text = doc2.Code,
						FileName = doc2.FullPath,
						Stream = pr.TokStream,
						Nodes = new IParseTree[] { pt },
						Lexer = pr.Lexer,
						Parser = pr.Parser
					};
					results.Add(tuple);
				}
			}
			string js1 = JsonSerializer.Serialize(results.ToArray(), serializeOptions);
			System.Console.WriteLine(js1);
		}
	}
}
