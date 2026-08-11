using System.Text.Json;
using XQuery.DataModel;

namespace XQuery.IO;

/// <summary>
/// Reads JSON documents into the XDM data model.
/// </summary>
public static class JsonDocumentReader
{
    /// <summary>
    /// Parses a JSON string into an XdmItem (map, array, or atomic value).
    /// </summary>
    public static XdmItem Parse(string json)
    {
        using var doc = JsonDocument.Parse(json);
        return ConvertElement(doc.RootElement);
    }

    /// <summary>
    /// Loads a JSON file into an XdmItem.
    /// </summary>
    public static XdmItem Load(string path)
    {
        var json = File.ReadAllText(path);
        return Parse(json);
    }

    /// <summary>
    /// Loads JSON from a stream into an XdmItem.
    /// </summary>
    public static XdmItem Load(Stream stream)
    {
        using var doc = JsonDocument.Parse(stream);
        return ConvertElement(doc.RootElement);
    }

    /// <summary>
    /// Converts JSON to an XML representation.
    /// </summary>
    public static XdmDocument ParseAsXml(string json, string rootElementName = "root")
    {
        using var doc = JsonDocument.Parse(json);
        var xdmDoc = new XdmDocument();
        var root = ConvertElementToXml(doc.RootElement, rootElementName);
        xdmDoc.AppendChild(root);
        return xdmDoc;
    }

    private static XdmItem ConvertElement(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                return ConvertObject(element);

            case JsonValueKind.Array:
                return ConvertArray(element);

            case JsonValueKind.String:
                return new XdmAtomicValue(element.GetString() ?? string.Empty);

            case JsonValueKind.Number:
                if (element.TryGetInt64(out var longValue))
                    return new XdmAtomicValue(longValue);
                if (element.TryGetDecimal(out var decimalValue))
                    return new XdmAtomicValue(decimalValue);
                return new XdmAtomicValue(element.GetDouble());

            case JsonValueKind.True:
                return XdmAtomicValue.True;

            case JsonValueKind.False:
                return XdmAtomicValue.False;

            case JsonValueKind.Null:
                // Represent null as empty sequence would require special handling
                // For now, return empty string
                return new XdmAtomicValue(string.Empty);

            default:
                return new XdmAtomicValue(element.GetRawText());
        }
    }

    private static XdmMap ConvertObject(JsonElement element)
    {
        var builder = XdmMap.Builder();

        foreach (var property in element.EnumerateObject())
        {
            var key = new XdmAtomicValue(property.Name);
            var value = ConvertElement(property.Value);
            builder.Add(key, new XdmSequence(value));
        }

        return builder.Build();
    }

    private static XdmArray ConvertArray(JsonElement element)
    {
        var builder = XdmArray.Builder();

        foreach (var item in element.EnumerateArray())
        {
            var value = ConvertElement(item);
            builder.Add(new XdmSequence(value));
        }

        return builder.Build();
    }

    private static XdmElement ConvertElementToXml(JsonElement element, string name)
    {
        var elem = new XdmElement(name);

        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in element.EnumerateObject())
                {
                    var child = ConvertElementToXml(property.Value, property.Name);
                    elem.AppendChild(child);
                }
                break;

            case JsonValueKind.Array:
                int index = 1;
                foreach (var item in element.EnumerateArray())
                {
                    var child = ConvertElementToXml(item, "item");
                    child.SetAttribute("index", index.ToString());
                    elem.AppendChild(child);
                    index++;
                }
                break;

            case JsonValueKind.String:
                elem.AppendChild(new XdmText(element.GetString() ?? string.Empty));
                elem.SetAttribute("type", "string");
                break;

            case JsonValueKind.Number:
                elem.AppendChild(new XdmText(element.GetRawText()));
                elem.SetAttribute("type", "number");
                break;

            case JsonValueKind.True:
            case JsonValueKind.False:
                elem.AppendChild(new XdmText(element.GetBoolean().ToString().ToLowerInvariant()));
                elem.SetAttribute("type", "boolean");
                break;

            case JsonValueKind.Null:
                elem.SetAttribute("type", "null");
                break;
        }

        return elem;
    }
}
