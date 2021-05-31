namespace Trash
{
    using Antlr4.Runtime.Tree;
    using AntlrJson;
    using LanguageServer;
    using System.IO;
    using System.Linq;
    using System.Text.Json;

    class CCombine
    {
        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trcombine.readme.md"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public void Execute(Config config)
        {
            var list = config.Files.ToList();
            var doc1 = Docs.Class1.ReadDoc(list[0]);
            var doc2 = Docs.Class1.ReadDoc(list[1]);
            Docs.Class1.ParseDoc(doc1, 10);
            _ = ParsingResultsFactory.Create(doc1);
            Docs.Class1.ParseDoc(doc2, 10);
            _ = ParsingResultsFactory.Create(doc2);
            var results = LanguageServer.Transform.CombineGrammars(doc1, doc2);
            Docs.Class1.EnactEdits(results);

            var doc = Docs.Class1.CreateDoc(results.First().Key, results.First().Value);
            Docs.Class1.ParseDoc(doc, 10);
            var pr = ParsingResultsFactory.Create(doc);
            var pt = pr.ParseTree;
            var tuple = new ParsingResultSet[1]
                {
                    new ParsingResultSet()
                    {
                        Text = doc.Code,
                        FileName = doc.FullPath,
                        Stream = pr.TokStream,
                        Nodes = new IParseTree[] { pt },
                        Lexer = pr.Lexer,
                        Parser = pr.Parser
                    }
                };
            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new AntlrJson.ParseTreeConverter());
            serializeOptions.WriteIndented = false;
            string js1 = JsonSerializer.Serialize(tuple, serializeOptions);
            System.Console.WriteLine(js1);
        }
    }
}
