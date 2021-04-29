namespace Trash
{
    using Antlr4.Runtime.Tree;
    using AntlrJson;
    using LanguageServer;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    
    class CAnalyze
    {
        public string Help()
        {
            return @"analyze
Trash can perform an analysis of a grammar. The analysis includes a count of symbol
type, cycles, and unused symbols.

Example:
    analyze
";
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
            serializeOptions.WriteIndented = false;
            AntlrJson.ParsingResultSet parse_info = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet>(lines, serializeOptions);
            var doc = Docs.Class1.CreateDoc(parse_info);
            var f = doc.FullPath;
            doc.ParseTree = null;
            doc.Changed = true;
            ParsingResults ref_pd = ParsingResultsFactory.Create(doc);
            ref_pd.ParseTree = null;
            _ = new Module().GetQuickInfo(0, doc);
            AnalyzeDoc(doc);
        }

		public void AnalyzeDoc(Workspaces.Document document)
		{
			_ = ParsingResultsFactory.Create(document);
			var results = LanguageServer.Analysis.PerformAnalysis(document);
			foreach (var r in results)
			{
				System.Console.Write(r.Document + " " + r.Severify + " " + r.Start + " " + r.Message);
				System.Console.WriteLine();
			}
		}
    }
}
