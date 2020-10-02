namespace Trash
{
    using LanguageServer;
    using System;
    using System.IO;
    using Workspaces;

    class Docs
    {
        private Repl _repl;

        public Docs(Repl repl)
        {
            _repl = repl;
        }

        public void ParseDoc(Document document, int quiet_after, string grammar = null)
        {
            document.Changed = true;
            document.ParseAs = grammar;
            var pd = ParsingResultsFactory.Create(document);
            pd.QuietAfter = quiet_after;
            var workspace = document.Workspace;
            _ = new LanguageServer.Module().Compile(workspace);
        }

        public Document CreateDoc(string path, string code)
        {
            string file_name = path;
            Document document = _repl._workspace.FindDocument(file_name);
            if (document == null)
            {
                document = new Workspaces.Document(file_name);
                Project project = _repl._workspace.FindProject("Misc");
                if (project == null)
                {
                    project = new Project("Misc", "Misc", "Misc");
                    _repl._workspace.AddChild(project);
                }
                project.AddDocument(document);
            }
            document.Code = code;
            return document;
        }

        public Document ReadDoc(string path)
        {
            string file_name = path;
            Document document = _repl._workspace.FindDocument(file_name);
            if (document == null)
            {
                document = new Workspaces.Document(file_name);
                try
                {   // Open the text file using a stream reader.
                    using (StreamReader sr = new StreamReader(file_name))
                    {
                        // Read the stream to a string, and write the string to the console.
                        string str = sr.ReadToEnd();
                        document.Code = str;
                    }
                }
                catch (IOException eeks)
                {
                    throw eeks;
                }
                Project project = _repl._workspace.FindProject("Misc");
                if (project == null)
                {
                    project = new Project("Misc", "Misc", "Misc");
                    _repl._workspace.AddChild(project);
                }
                project.AddDocument(document);
            }
            return document;
        }

        public void WriteDoc(Document document)
        {
            if (document != null)
            {
                var file_name = document.FullPath;
                try
                {   // Open the text file using a stream reader.
                    using (StreamWriter sw = new StreamWriter(file_name))
                    {
                        sw.Write(document.Code);
                    }
                    System.Console.Error.WriteLine("Written to file " + file_name);
                }
                catch (Exception e)
                {
                    System.Console.Error.WriteLine("Failed to write file " + e.Message);
                }
            }
        }

        public void AnalyzeDoc(Document document)
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
