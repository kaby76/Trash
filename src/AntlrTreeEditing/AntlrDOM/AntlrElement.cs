namespace AntlrTreeEditing.AntlrDOM
{
    using org.w3c.dom;
    using System;

    public class AntlrElement : AntlrNode, Element
    {
        public AntlrElement() { }

        public virtual void EnterRule(IMyParseTreeListener listener)
        {
        }

        public virtual void ExitRule(IMyParseTreeListener listener)
        {
        }

        public object getAttributeNS(string sCHEMA_INSTANCE, string nIL_ATTRIBUTE)
        {
            return null;
        }

        public string Prefix { get; set; }
        public TypeInfo SchemaTypeInfo { get; set; }
        public string lookupNamespaceURI(string prefix)
        {
            throw new NotImplementedException();
        }

        public bool isDefaultNamespace(object elementNamespaceUri)
        {
            throw new NotImplementedException();
        }

        public string GetText()
        {
            throw new NotImplementedException();
        }
    }
}
