namespace Workspaces
{
    using System.Collections.Generic;

    public class Project : Container
    {
        public uint _id;
        public int _hash;
        private readonly Dictionary<string, string> _properties = new Dictionary<string, string>();
        private readonly Dictionary<string, bool> _lazy_evaluated = new Dictionary<string, bool>();


        public Project(string canonical_name, string name, string ffn)
        {
            this.CanonicalName = canonical_name;
            this.Name = name;
            this.FullPath = ffn;
        }


        public Document AddDocument(Document doc)
        {
            return AddChild(doc) as Document;
        }

        public void AddProperty(string name)
        {
            _properties[name] = null;
            _lazy_evaluated[name] = false;
        }

        public void AddProperty(string name, string value)
        {
            _properties[name] = value;
            _lazy_evaluated[name] = true;
        }

        public string GetProperty(string name)
        {
            _lazy_evaluated.TryGetValue(name, out _);
            string result = null;
            return result;
        }
    }
}
