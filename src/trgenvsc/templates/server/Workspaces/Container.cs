namespace Workspaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Container
    {
        public virtual Container Parent
        {
            get;
            set;
        }
        public virtual Container AddChild(Container c)
        {
            if (c == null)
            {
                throw new System.Exception("Trying to add null item.");
            }
            _contents.Add(c);
            c.Parent = this;
            return c;
        }

        public virtual void Delete()
        {
            var parent = this.Parent;
            for (int i = 0; i < parent._contents.Count; ++i)
            {
                if (parent._contents[i] == this)
                {
                    parent._contents.RemoveAt(i);
                    return;
                }
            }
        }

        public virtual Document FindDocument(string ffn)
        {
            foreach (Container container in _contents)
            {
                Document found = container.FindDocument(ffn);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }

        public virtual Document ReadDocument(string ffn) { return null; }
        
        public virtual Project FindProject(string ffn)
        {
            if (ffn == null || FullPath == null)
            {
                return null;
            }

            if (FullPath.ToLower() == ffn.ToLower())
            {
                return this as Project;
            }

            foreach (Container proj in _contents)
            {
                Project found = proj.FindProject(ffn);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }
        public string FullPath
        {
            get => _ffn;
            set => _ffn = value;
        }

        private string _ffn;
        private string _canonical_name;
        public string CanonicalName
        {
            get => _canonical_name;
            set => _canonical_name = value;
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public virtual Project FindProject(string canonical_name, string name, string ffn)
        {
            if (CanonicalName == canonical_name &&
                Name == name &&
                FullPath.ToLower() == ffn.ToLower())
            {
                return this as Project;
            }

            foreach (Container proj in _contents)
            {
                Project found = proj.FindProject(canonical_name, name, ffn);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }
        public IEnumerable<Container> Children => _contents;
        private readonly List<Container> _contents = new List<Container>();
        public virtual Workspace Workspace
        {
            get
            {
                var c = this;
                for (;;)
                {
                    if (c is Workspace) return c as Workspace;
                    c = c.Parent;
                    if (c is null) return null;
                }
            }
        }
    }

    public class DFSContainer
    {
        public static IEnumerable<Document> DFS(Container root)
        {
            Stack<Container> toVisit = new Stack<Container>();
            Stack<Container> visitedAncestors = new Stack<Container>();
            toVisit.Push(root);
            while (toVisit.Count > 0)
            {
                Container node = toVisit.Peek();
                if (!(node is Document))
                {
                    if (visitedAncestors.PeekOrDefault() != node)
                    {
                        visitedAncestors.Push(node);
                        if (node is Document)
                        {
                        }
                        else if (node is Project)
                        {
                            Project internal_node = node as Project;
                            IEnumerable<Container> children = internal_node.Children;
                            foreach (Container o in children.Reverse())
                            {
                                toVisit.Push(o);
                            }
                            continue;
                        }
                        else if (node is Workspace)
                        {
                            Workspace internal_node = node as Workspace;
                            IEnumerable<Container> children = internal_node.Children;
                            foreach (Container o in children.Reverse())
                            {
                                toVisit.Push(o);
                            }
                            continue;
                        }
                    }
                    visitedAncestors.Pop();
                }
                else
                {
                    yield return node as Document;
                }
                toVisit.Pop();
            }
        }
    }

    internal static class StackHelper
    {
        public static Container PeekOrDefault(this Stack<Container> s)
        {
            return s.Count == 0 ? null : s.Peek();
        }
    }

}
