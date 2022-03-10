using Antlr4.Runtime.Tree;
using AntlrJson;
using LanguageServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Workspaces;

namespace Trash
{
    class Command
    {
        public Workspace _workspace { get; set; } = new Workspace();

        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trparse.readme.md"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public void Execute(Config config)
        {
            // There are two ways to do this. One is a
            // bootstrapped method using LanguageServer, the
            // other is by using the generated code, with the loading
            // and running of the parser. We need to determine which way.
            // If Generated/exists, and it's a CSharp program that compiles,
            // use that to parse the input.
            //
            // If Generated/ does not exist, then parse as Antlr4.
            // If Type=="gen", then parse using Generated/.
            // If Type=="antlr2", then parse using Antlr2.
            // Etc.

            var path = Environment.CurrentDirectory + Path.DirectorySeparatorChar;
            var full_path = path + "Generated/bin/Debug/net6.0/";
            var exists = File.Exists(full_path + "Test.dll");
            if (!exists)
            {
                full_path = path + "bin/Debug/net6.0/";
                exists = File.Exists(full_path + "Test.dll");
            }

            if (config.Type != null && config.Type != "gen" || !exists)
            {
                Dictionary<string, Document> list = new Dictionary<string, Document>();
                var serializeOptions = new JsonSerializerOptions();
                serializeOptions.Converters.Add(new AntlrJson.ParseTreeConverter());
                serializeOptions.WriteIndented = true;
                var results = new List<ParsingResultSet>();
                foreach (var file in config.Files)
                {
                    Document doc = Docs.Class1.ReadDoc(file);
                    list.Add(file, doc);
                    Docs.Class1.ParseDoc(doc, 10, config.Type);
                    var pr = ParsingResultsFactory.Create(doc);
                    IParseTree pt = pr.ParseTree;
                    var rel_path = Path.GetRelativePath(Environment.CurrentDirectory, doc.FullPath);
                    var tuple = new ParsingResultSet()
                    {
                        Text = doc.Code,
                        FileName = rel_path,
                        Stream = pr.TokStream,
                        Nodes = new IParseTree[] { pt },
                        Lexer = pr.Lexer,
                        Parser = pr.Parser,
                        StartSymbol = "",
                        MetaStartSymbol = ""
                    };
                    results.Add(tuple);
                }
                string js1 = JsonSerializer.Serialize(results.ToArray(), serializeOptions);
                System.Console.WriteLine(js1);
            }
            else
            {
                var grun = new Grun(config);
                grun.Run();
            }
        }
    }
}
