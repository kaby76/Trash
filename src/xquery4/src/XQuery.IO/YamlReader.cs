using YamlDotNet.RepresentationModel;
using XQuery.DataModel;

namespace XQuery.IO;

/// <summary>
/// Reads YAML documents into the XDM data model.
/// </summary>
public static class YamlDocumentReader
{
    /// <summary>
    /// Parses a YAML string into an XdmItem (map, array, or atomic value).
    /// </summary>
    public static XdmItem Parse(string yaml)
    {
        using var reader = new StringReader(yaml);
        var yamlStream = new YamlStream();
        yamlStream.Load(reader);

        if (yamlStream.Documents.Count == 0)
            return XdmMap.Empty;

        return ConvertNode(yamlStream.Documents[0].RootNode);
    }

    /// <summary>
    /// Parses multiple YAML documents into a sequence.
    /// </summary>
    public static XdmSequence ParseAll(string yaml)
    {
        using var reader = new StringReader(yaml);
        var yamlStream = new YamlStream();
        yamlStream.Load(reader);

        var items = new List<XdmItem>();
        foreach (var doc in yamlStream.Documents)
        {
            items.Add(ConvertNode(doc.RootNode));
        }

        return new XdmSequence(items);
    }

    /// <summary>
    /// Loads a YAML file into an XdmItem.
    /// </summary>
    public static XdmItem Load(string path)
    {
        var yaml = File.ReadAllText(path);
        return Parse(yaml);
    }

    /// <summary>
    /// Loads YAML from a stream into an XdmItem.
    /// </summary>
    public static XdmItem Load(Stream stream)
    {
        using var reader = new StreamReader(stream);
        var yaml = reader.ReadToEnd();
        return Parse(yaml);
    }

    /// <summary>
    /// Converts YAML to an XML representation.
    /// </summary>
    public static XdmDocument ParseAsXml(string yaml, string rootElementName = "root")
    {
        using var reader = new StringReader(yaml);
        var yamlStream = new YamlStream();
        yamlStream.Load(reader);

        var xdmDoc = new XdmDocument();

        if (yamlStream.Documents.Count == 0)
        {
            xdmDoc.AppendChild(new XdmElement(rootElementName));
            return xdmDoc;
        }

        var root = ConvertNodeToXml(yamlStream.Documents[0].RootNode, rootElementName);
        xdmDoc.AppendChild(root);
        return xdmDoc;
    }

    private static XdmItem ConvertNode(YamlNode node)
    {
        switch (node)
        {
            case YamlMappingNode mapping:
                return ConvertMapping(mapping);

            case YamlSequenceNode sequence:
                return ConvertSequence(sequence);

            case YamlScalarNode scalar:
                return ConvertScalar(scalar);

            default:
                return new XdmAtomicValue(node.ToString());
        }
    }

    private static XdmMap ConvertMapping(YamlMappingNode mapping)
    {
        var builder = XdmMap.Builder();

        foreach (var entry in mapping.Children)
        {
            var keyNode = entry.Key as YamlScalarNode;
            var key = new XdmAtomicValue(keyNode?.Value ?? entry.Key.ToString());
            var value = ConvertNode(entry.Value);
            builder.Add(key, new XdmSequence(value));
        }

        return builder.Build();
    }

    private static XdmArray ConvertSequence(YamlSequenceNode sequence)
    {
        var builder = XdmArray.Builder();

        foreach (var item in sequence.Children)
        {
            var value = ConvertNode(item);
            builder.Add(new XdmSequence(value));
        }

        return builder.Build();
    }

    private static XdmAtomicValue ConvertScalar(YamlScalarNode scalar)
    {
        var value = scalar.Value ?? string.Empty;

        // Try to detect type based on YAML tag or value
        if (scalar.Tag == "tag:yaml.org,2002:bool" ||
            value.Equals("true", StringComparison.OrdinalIgnoreCase))
            return XdmAtomicValue.True;

        if (value.Equals("false", StringComparison.OrdinalIgnoreCase))
            return XdmAtomicValue.False;

        if (scalar.Tag == "tag:yaml.org,2002:null" ||
            value.Equals("null", StringComparison.OrdinalIgnoreCase) ||
            value == "~" || string.IsNullOrEmpty(value))
            return new XdmAtomicValue(string.Empty);

        if (scalar.Tag == "tag:yaml.org,2002:int" ||
            (long.TryParse(value, out var longValue) && !value.Contains('.')))
            return new XdmAtomicValue(long.Parse(value));

        if (scalar.Tag == "tag:yaml.org,2002:float" ||
            double.TryParse(value, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out var doubleValue))
            return new XdmAtomicValue(double.Parse(value, System.Globalization.CultureInfo.InvariantCulture));

        return new XdmAtomicValue(value);
    }

    private static XdmElement ConvertNodeToXml(YamlNode node, string name)
    {
        var elem = new XdmElement(SanitizeName(name));

        switch (node)
        {
            case YamlMappingNode mapping:
                foreach (var entry in mapping.Children)
                {
                    var keyNode = entry.Key as YamlScalarNode;
                    var childName = keyNode?.Value ?? entry.Key.ToString();
                    var child = ConvertNodeToXml(entry.Value, childName);
                    elem.AppendChild(child);
                }
                break;

            case YamlSequenceNode sequence:
                int index = 1;
                foreach (var item in sequence.Children)
                {
                    var child = ConvertNodeToXml(item, "item");
                    child.SetAttribute("index", index.ToString());
                    elem.AppendChild(child);
                    index++;
                }
                break;

            case YamlScalarNode scalar:
                var value = scalar.Value ?? string.Empty;
                elem.AppendChild(new XdmText(value));

                // Add type attribute
                if (scalar.Tag == "tag:yaml.org,2002:bool" ||
                    value.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                    value.Equals("false", StringComparison.OrdinalIgnoreCase))
                    elem.SetAttribute("type", "boolean");
                else if (scalar.Tag == "tag:yaml.org,2002:int" ||
                    (long.TryParse(value, out _) && !value.Contains('.')))
                    elem.SetAttribute("type", "integer");
                else if (scalar.Tag == "tag:yaml.org,2002:float" ||
                    double.TryParse(value, System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture, out _))
                    elem.SetAttribute("type", "number");
                else if (scalar.Tag == "tag:yaml.org,2002:null" ||
                    value.Equals("null", StringComparison.OrdinalIgnoreCase) ||
                    value == "~")
                    elem.SetAttribute("type", "null");
                else
                    elem.SetAttribute("type", "string");
                break;
        }

        return elem;
    }

    private static string SanitizeName(string name)
    {
        // Make the name a valid XML element name
        if (string.IsNullOrEmpty(name))
            return "_";

        var result = new System.Text.StringBuilder();

        // First character must be letter or underscore
        var first = name[0];
        if (char.IsLetter(first) || first == '_')
            result.Append(first);
        else
            result.Append('_');

        // Subsequent characters can include digits and hyphens
        for (int i = 1; i < name.Length; i++)
        {
            var c = name[i];
            if (char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == '.')
                result.Append(c);
            else
                result.Append('_');
        }

        return result.ToString();
    }
}
