using LanguageServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Workspaces;


namespace UnitTestProject1
{
    class Setup
    {
        public static Document OpenAndParse(string code)
        {
            Workspace _workspace = new Workspace();
            Document document = Document.CreateStringDocument(code);

            // The Kleene rewrite currently produces
            // xx : 'a' * 'a' ;
            // yy: 'b' 'b' * ;
            // zz: 'a' * ( | 'a');
            // z2: ( | 'b') 'b' * ;
            //
            // This should be instead
            // xx : 'a' + ;
            // yy : 'b' + ;
            // zz : 'a' * ;
            // z2 : 'b' * ;

            _ = ParsingResultsFactory.Create(document);
            var workspace = document.Workspace;
            _ = new LanguageServer.Module().Compile(workspace);
            Project project = _workspace.FindProject("Misc");
            if (project == null)
            {
                project = new Project("Misc", "Misc", "Misc");
                _workspace.AddChild(project);
            }
            project.AddDocument(document);
            var pr = LanguageServer.ParsingResultsFactory.Create(document);
            if (document.ParseTree == null)
            {
                new LanguageServer.Module().Compile(_workspace);
            }
            return document;
        }
    }
}
