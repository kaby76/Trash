using AntlrJson;
using org.eclipse.wst.xml.xpath2.processor.util;
using org.w3c.dom;
using ParseTreeEditing.UnvParseTreeDOM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Antlr4.Runtime;

namespace Trash
{
    class Command
    {
        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trextract.readme.md"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public void Execute(Config config)
        {
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
            JsonSerializerOptions serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new AntlrJson.ParsingResultSetSerializer());
            serializeOptions.WriteIndented = config.Format;
            serializeOptions.MaxDepth = 10000;
            ParsingResultSet[] data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
            List<ParsingResultSet> results = new List<ParsingResultSet>();
            foreach (ParsingResultSet parse_info in data)
            {
                UnvParseTreeNode[] atrees = parse_info.Nodes;
                Parser parser = parse_info.Parser;
                Lexer lexer = parse_info.Lexer;
                var filename = parse_info.FileName;

                org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                ConvertToDOM ate = new ParseTreeEditing.UnvParseTreeDOM.ConvertToDOM();
                
                List<ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement> id = null;
                UnvParseTreeElement z;
                using (ParseTreeEditing.UnvParseTreeDOM.AntlrDynamicContext dynamicContext = ate.Try(atrees, parser))
                {
                    id = engine.parseExpression(
                        "/grammarSpec/grammarDecl/identifier",
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                    z = engine.parseExpression(
                        "/grammarSpec",
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList().First();
                    if (id.Count == 0)
                    {
                        throw new Exception("No grammar name found.");
                    }
                    var macro_name = new Dictionary<UnvParseTreeElement, string>();
                    int id2 = 0;
                    {
                        // Move WS.
                        var wses = engine.parseExpression(
                                "//lexerElement/actionBlock/@WS",
                                new StaticContextBuilder()).evaluate(dynamicContext,
                                new object[] { dynamicContext.Document })
                            .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeNode))
                            .ToList();
                        foreach (UnvParseTreeNode ws in wses)
                        {
                            var to = engine.parseExpression(
                                    "./ancestor::actionBlock",
                                    new StaticContextBuilder()).evaluate(dynamicContext,
                                    new object[] { ws })
                                .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeNode))
                                .First();
                            TreeEdits.MoveBefore(new List<UnvParseTreeNode>() { ws as UnvParseTreeNode }, to);
                        }

                        // replace all //lexerElement/actionBlock with macro name.
                        List<UnvParseTreeElement> lexerActionBlocks = engine.parseExpression(
                                "//lexerElement/actionBlock",
                                new StaticContextBuilder()).evaluate(dynamicContext,
                                new object[] { dynamicContext.Document })
                            .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement))
                            .ToList();
                        foreach (UnvParseTreeElement a in lexerActionBlocks)
                        {
                            macro_name[a] = "$$" + id2++;
                            // replace with macro name.
                            TreeEdits.Replace(a, macro_name[a]);
                        }
                    }
                    {
                        // Move WS.
                        var wses = engine.parseExpression(
                                "//action_//@WS",
                                new StaticContextBuilder()).evaluate(dynamicContext,
                                new object[] { dynamicContext.Document })
                            .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeNode))
                            .ToList();
                        foreach (UnvParseTreeNode ws in wses)
                        {
                            var to = engine.parseExpression(
                                    "./ancestor::action_",
                                    new StaticContextBuilder()).evaluate(dynamicContext,
                                    new object[] { ws })
                                .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeNode))
                                .First();
                            TreeEdits.MoveBefore(new List<UnvParseTreeNode>() { ws as UnvParseTreeNode }, to);
                        }
                        // replace all action_ blocks with macro names.
                        List<UnvParseTreeElement> actions = engine.parseExpression(
                                "//action_",
                                new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                            .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                        foreach (UnvParseTreeElement a in actions)
                        {
                            macro_name[a] = "$$" + id2++;
                            // replace with macro name.
                            TreeEdits.Replace(a, macro_name[a]);
                        }
                    }
                    {
                        // Move WS.
                        var wses = engine.parseExpression(
                                "//element/actionBlock/@WS",
                                new StaticContextBuilder()).evaluate(dynamicContext,
                                new object[] { dynamicContext.Document })
                            .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement))
                            .ToList();
                        foreach (UnvParseTreeNode ws in wses)
                        {
                            var to = engine.parseExpression(
                                    "./ancestor::action_",
                                    new StaticContextBuilder()).evaluate(dynamicContext,
                                    new object[] { ws })
                                .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeNode))
                                .First();
                            TreeEdits.MoveBefore(new List<UnvParseTreeNode>() { ws as UnvParseTreeNode }, to);
                        }
                        // replace all //element/actionBlock with macro name.
                        List<UnvParseTreeElement> parserActionBlocks = engine.parseExpression(
                                "//element/actionBlock",
                                new StaticContextBuilder()).evaluate(dynamicContext,
                                new object[] { dynamicContext.Document })
                            .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement))
                            .ToList();
                        foreach (UnvParseTreeElement a in parserActionBlocks)
                        {
                            macro_name[a] = "$$" + id2++;
                            // replace with macro name.
                            TreeEdits.Replace(a, macro_name[a]);
                        }
                    }
                }
                ParsingResultSet tuple1 = new ParsingResultSet()
                {
                    Nodes = atrees,
                    FileName = filename,
                    Lexer = lexer,
                    Parser = parser
                };
                results.Add(tuple1);
            }
            if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("starting serialization");
            string js1 = JsonSerializer.Serialize(results.ToArray(), serializeOptions);
            if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("serialized");
            System.Console.WriteLine(js1);

            //var results = LanguageServer.Transform.CombineGrammars(doc1, doc2);
        }

        public string Reconstruct(Node tree)
        {
            Stack<Node> stack = new Stack<Node>();
            stack.Push(tree);
            StringBuilder sb = new StringBuilder();
            while (stack.Any())
            {
                var n = stack.Pop();
                if (n is UnvParseTreeAttr a)
                {
                    sb.Append(a.StringValue);
                }
                else if (n is UnvParseTreeText t)
                {
                    sb.Append(t.NodeValue);
                }
                else if (n is UnvParseTreeElement e)
                {
                    for (int i = n.ChildNodes.Length - 1; i >= 0; i--)
                    {
                        stack.Push(n.ChildNodes.item(i));
                    }
                }
            }
            return sb.ToString();
        }
    }
}
