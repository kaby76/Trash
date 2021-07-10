using System;
using Workspaces;
using AntlrJson;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Antlr4.Runtime;

namespace Docs
{
    public class Class1
    {
        public static Workspace _workspace { get; set; } = new Workspace();

        public static int QuietAfter = 10;

        public static void ParseDoc(Document document, int quiet_after, string grammar = null)
        {
            document.Changed = true;
            if (grammar != null) document.ParseAs = grammar;
            var pd = LanguageServer.ParsingResultsFactory.Create(document);
            if (pd != null) pd.QuietAfter = quiet_after;
            var workspace = document.Workspace;
            _ = new LanguageServer.Module().Compile(workspace);
        }
        
        public static Document ReadDoc(string path)
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

        public static Document CreateDoc(AntlrJson.ParsingResultSet parse_info)
        {
            string file_name = parse_info.FileName;
            Document document = new Workspaces.Document(file_name);
            Project project = _workspace.FindProject("Misc");
            if (project == null)
            {
                project = new Project("Misc", "Misc", "Misc");
                _workspace.AddChild(project);
            }
            project.AddDocument(document);
            document.Code = parse_info.Text;
            document.ParseAs = parse_info.Parser.GrammarFileName;
            var pr = LanguageServer.ParsingResultsFactory.Create(document);
            pr.Parser = parse_info.Parser;
            pr.Lexer = parse_info.Lexer;
	        pr.ParseTree = parse_info.Nodes.First();
            pr.TokStream = new CommonTokenStream(pr.Lexer);
            pr.TokStream.Fill();
            return document;
        }
        
        public static Document CreateDoc(string path, string code)
        {
            string file_name = path;
            Document document = _workspace.FindDocument(file_name);
            if (document == null)
            {
                document = new Workspaces.Document(file_name);
                Project project = _workspace.FindProject("Misc");
                if (project == null)
                {
                    project = new Project("Misc", "Misc", "Misc");
                    _workspace.AddChild(project);
                }
                project.AddDocument(document);
            }
            document.Code = code;
            return document;
        }

        public static void EnactEdits(Dictionary<string, string> results)
        {
            if (results.Count > 0)
            {
                foreach (var res in results)
                {
                    if (res.Value == null) continue;
                    var new_doc = CreateDoc(res.Key, res.Value);
                    ParseDoc(new_doc, QuietAfter);
                }
            }
            else
            {
                System.Console.Error.WriteLine("no changes");
            }
        }

    }
}
