namespace ParseTreeEditing.AntlrDOM
{
    using org.w3c.dom;
    using System;
    using System.Collections.Generic;

    public class AntlrNodeList : NodeList
    {
        public List<Node> _node_list = new List<Node>();

        public int Length
        {
            get { return _node_list.Count; }
            set { throw new Exception(); }
        }

        public Node item(int i)
        {
            return _node_list[i];
        }

        public void Add(Node e)
        {
            _node_list.Add(e);
        }

        public void Delete(Node e)
        {
            if (_node_list.Contains(e))
                _node_list.Remove(e);
        }

        public void Insert(int i, Node node)
        {
            this._node_list.Insert(i, node);
        }

        public void Replace(int i, Node node)
        {
            this._node_list.RemoveAt(i);
            this._node_list.Insert(i, node);
        }

        public void RemoveAt(int i)
        {
            this._node_list.RemoveAt(i);
        }

        public IEnumerable<Node> All()
        {
            return this._node_list;
        }
    }
}
