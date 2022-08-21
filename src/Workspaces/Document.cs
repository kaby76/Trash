namespace Workspaces
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    public class Document : Container
    {
        private string _contents;
        private readonly Dictionary<string, string> _properties = new Dictionary<string, string>();
        private readonly Dictionary<string, bool> _lazy_evaluated = new Dictionary<string, bool>();
        private string _parse_as;

        public bool Changed
        {
            get;
            set;
        }
        public string Code
        {
            get
            {
                if (_contents == null)
                {
                    try
                    {
                        StreamReader sr = new StreamReader(FullPath);
                        _contents = sr.ReadToEnd();
                        Changed = true;
                    }
                    catch {}
                }
                return _contents;
            }
            set
            {
                if (_contents == value)
                {
                    return;
                }
                Changed = true;
                ParseTree = null;
                Indices = null;
                _contents = value;
            }
        }
        public Dictionary<TerminalNodeImpl, int> Defs
        {
            get;
            set;
        }
        public string Encoding { get; set; }
        public string FullPath { get; private set; }
        public int[] Indices { get; set; }
        public string ParseAs { get; set; }
        public Dictionary<TerminalNodeImpl, int> Refs
        {
            get;
            set;
        }

        public Document(string ffn, string encoding = "utf8")
        {
            FullPath = Util.GetProperFilePathCapitalization(ffn);
            Encoding = encoding;
            Changed = false;
        }

        public void AddProperty(string name, string value)
        {
            _properties[name] = value;
            _lazy_evaluated[name] = true;
        }

        public void AddProperty(string name)
        {
            _properties[name] = null;
            _lazy_evaluated[name] = false;
        }

        public override Document FindDocument(string ffn)
        {
            var normalized_ffn = Util.GetProperFilePathCapitalization(ffn);
            Debug.Assert(this.FullPath == Util.GetProperFilePathCapitalization(this.FullPath));

            if (normalized_ffn == this.FullPath)
            {
                return this;
            }

            return null;
        }

        public IParseTree ParseTree
        {
            get; set;
        }

        public Parser Parser
        {
            get; set;
        }

        public Lexer Lexer
        {
            get; set;
        }

        public string GetProperty(string name)
        {
            if (name is null)
            {
                throw new System.ArgumentNullException(nameof(name));
            }
            return null;
        }

        static Random random_number_generator = new Random();

        public static Document CreateStringDocument(string input)
        {
            var random_number = random_number_generator.Next();
            string file_name = "Dummy" + random_number + ".g4";
            Document document = Workspaces.Workspace.Instance.FindDocument(file_name);
            if (document == null)
            {
                document = new Workspaces.Document(file_name);
                document.Code = input;
                Project project = Workspaces.Workspace.Instance.FindProject("Misc");
                if (project == null)
                {
                    project = new Project("Misc", "Misc", "Misc");
                    Workspaces.Workspace.Instance.AddChild(project);
                }
                project.AddDocument(document);
            }
            document.Changed = true;
            return document;
        }
    }
}
