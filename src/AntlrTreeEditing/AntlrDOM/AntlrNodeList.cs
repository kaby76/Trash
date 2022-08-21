namespace AntlrTreeEditing.AntlrDOM
{
    using org.w3c.dom;
    using System;
    using System.Collections.Generic;

    public class AntlrNodeList : NodeList
    {
        public List<AntlrNode> _node_list = new List<AntlrNode>();

        public int Length
        {
            get { return _node_list.Count; }
            set { throw new Exception(); }
        }

        public Node item(int i)
        {
            return _node_list[i];
        }

        public void Add(AntlrNode e)
        {
            _node_list.Add(e);
        }

        public void Delete(AntlrNode e)
        {
            if (_node_list.Contains(e))
                _node_list.Remove(e);
        }

    }
}
