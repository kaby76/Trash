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

        public void ParseDoc(Document document, int quiet_after, string grammar = null)
        {
            document.Changed = true;
            document.ParseAs = grammar;
            var pd = ParsingResultsFactory.Create(document);
            pd.QuietAfter = quiet_after;
            var workspace = document.Workspace;
            _ = new LanguageServer.Module().Compile(workspace);
        }

        public Document ReadDoc(string path)
        {
            string file_name = path;
            Document document = _workspace.FindDocument(file_name);
            if (document == null)
            {
                throw new Exception("File does not exist.");
            }
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(file_name))
                {
                    // Read the stream to a string, and write the string to the console.
                    string str = sr.ReadToEnd();
                    document.Code = str;
                }
            }
            catch (IOException)
            {
                throw;
            }
            return document;
        }

        public void Help()
        {
            System.Console.WriteLine(@"
This program is part of the Trash toolkit.

trparse (antlr2 | antlr3 | antlr4 | bison | ebnf)? <string>
Parse the flie at the top of stack with the given parser type (antlr2, _antlr3, antlr4, bison, etc).

Example:
    trparse <string>
    trparse antlr2 <string>
");
        }

        public void Execute(Config config)
        {
            Dictionary<string, Document> list = new Dictionary<string, Document>();
            foreach (var f in config.Grammars)
            {
                Document doc = ReadDoc(f);
                list.Add(f, doc);
            }
            foreach (var p in list)
            {
                var doc = p.Value;
                ParseDoc(doc, 0, config.Type);
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
        }
    }
}
