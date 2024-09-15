using AntlrJson;
using org.eclipse.wst.xml.xpath2.processor.util;
using ParseTreeEditing.UnvParseTreeDOM;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Antlr4.Runtime;

namespace Trash;

class Command
{
    public string Help()
    {
        using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trungroup.readme.md"))
        using (StreamReader reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }

    public void Execute(Config config)
    {
        var expr = config.Expr.First();
        //System.Console.Error.WriteLine("Expr = '" + expr + "'");
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
        var data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
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

                this.Ungroup(nodes, parser, lexer, config, results, fn);
               // var res = LanguageServer.Transform.Ungroup(nodes, doc);

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

        string js1 = JsonSerializer.Serialize(results.ToArray(), serializeOptions);
        System.Console.WriteLine(js1);
    }

    private void Ungroup(List<UnvParseTreeElement> nodes, Parser parser, Lexer lexer, Config config, List<ParsingResultSet> results, string fn)
    {
        /*
        Ungroup:
        
        a b (c | d) -> a b c | a b d
        a (b | c) d -> a b d | a c d
	a ( b (c | d) | e ) f -> a ( (b c | b d) | e ) f   ungroup 2nd paren.
                         or  a b (c | d) f | a e f         ungroup 1st paren.
         */
        throw new System.NotImplementedException();
    }
}
