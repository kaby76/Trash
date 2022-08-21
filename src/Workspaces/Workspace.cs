namespace Workspaces
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class Workspace : Container
    {
        private static Workspace _instance;
        private string _name;
        private string _ffn;

        public string FFN
        {
            get => _ffn;
            set => _ffn = value;
        }

        public static Workspace Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Workspace();
                }

                return _instance;
            }
        }

        public IEnumerable<Document> AllDocuments()
        {
            HashSet<Container> visited = new HashSet<Container>();
            Stack<Container> stack = new Stack<Container>();
            stack.Push(this);
            while (stack.Any())
            {
                Container current = stack.Pop();
                if (visited.Contains(current))
                {
                    continue;
                }

                visited.Add(current);
                if (current is Document)
                {
                    yield return current as Document;
                }
                else
                {
                    foreach (Container c in this.Children)
                    {
                        stack.Push(c);
                    }
                }
            }
        }

        public override Document FindDocument(string ffn)
        {
            if (ffn == null)
            {
                return null;
            }

            foreach (Container container in this.Children)
            {
                Document found = container.FindDocument(ffn);
                if (found != null)
                {
                    return found;
                }
            }
            // If no document found, try to create it.
            if (File.Exists(ffn))
            {
                Document document = new Workspaces.Document(ffn);
                AddChild(document);
                return document;
            }
            return null;
        }

        public override Document ReadDocument(string ffn)
        {
            var document = FindDocument(ffn);
            if (document != null)
            {
                try
                {   // Open the text file using a stream reader.
                    using (StreamReader sr = new StreamReader(ffn))
                    {
                        // Read the stream to a string, and write the string to the console.
                        string str = sr.ReadToEnd();
                        document.Code = str;
                    }
                }
                catch (IOException)
                {
                    return null;
                }
            }
            return document;
        }

        public override Project FindProject(string ffn)
        {
            foreach (Container container in this.Children)
            {
                Project found = container.FindProject(ffn);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }

        public override Project FindProject(string canonical_name, string name, string ffn)
        {
            foreach (Container doc in this.Children)
            {
                Project found = doc.FindProject(canonical_name, name, ffn);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }
      
        public static Workspace Initialize(string name, string ffn)
        {
            if (_instance != null)
            {
                return _instance;
            }

            Workspace i = Instance;
            i._name = name;
            i._ffn = ffn;
            return i;
        }
    }
}
