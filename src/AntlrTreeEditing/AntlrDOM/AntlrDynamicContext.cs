namespace AntlrTreeEditing.AntlrDOM
{
    using java.net;
    using java.util;
    using javax.xml.datatype;
    using javax.xml.@namespace;
    using org.eclipse.wst.xml.xpath2.api;
    using org.w3c.dom;
    using System;
    using System.Collections.Generic;
    using DynamicContext = org.eclipse.wst.xml.xpath2.api.DynamicContext;

    public class AntlrDynamicContext : DynamicContext, IDisposable
    {
        public AntlrDynamicContext() { }

        public Node LimitNode { get; }
        public AntlrDocument Document { get; set; }
        public ResultSequence getVariable(QName name)
        {
            throw new NotImplementedException();
        }

        public URI resolveUri(string uri)
        {
            throw new NotImplementedException();
        }

        public GregorianCalendar CurrentDateTime { get; }
        public Duration TimezoneOffset { get; }
        public Document getDocument(URI uri)
        {
            return Document;
        }

        public void Dispose()
        {
            Document.Dispose();
        }

        public IDictionary<string, IList<Document>> Collections { get; }
        public IList<Document> DefaultCollection { get; }
        public CollationProvider CollationProvider
        {
            get { return new AntlrCollationProvider(); }
        }
    }
}
