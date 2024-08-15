using java.net;
using java.util;
using javax.xml.datatype;
using javax.xml.@namespace;
using org.eclipse.wst.xml.xpath2.api;
using org.w3c.dom;
using System;
using System.Collections.Generic;
using System.Linq;
using DynamicContext = org.eclipse.wst.xml.xpath2.api.DynamicContext;

namespace ParseTreeEditing.AntlrDOM;

public class AntlrDynamicContext : DynamicContext, IDisposable
{
    private static List<AntlrDynamicContext> _instances = new List<AntlrDynamicContext>();

    public AntlrDynamicContext(Document document)
    {
        this.Document = Document;
        _instances.Add(this);
    }

    public Node LimitNode { get; }
    public AntlrDocument Document { get; set; }

    public ResultSequence getVariable(QName name)
    {
        throw new NotImplementedException();
    }

    public URI resolveUri(string uri)
    {
        if (uri == "*")
            return new URI(uri);
        else
            throw new NotImplementedException();
    }

    public GregorianCalendar CurrentDateTime { get; }
    public Duration TimezoneOffset { get; }

    public IEnumerable<Document> getDocument(URI uri)
    {
        if ("*" == uri.ToString())
        {
            return _instances.Select(i => i.Document);
        }
        else return new List<Document>() { Document };
    }

    public void Dispose()
    {
        Document.Dispose();
    }

    public IDictionary<string, IList<Document>> Collections { get; }
    public IList<Document> DefaultCollection { get; }

    public CollationProvider CollationProvider
    {
        get { return new MyCollationProvider(); }
    }
}
