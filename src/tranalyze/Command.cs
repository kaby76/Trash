namespace Trash
{
    using LanguageServer;
    using System.Collections;
    using System.IO;
    using System.Text.Json;

    class Command
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
                if (config.Verbose)
                {
                    System.Console.Error.WriteLine("reading from stdin");
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
                if (config.Verbose)
                {
                    System.Console.Error.WriteLine("reading from file >>>" + config.File + "<<<");
                }
                lines = File.ReadAllText(config.File);
            }
            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new AntlrJson.ParsingResultSetSerializer());
            serializeOptions.WriteIndented = false;
            serializeOptions.MaxDepth = 10000;
            AntlrJson.ParsingResultSet[] data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
            foreach (AntlrJson.ParsingResultSet parse_info in data)
            {
                var doc = Docs.Class1.CreateDoc(parse_info);
                var f = doc.FullPath;
                doc.ParseTree = null;
                doc.Changed = true;
                ParsingResults ref_pd = ParsingResultsFactory.Create(doc);
                ref_pd.ParseTree = null;
                _ = new Module().GetQuickInfo(0, doc);
                AnalyzeDoc(doc, config.start_rules);
            }
        }

        public void AnalyzeDoc(Workspaces.Document document, System.Collections.Generic.IEnumerable<string> start_rules)
        {
            _ = ParsingResultsFactory.Create(document);
            var results = LanguageServer.Analysis.PerformAnalysis(document, start_rules);
            foreach (var r in results)
            {
                System.Console.Write((r.Start != 0 ? r.Start + " " : "") + r.Message);
                System.Console.WriteLine();
            }
        }
    }
}
