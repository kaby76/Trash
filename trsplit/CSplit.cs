namespace Trash
{
    using Antlr4.Runtime.Tree;
    using AntlrJson;
    using LanguageServer;
    using System.IO;
    using System.Linq;
    using System.Text.Json;

    class CSplit
    {
        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trsplit.readme.md"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public void Execute(Config config)
        {
            var file = config.File;
            var doc_in = Docs.Class1.ReadDoc(file);
            Docs.Class1.ParseDoc(doc_in, 10);
            _ = ParsingResultsFactory.Create(doc_in);
            var results = LanguageServer.Transform.SplitGrammar(doc_in);
            Docs.Class1.EnactEdits(results);
            foreach (var r in results)
            {
                var doc = Docs.Class1.CreateDoc(results.First().Key, results.First().Value);
                Docs.Class1.ParseDoc(doc, 10);
                var pr = ParsingResultsFactory.Create(doc);
                var pt = pr.ParseTree;
                var tuple = new ParsingResultSet()
                {
                    Text = doc.Code,
                    FileName = doc.FullPath,
                    Stream = pr.TokStream,
                    Nodes = new IParseTree[] { pt },
                    Lexer = pr.Lexer,
                    Parser = pr.Parser
                };
                var serializeOptions = new JsonSerializerOptions();
                serializeOptions.Converters.Add(new AntlrJson.ParseTreeConverter());
                serializeOptions.WriteIndented = false;
                string js1 = JsonSerializer.Serialize(tuple, serializeOptions);
                System.Console.WriteLine(js1);
            }
        }
    }
}
