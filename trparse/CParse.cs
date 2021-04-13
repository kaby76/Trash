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
    class CParse
    {
        public Workspace _workspace { get; set; } = new Workspace();

        public string Help()
        {
            return @"
This program is part of the Trash toolkit.

trparse (antlr2 | antlr3 | antlr4 | bison | ebnf)? <string>
Parse the flie at the top of stack with the given parser type (antlr2, _antlr3, antlr4, bison, etc).

Example:
    trparse <string>
    trparse antlr2 <string>
";
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

            var path = Environment.CurrentDirectory;
            path = path + Path.DirectorySeparatorChar + "Generated";
            if (config.Type != null && config.Type != "gen" || !Directory.Exists(path))
            {
                Dictionary<string, Document> list = new Dictionary<string, Document>();
                Document doc = Docs.Class1.ReadDoc(config.File);
                list.Add(config.File, doc);
                Docs.Class1.ParseDoc(doc, 0, config.Type);
                var pr = ParsingResultsFactory.Create(doc);
                IParseTree pt = pr.ParseTree;
                var serializeOptions = new JsonSerializerOptions();
                serializeOptions.Converters.Add(new AntlrJson.ParseTreeConverter());
                serializeOptions.WriteIndented = true;
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
            else
            {
                var grun = new Grun(config);
                grun.Run();
            }
        }
    }
}
