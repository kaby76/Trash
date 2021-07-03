namespace Trash
{
    using LanguageServer;
    using System.IO;
    using System.Text.Json;

    class CAnalyze
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
            serializeOptions.WriteIndented = false;
            AntlrJson.ParsingResultSet[] data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
            foreach (var parse_info in data)
            {
                var doc = Docs.Class1.CreateDoc(parse_info);
                var f = doc.FullPath;
                doc.ParseTree = null;
                doc.Changed = true;
                ParsingResults ref_pd = ParsingResultsFactory.Create(doc);
                ref_pd.ParseTree = null;
                _ = new Module().GetQuickInfo(0, doc);
                AnalyzeDoc(doc);
            }
        }

        public void AnalyzeDoc(Workspaces.Document document)
        {
            _ = ParsingResultsFactory.Create(document);
            var results = LanguageServer.Analysis.PerformAnalysis(document);
            foreach (var r in results)
            {
                System.Console.Write((r.Start != 0 ? r.Start + " " : "") + r.Message);
                System.Console.WriteLine();
            }
        }
    }
}
