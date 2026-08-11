using System.Xml;
using XQuery.DataModel;

namespace XQuery.IO;

/// <summary>
/// Reads XML documents into the XDM data model.
/// </summary>
public static class XmlDocumentReader
{
    /// <summary>
    /// Parses an XML string into an XdmDocument.
    /// </summary>
    public static XdmDocument Parse(string xml)
    {
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xml);
        return ConvertDocument(xmlDoc);
    }

    /// <summary>
    /// Loads an XML file into an XdmDocument.
    /// </summary>
    public static XdmDocument Load(string path)
    {
        var xmlDoc = new XmlDocument();
        xmlDoc.Load(path);
        var doc = ConvertDocument(xmlDoc);
        doc.SetDocumentUri(new Uri(Path.GetFullPath(path)));
        return doc;
    }

    /// <summary>
    /// Loads an XML file from a stream into an XdmDocument.
    /// </summary>
    public static XdmDocument Load(Stream stream)
    {
        var xmlDoc = new XmlDocument();
        xmlDoc.Load(stream);
        return ConvertDocument(xmlDoc);
    }

    private static XdmDocument ConvertDocument(XmlDocument xmlDoc)
    {
        var doc = new XdmDocument();

        foreach (XmlNode child in xmlDoc.ChildNodes)
        {
            var converted = ConvertNode(child);
            if (converted != null)
                doc.AppendChild(converted);
        }

        return doc;
    }

    private static XdmNode? ConvertNode(XmlNode node)
    {
        switch (node.NodeType)
        {
            case XmlNodeType.Element:
                return ConvertElement((XmlElement)node);

            case XmlNodeType.Text:
            case XmlNodeType.CDATA:
                var text = node.Value ?? string.Empty;
                if (!string.IsNullOrEmpty(text))
                    return new XdmText(text);
                return null;

            case XmlNodeType.Comment:
                return new XdmComment(node.Value ?? string.Empty);

            case XmlNodeType.ProcessingInstruction:
                var pi = (XmlProcessingInstruction)node;
                return new XdmProcessingInstruction(pi.Target, pi.Data);

            case XmlNodeType.Whitespace:
            case XmlNodeType.SignificantWhitespace:
                // Preserve whitespace as text
                var wsText = node.Value ?? string.Empty;
                if (!string.IsNullOrEmpty(wsText))
                    return new XdmText(wsText);
                return null;

            default:
                return null;
        }
    }

    private static XdmElement ConvertElement(XmlElement xmlElem)
    {
        var name = string.IsNullOrEmpty(xmlElem.NamespaceURI)
            ? new XdmQName(xmlElem.LocalName)
            : new XdmQName(xmlElem.NamespaceURI, xmlElem.LocalName, xmlElem.Prefix);

        var elem = new XdmElement(name);

        // Add namespace declarations
        foreach (XmlAttribute attr in xmlElem.Attributes)
        {
            if (attr.Prefix == "xmlns" || attr.Name == "xmlns")
            {
                var nsPrefix = attr.Prefix == "xmlns" ? attr.LocalName : string.Empty;
                elem.AddNamespace(nsPrefix, attr.Value);
            }
        }

        // Add attributes (excluding namespace declarations)
        foreach (XmlAttribute attr in xmlElem.Attributes)
        {
            if (attr.Prefix == "xmlns" || attr.Name == "xmlns")
                continue;

            var attrName = string.IsNullOrEmpty(attr.NamespaceURI)
                ? new XdmQName(attr.LocalName)
                : new XdmQName(attr.NamespaceURI, attr.LocalName, attr.Prefix);

            elem.AddAttribute(new XdmAttribute(attrName, attr.Value));
        }

        // Add children
        foreach (XmlNode child in xmlElem.ChildNodes)
        {
            var converted = ConvertNode(child);
            if (converted != null)
                elem.AppendChild(converted);
        }

        return elem;
    }
}
