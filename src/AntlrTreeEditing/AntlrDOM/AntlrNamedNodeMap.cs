namespace AntlrTreeEditing.AntlrDOM
{
    using org.w3c.dom;
    using System;
    using System.Collections.Generic;

    public class AntlrNamedNodeMap : NamedNodeMap
    {
        private List<Attr> _attrs = new List<Attr>();

        public void Add(Attr a)
        {
            _attrs.Add(a);
        }

        public int Length
        {
            get
            {
                return _attrs.Count;
            }
            set
            {
                throw new Exception();
            }
        }

        // Zero based???
        public Attr item(int i)
        {
            return _attrs[i];
        }
    }
}
