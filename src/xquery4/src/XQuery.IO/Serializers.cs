using System.Text;
using System.Text.Json;
using XQuery.DataModel;
using YamlDotNet.Serialization;

namespace XQuery.IO;

/// <summary>
/// Options for serialization.
/// </summary>
public class SerializationOptions
{
    public bool Indent { get; set; } = true;
    public string IndentChars { get; set; } = "  ";
    public bool OmitXmlDeclaration { get; set; } = false;
    public string? Encoding { get; set; } = "UTF-8";
    public bool EscapeUriAttributes { get; set; } = false;
}

/// <summary>
/// Serializes XDM items to XML format.
/// </summary>
public static class XmlSerializer
{
    public static string Serialize(XdmSequence sequence, SerializationOptions? options = null)
    {
        options ??= new SerializationOptions();
        var sb = new StringBuilder();

        if (!options.OmitXmlDeclaration)
        {
            sb.AppendLine($"<?xml version=\"1.0\" encoding=\"{options.Encoding}\"?>");
        }

        foreach (var item in sequence)
        {
            if (item is XdmNode node)
                SerializeNode(node, sb, 0, options);
            else
                sb.Append(item.StringValue);
        }

        return sb.ToString();
    }

    public static string Serialize(XdmNode node, SerializationOptions? options = null)
    {
        return Serialize(new XdmSequence(node), options);
    }

    private static void SerializeNode(XdmNode node, StringBuilder sb, int depth, SerializationOptions options)
    {
        var indent = options.Indent ? new string(' ', depth * options.IndentChars.Length) : "";

        switch (node.NodeKind)
        {
            case XdmNodeKind.Document:
                foreach (var child in node.Children)
                    SerializeNode(child, sb, depth, options);
                break;

            case XdmNodeKind.Element:
                var elem = (XdmElement)node;
                if (options.Indent && depth > 0)
                    sb.Append(indent);

                sb.Append('<');
                sb.Append(elem.NodeName!.PrefixedName);

                // Namespace declarations
                foreach (var ns in elem.NamespaceBindings)
                {
                    if (string.IsNullOrEmpty(ns.Key))
                        sb.Append($" xmlns=\"{EscapeAttribute(ns.Value)}\"");
                    else
                        sb.Append($" xmlns:{ns.Key}=\"{EscapeAttribute(ns.Value)}\"");
                }

                // Attributes
                foreach (var attr in elem.Attributes)
                {
                    sb.Append(' ');
                    sb.Append(attr.NodeName!.PrefixedName);
                    sb.Append("=\"");
                    sb.Append(EscapeAttribute(attr.Value));
                    sb.Append('"');
                }

                if (elem.Children.Count == 0)
                {
                    sb.Append("/>");
                    if (options.Indent)
                        sb.AppendLine();
                }
                else
                {
                    sb.Append('>');

                    bool hasElements = elem.Children.Any(c => c is XdmElement);
                    if (hasElements && options.Indent)
                        sb.AppendLine();

                    foreach (var child in elem.Children)
                        SerializeNode(child, sb, depth + 1, options);

                    if (hasElements && options.Indent)
                        sb.Append(indent);

                    sb.Append("</");
                    sb.Append(elem.NodeName.PrefixedName);
                    sb.Append('>');
                    if (options.Indent)
                        sb.AppendLine();
                }
                break;

            case XdmNodeKind.Text:
                sb.Append(EscapeText(((XdmText)node).Value));
                break;

            case XdmNodeKind.Comment:
                if (options.Indent)
                    sb.Append(indent);
                sb.Append("<!--");
                sb.Append(((XdmComment)node).Value);
                sb.Append("-->");
                if (options.Indent)
                    sb.AppendLine();
                break;

            case XdmNodeKind.ProcessingInstruction:
                var pi = (XdmProcessingInstruction)node;
                if (options.Indent)
                    sb.Append(indent);
                sb.Append("<?");
                sb.Append(pi.Target);
                if (!string.IsNullOrEmpty(pi.Data))
                {
                    sb.Append(' ');
                    sb.Append(pi.Data);
                }
                sb.Append("?>");
                if (options.Indent)
                    sb.AppendLine();
                break;
        }
    }

    private static string EscapeText(string text)
    {
        return text
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;");
    }

    private static string EscapeAttribute(string text)
    {
        return text
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("\n", "&#10;")
            .Replace("\r", "&#13;")
            .Replace("\t", "&#9;");
    }
}

