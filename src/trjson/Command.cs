using AntlrJson;
using ParseTreeEditing.UnvParseTreeDOM;
using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Trash;

class Command
{
    public string Help()
    {
        using Stream stream = GetType().Assembly.GetManifestResourceStream("trjson.readme.md");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    public void Execute(Config config)
    {
        if (config.Text && config.Bundle)
            throw new ArgumentException("--text and --bundle cannot be combined.");

        var input = ParsingResultIO.Read(config.File);
        if (!config.Text)
        {
            using var output = Console.OpenStandardOutput();
            ParsingResultIO.WriteFilteredBundle(
                output, input, ".json",
                result => ParsingResultIO.Utf8(Render(result)));
            return;
        }

        foreach (var result in input.Results)
            Console.WriteLine(Render(result));
    }

    private static string Render(ParsingResultSet result)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions
               { Indented = true, MaxDepth = 10000 }))
        {
            if (result.Nodes.Length != 1) writer.WriteStartArray();
            foreach (var node in result.Nodes)
                WriteNode(writer, node);
            if (result.Nodes.Length != 1) writer.WriteEndArray();
        }
        return Encoding.UTF8.GetString(stream.ToArray());
    }

    private static void WriteNode(Utf8JsonWriter writer, UnvParseTreeNode node)
    {
        if (node is UnvParseTreeText text)
        {
            writer.WriteStartObject();
            writer.WriteString("Text", text.Data);
            writer.WriteEndObject();
            return;
        }
        if (node is not UnvParseTreeElement element)
            throw new InvalidDataException($"Cannot render parse-tree node '{node.GetType().Name}' as JSON.");

        writer.WriteStartObject();
        writer.WritePropertyName(element.LocalName);
        writer.WriteStartArray();
        for (var i = 0; i < element.ChildNodes.Length; i++)
        {
            if (element.ChildNodes.item(i) is UnvParseTreeElement child)
                WriteNode(writer, child);
            else if (element.ChildNodes.item(i) is UnvParseTreeText childText)
                WriteNode(writer, childText);
        }
        writer.WriteEndArray();
        writer.WriteEndObject();
    }
}
