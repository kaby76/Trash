namespace Trash
{
    using Antlr4.Runtime.Tree;
    using AntlrJson;
    using AntlrTreeEditing.AntlrDOM;
    using LanguageServer;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using LoggerNs;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.Json;

    class Command
    {
        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trdelete.readme.md"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public void Execute(Config config)
        {
            var expr = config.Expr.First();
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
                for (; ; )
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
            serializeOptions.WriteIndented = false;
            serializeOptions.MaxDepth = 10000;
            var data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
            var results = new List<ParsingResultSet>();
            foreach (var parse_info in data)
            {
                var text = parse_info.Text;
                var fn = parse_info.FileName;
                var trees = parse_info.Nodes;
                var parser = parse_info.Parser;
                var lexer = parse_info.Lexer;

                if (config.Verbose)
                {
                    foreach (var n in trees)
                        System.Console.WriteLine(TreeOutput.OutputTree(n, lexer, parser).ToString());
                }
                org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                var root = trees.ToArray();
                var ate = new AntlrTreeEditing.AntlrDOM.ConvertToDOM();
                using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = ate.Try(trees, parser))
                {
                    var nodes = engine.parseExpression(expr,
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement)).ToList();
                    if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("Found " + nodes.Count + " nodes.");
                    foreach (var node in nodes)
                    {
                        TreeEdits.Delete(node);
                    }

                    var tuple = new ParsingResultSet()
                    {
                        Text = text,
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
    }
}