/// <summary>
/// Serializes XDM items to JSON format.
/// </summary>
public static class JsonSerializer
{
    public static string Serialize(XdmSequence sequence, SerializationOptions? options = null)
    {
        options ??= new SerializationOptions();

        if (sequence.Count == 1)
        {
            return SerializeItem(sequence.First!, options);
        }

        // Multiple items become a JSON array
        var jsonOptions = new JsonWriterOptions { Indented = options.Indent };
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream, jsonOptions))
        {
            writer.WriteStartArray();
            foreach (var item in sequence)
            {
                WriteItem(writer, item);
            }
            writer.WriteEndArray();
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    public static string Serialize(XdmItem item, SerializationOptions? options = null)
    {
        return SerializeItem(item, options ?? new SerializationOptions());
    }

    private static string SerializeItem(XdmItem item, SerializationOptions options)
    {
        var jsonOptions = new JsonWriterOptions { Indented = options.Indent };
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream, jsonOptions))
        {
            WriteItem(writer, item);
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    private static void WriteItem(Utf8JsonWriter writer, XdmItem item)
    {
        switch (item)
        {
            case XdmMap map:
                writer.WriteStartObject();
                foreach (var entry in map)
                {
                    writer.WritePropertyName(entry.Key.StringValue);
                    WriteSequence(writer, entry.Value);
                }
                writer.WriteEndObject();
                break;

            case XdmArray array:
                writer.WriteStartArray();
                foreach (var member in array)
                {
                    WriteSequence(writer, member);
                }
                writer.WriteEndArray();
                break;

            case XdmAtomicValue atomic:
                if (atomic.IsBoolean)
                    writer.WriteBooleanValue(atomic.AsBoolean());
                else if (atomic.IsInteger)
                    writer.WriteNumberValue(atomic.AsInteger());
                else if (atomic.IsNumeric)
                    writer.WriteNumberValue(atomic.AsDouble());
                else
                    writer.WriteStringValue(atomic.StringValue);
                break;

            case XdmNode node:
                // Serialize node as its string value
                writer.WriteStringValue(node.StringValue);
                break;

            default:
                writer.WriteStringValue(item.StringValue);
                break;
        }
    }

    private static void WriteSequence(Utf8JsonWriter writer, XdmSequence sequence)
    {
        if (sequence.Count == 0)
        {
            writer.WriteNullValue();
        }
        else if (sequence.Count == 1)
        {
            WriteItem(writer, sequence.First!);
        }
        else
        {
            writer.WriteStartArray();
            foreach (var item in sequence)
            {
                WriteItem(writer, item);
            }
            writer.WriteEndArray();
        }
    }
}

/// <summary>
/// Serializes XDM items to YAML format.
/// </summary>
public static class YamlSerializer
{
    public static string Serialize(XdmSequence sequence, SerializationOptions? options = null)
    {
        options ??= new SerializationOptions();

        if (sequence.Count == 1)
        {
            return SerializeItem(sequence.First!, options);
        }

        // Multiple items become a YAML array
        var obj = new List<object?>();
        foreach (var item in sequence)
        {
            obj.Add(ConvertToObject(item));
        }

        var serializer = new SerializerBuilder().Build();
        return serializer.Serialize(obj);
    }

    public static string Serialize(XdmItem item, SerializationOptions? options = null)
    {
        return SerializeItem(item, options ?? new SerializationOptions());
    }

    private static string SerializeItem(XdmItem item, SerializationOptions options)
    {
        var obj = ConvertToObject(item);
        var serializer = new SerializerBuilder().Build();
        return serializer.Serialize(obj);
    }

    private static object? ConvertToObject(XdmItem item)
    {
        switch (item)
        {
            case XdmMap map:
                var dict = new Dictionary<string, object?>();
                foreach (var entry in map)
                {
                    dict[entry.Key.StringValue] = ConvertSequenceToObject(entry.Value);
                }
                return dict;

            case XdmArray array:
                var list = new List<object?>();
                foreach (var member in array)
                {
                    list.Add(ConvertSequenceToObject(member));
                }
                return list;

            case XdmAtomicValue atomic:
                if (atomic.IsBoolean)
                    return atomic.AsBoolean();
                if (atomic.IsInteger)
                    return atomic.AsInteger();
                if (atomic.IsNumeric)
                    return atomic.AsDouble();
                return atomic.StringValue;

            case XdmNode node:
                return node.StringValue;

            default:
                return item.StringValue;
        }
    }

    private static object? ConvertSequenceToObject(XdmSequence sequence)
    {
        if (sequence.Count == 0)
            return null;
        if (sequence.Count == 1)
            return ConvertToObject(sequence.First!);

        var list = new List<object?>();
        foreach (var item in sequence)
        {
            list.Add(ConvertToObject(item));
        }
        return list;
    }
}
