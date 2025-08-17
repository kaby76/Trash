using AntlrJson;
using org.eclipse.wst.xml.xpath2.processor.util;
using ParseTreeEditing.UnvParseTreeDOM;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Trash;

class Command
{
    public string Help()
    {
        using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trunfoldlit.readme.md"))
        using (StreamReader reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }

    public void Execute(Config config)
    {
        var expr = "//parserRuleSpec//atom/terminalDef/TOKEN_REF";
        if (config.Verbose)
        {
            System.Console.Error.WriteLine("Expr = >>>" + expr + "<<<");
        }

        string lines = null;
        if (!(config.File != null && config.File != ""))
        {
            if (config.Verbose)
            {
                System.Console.Error.WriteLine("reading from stdin");
            }

            for (;;)
            {
                lines = System.Console.In.ReadToEnd();
                if (lines != null && lines != "") break;
            }

            lines = lines.Trim();
        }
        else
        {
            if (config.Verbose)
            {
                System.Console.Error.WriteLine("reading from file >>>" + config.File + "<<<");
            }

            lines = File.ReadAllText(config.File);
        }

        var serializeOptions = new JsonSerializerOptions();
        serializeOptions.Converters.Add(new AntlrJson.ParsingResultSetSerializer());
        serializeOptions.WriteIndented = config.Format;
        serializeOptions.MaxDepth = 10000;
        if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("starting deserialization");
        var data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
        if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("deserialized");
        var results = new List<ParsingResultSet>();
        foreach (var parse_info in data)
        {
            var fn = parse_info.FileName;
            var trees = parse_info.Nodes;
            var parser = parse_info.Parser;
            var lexer = parse_info.Lexer;
            if (config.Verbose)
            {
                foreach (var n in trees)
                    System.Console.WriteLine(new TreeOutput(lexer, parser).OutputTree(n).ToString());
            }

            org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
            var ate = new ParseTreeEditing.UnvParseTreeDOM.ConvertToDOM();
            using (AntlrDynamicContext dynamicContext = ate.Try(trees, parser))
            {
                var nodes = engine.parseExpression(expr,
                        new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("Found " + nodes.Count + " nodes.");
                foreach (UnvParseTreeElement node in nodes)
                {
                    // Find parser or lexer rule.
                    var name = node.GetText();
                    var exp = $" doc('*')//lexerRuleSpec[TOKEN_REF/text()='{name}' and not(FRAGMENT)]/lexerRuleBlock[lexerAltList[count(*) = 1]/lexerAlt[count(*) = 1]/lexerElements[count(*) = 1]/lexerElement[count(*) = 1]/lexerAtom/terminalDef[count(*) = 1]/STRING_LITERAL]";
                    var defs = engine.parseExpression(exp,
                            new StaticContextBuilder())
                        .evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                    if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("Found " + defs.Count + " defs.");
                    if (defs.Count != 1) continue;
                    var copy = TreeEdits.CopyTreeRecursive(defs.First());
                    //TreeEdits.InsertBefore(node, "(");
                    //TreeEdits.InsertAfter(node, ")");
                    // Replace
                    TreeEdits.InsertBefore(node, copy);
                    TreeEdits.Delete(node);
                }

                if (config.Verbose)
                {
                    System.Console.Error.WriteLine("Final trees:");
                    foreach (var n in trees)
                        System.Console.WriteLine(new TreeOutput(lexer, parser).OutputTree(n).ToString());
                }

                var tuple = new ParsingResultSet()
                {
                    FileName = fn,
                    Nodes = trees,
                    Lexer = lexer,
                    Parser = parser
                };
                results.Add(tuple);
            }
        }

        if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("starting serialization");
        string js1 = JsonSerializer.Serialize(results.ToArray(), serializeOptions);
        if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("serialized");
        System.Console.WriteLine(js1);
    }
}
