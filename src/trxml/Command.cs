using AntlrJson;
using ParseTreeEditing.UnvParseTreeDOM;
using System;
using System.IO;
using System.Xml;

namespace Trash;

class Command
{
    public string Help()
    {
        using Stream stream = GetType().Assembly.GetManifestResourceStream("trxml.readme.md");
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
                output, input, ".xml",
                result => ParsingResultIO.Utf8(Render(result)));
            return;
        }

        foreach (var result in input.Results)
            Console.Write(Render(result));
    }

    private static string Render(ParsingResultSet result)
    {
        using var output = new StringWriter();
        using (var writer = XmlWriter.Create(output, new XmlWriterSettings
               { Indent = true, OmitXmlDeclaration = true,
                 ConformanceLevel = ConformanceLevel.Fragment }))
        {
            foreach (var node in result.Nodes)
                WriteNode(writer, node);
        }
        if (result.Nodes.Length > 0)
            output.WriteLine();
        return output.ToString();
    }

    private static void WriteNode(XmlWriter writer, UnvParseTreeNode node)
    {
        if (node is UnvParseTreeText text)
        {
            writer.WriteElementString("t", text.Data);
            return;
        }
        if (node is not UnvParseTreeElement element)
            throw new InvalidDataException($"Cannot render parse-tree node '{node.GetType().Name}' as XML.");

        writer.WriteStartElement(element.LocalName);
        for (var i = 0; i < element.ChildNodes.Length; i++)
        {
            if (element.ChildNodes.item(i) is UnvParseTreeElement child)
                WriteNode(writer, child);
            else if (element.ChildNodes.item(i) is UnvParseTreeText childText)
                WriteNode(writer, childText);
        }
        writer.WriteEndElement();
    }
}
