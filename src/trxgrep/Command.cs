namespace Trash
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using AntlrJson;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Text.Json;

    class Command
    {
        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trxgrep.readme.md"))
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
            IParseTree[] atrees;
            Parser parser;
            Lexer lexer;
            string text;
            string fn;
            ITokenStream tokstream;
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
            serializeOptions.Converters.Add(new AntlrJson.ParseTreeConverter());
            serializeOptions.WriteIndented = false;
            var data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
            var results = new List<ParsingResultSet>();
            bool do_rs = !config.NoParsingResultSets;
            List<AntlrTreeEditing.AntlrDOM.AntlrNode> d = new List<AntlrTreeEditing.AntlrDOM.AntlrNode>();
            List<AntlrTreeEditing.AntlrDOM.AntlrDynamicContext> dc = new List<AntlrTreeEditing.AntlrDOM.AntlrDynamicContext>();
            org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
            foreach (var parse_info in data)
            {
                text = parse_info.Text;
                fn = parse_info.FileName;
                atrees = parse_info.Nodes;
                parser = parse_info.Parser;
                lexer = parse_info.Lexer;
                tokstream = parse_info.Stream;
                IParseTree root = atrees.First().Root();
                var ate = new AntlrTreeEditing.AntlrDOM.ConvertToDOM();
                AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = ate.Try(root, parser);
                dc.Add(dynamicContext);
                d.Add(dynamicContext.Document);
            }
            int i = 0;
            foreach (var parse_info in data)
            {
                var dynamicContext = dc[i++];
                var a = dynamicContext.Document;
                text = parse_info.Text;
                fn = parse_info.FileName;
                atrees = parse_info.Nodes;
                parser = parse_info.Parser;
                lexer = parse_info.Lexer;
                tokstream = parse_info.Stream;
                IParseTree root = atrees.First().Root();
                //IEnumerable<AntlrTreeEditing.AntlrDOM.AntlrNode> l = atrees.Select(t => ate.FindDomNode(t));
                AntlrTreeEditing.AntlrDOM.AntlrNode[] l = new AntlrTreeEditing.AntlrDOM.AntlrNode[1] { a };
                var nodes = engine.parseExpression(expr,
                        new StaticContextBuilder()).evaluate(dynamicContext, l)
                    .Select(x => (x.NativeValue)).ToArray();
                if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("Found " + nodes.Length + " nodes.");
                List<IParseTree> res = new List<IParseTree>();
                foreach (var v in nodes)
                {
                    if (v is AntlrTreeEditing.AntlrDOM.AntlrElement)
                    {
                        var q = v as AntlrTreeEditing.AntlrDOM.AntlrElement;
                        IParseTree r = q.AntlrIParseTree;
                        res.Add(r);
                    }
                    else if (v is AntlrTreeEditing.AntlrDOM.AntlrText)
                    {
                        var q = v as AntlrTreeEditing.AntlrDOM.AntlrText;
                        var s = q.AntlrIParseTree.GetText();
                        do_rs = false;
                        System.Console.WriteLine(s);
                    }
                    else if (v is AntlrTreeEditing.AntlrDOM.AntlrAttr)
                    {
                        var q = v as AntlrTreeEditing.AntlrDOM.AntlrAttr;
                        var s = q.Value;
                        do_rs = false;
                        System.Console.WriteLine(s);
                    }
                    else if (v is AntlrTreeEditing.AntlrDOM.AntlrDocument)
                    {
                        var q = v as AntlrTreeEditing.AntlrDOM.AntlrDocument;
                        do_rs = false;
                        System.Console.WriteLine(v);
                    }
                    else
                    {
                        do_rs = false;
                        System.Console.WriteLine(v);
                    }
                }
                var parse_info_out = new AntlrJson.ParsingResultSet() { Text = text, FileName = fn, Lexer = lexer, Parser = parser, Stream = tokstream, Nodes = res.ToArray() };
                results.Add(parse_info_out);
            }
            if (do_rs)
            {
                string js1 = JsonSerializer.Serialize(results.ToArray(), serializeOptions);
                System.Console.WriteLine(js1);
            }
        }
    }
}
