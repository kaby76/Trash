// Template generated code from trgen 0.13.8

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using Workspaces;
using LanguageServer;

public class Program
{
    public static Document CheckDoc(string path)
    {
        string file_name = path;
        Document document = Workspaces.Workspace.Instance.FindDocument(file_name);
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
            catch (IOException)
            {
            }
            Project project = Workspaces.Workspace.Instance.FindProject("Misc");
            if (project == null)
            {
                project = new Project("Misc", "Misc", "Misc");
                Workspaces.Workspace.Instance.AddChild(project);
            }
            project.AddDocument(document);
        }
        document.Changed = true;
        _ = ParsingResultsFactory.Create(document);
        var workspace = document.Workspace;
        _ = new LanguageServer.Module().Compile(workspace);
        return document;
    }
}
