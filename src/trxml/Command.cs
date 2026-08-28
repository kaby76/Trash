using ParseTreeEditing.UnvParseTreeDOM;
using System;
using System.IO;
using System.Text.Json;

namespace Trash;

class Command
{
    public string Help()
    {
        using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trxml.readme.md"))
        using (StreamReader reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }

    class XmlWalk : MyParseTreeListener
    {
        int INDENT = 4;
        int level = 0;
        private readonly TextWriter output;

        public XmlWalk(TextWriter output)
        {
            this.output = output;
        }

        public void EnterEveryRule(UnvParseTreeNode ctx)
        {
            output.WriteLine(
                indent()
                + "<" + ctx.LocalName
                + ">");
            ++level;
        }

        public void ExitEveryRule(UnvParseTreeNode ctx)
        {
            --level;
            output.WriteLine(
                indent()
                + "</" + ctx.LocalName
                + ">");
        }

        public void VisitErrorNode(UnvParseTreeNode node)
        {
            throw new NotImplementedException();
        }

        //public void VisitErrorNode(IErrorNode node)
        //{
        //    throw new NotImplementedException();
        //}

        public void VisitTerminal(UnvParseTreeNode node)
        {
            string value = (node as UnvParseTreeText).Data;
            {
                output.WriteLine(
                    indent()
                    + "<t>"
                    + value
                    + "</t>");
            }
        }

        private String indent()
        {
            var result = new string(' ', level * INDENT);
            return result;
        }
    }

    public void Execute(Config config)
    {
        var input = AntlrJson.ParsingResultIO.Read(config.File);
        if (config.Bundle)
        {
            using var output = System.Console.OpenStandardOutput();
            AntlrJson.ParsingResultIO.WriteFilteredBundle(
                output, input, ".xml",
                result => AntlrJson.ParsingResultIO.Utf8(Render(result)));
            return;
        }

        foreach (var parseInfo in input.Results)
            System.Console.Write(Render(parseInfo));
    }

    private static string Render(AntlrJson.ParsingResultSet parseInfo)
    {
        using var output = new StringWriter();
        foreach (var node in parseInfo.Nodes)
        {
            if (node is UnvParseTreeElement element)
                MyParseTreeWalker.Default.Walk(new XmlWalk(output), element);
        }
        return output.ToString();
    }
}
