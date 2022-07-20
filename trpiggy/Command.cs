extern alias MainGlobbing;

namespace Trash
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using AntlrJson;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Dynamic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text.Json;
    using LanguageServer;
    using org.eclipse.wst.xml.xpath2.processor.util;

    class Command
    {
        Config _config;
        
        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trpiggy.readme.md"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public int Execute(Config config)
        {
            _config = config;
            string lines = null;
            if (!(_config.TreeFile != null && _config.TreeFile != ""))
            {
                if (_config.Verbose)
                {
                    System.Console.Error.WriteLine("reading from stdin");
                }
                for (; ; )
                {
                    lines = System.Console.In.ReadToEnd();
                    if (lines != null && lines != "") break;
                }
            }
            else
            {
                if (_config.Verbose)
                {
                    System.Console.Error.WriteLine("reading from file >>>" + _config.TreeFile + "<<<");
                }
                lines = File.ReadAllText(_config.TreeFile);
            }
            ICharStream str = CharStreams.fromPath(config.TemplateFile.First());
            var spec_lexer = new TreeMLLexer(str);
            var ts = new Antlr4.Runtime.CommonTokenStream(spec_lexer);
            var spec_parser = new TreeMLParser(ts);
            var spec = spec_parser.file_();
            var xml = string.Join("", lines);
            if (spec_parser.NumberOfSyntaxErrors > 0)
                return 0;
            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new AntlrJson.ParseTreeConverter());
            serializeOptions.WriteIndented = false;
            var data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
            var path = Environment.CurrentDirectory;
            var cd = Environment.CurrentDirectory.Replace('\\', '/') + "/";
            root_directory = cd;
            GenerateSingle(spec, data);
            return 0;
        }

        public static string version = "0.16.5";
        public List<string> all_source_files = null;
        public List<string> all_target_files = null;
        public string root_directory;
        public string suffix = "";
        public string ignore_string = null;


        public static LineTranslationType GetLineTranslationType()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return LineTranslationType.LF;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return LineTranslationType.CRLF;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return LineTranslationType.CR;
            }
            throw new Exception("Cannot determine operating system!");
        }

        public static EnvType GetEnvType()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return EnvType.Unix;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return EnvType.Windows;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return EnvType.Mac;
            }
            throw new Exception("Cannot determine operating system!");
        }

        public static PathSepType GetPathSep()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return PathSepType.Colon;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return PathSepType.Semi;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return PathSepType.Colon;
            }
            throw new Exception("Cannot determine operating system!");
        }


        public void GenerateSingle(TreeMLParser.File_Context spec_tree, AntlrJson.ParsingResultSet[] data)
        {
            // Collect all patterns.
            var pattern = spec_tree.patterns().pattern();
            var results = new List<ParsingResultSet>();
            bool do_rs = true;
            foreach (var parse_info in data)
            {
                var text = parse_info.Text;
                var fn = parse_info.FileName;
                var atrees = parse_info.Nodes;
                var parser = parse_info.Parser;
                var lexer = parse_info.Lexer;
                var tokstream = parse_info.Stream as AltAntlr.MyTokenStream;
                foreach (var pat in pattern)
                {
                    var expr = pat.xpath().GetText();
                    var template_source = pat.text().GetText();
                    var template_lexer = new TemplateLexer(CharStreams.fromString(template_source));
                    var template_parser = new TemplateParser(new Antlr4.Runtime.CommonTokenStream(template_lexer));
                    var template_tree = template_parser.file_();
                    org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                    var ate = new AntlrTreeEditing.AntlrDOM.ConvertToDOM();
                    using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = ate.Try(atrees, parser))
                    {
                        var nodes = engine.parseExpression(expr,
                                new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                            .Select(x => (x.NativeValue)).ToArray();
                        if (_config.Verbose) System.Console.Error.WriteLine("Result size " + nodes.Count());
                        List<IParseTree> res = new List<IParseTree>();
                        foreach (var v in nodes)
                        {
                            if (v is AntlrTreeEditing.AntlrDOM.AntlrElement)
                            {
                                var q = v as AntlrTreeEditing.AntlrDOM.AntlrElement;
                                var r = q.AntlrIParseTree as AltAntlr.MyParserRuleContext;
                                var leaf = TreeEdits.LeftMostToken(r);
                                var first_child = r.GetChild(0);
                                var place_holder = new AltAntlr.MyTerminalNodeImpl(new AltAntlr.MyToken() { Text = "xxx" }) ;
                                if (first_child != null)
                                {
                                    TreeEdits.InsertBeforeInStreams(tokstream, first_child, place_holder);
                                }
                                else
                                {
                                    r.AddChild(place_holder);
                                }
                                var old_children = r.children.ToList();
                                place_holder.Parent = r;
                                for (int c = 0; c < template_tree.ChildCount; ++c)
                                {
                                    var new_child = template_tree.GetChild(c);
                                    var new_intertext = new_child.GetText().Replace("{{","").Replace("}}","");
                                    var xx = new_child as TerminalNodeImpl;
                                    if (xx.Symbol.Type == TemplateLexer.Any)
                                    {
                                        var new_text = xx.Symbol.Text.Replace("{{", "").Replace("}}", "");
                                        if (new_text != null && new_text != "")
                                            TreeEdits.InsertBeforeInStreams(tokstream, r, new_text);
                                    }
                                    else
                                    {
                                        // From code, get xpath.
                                        var xpath_expr = xx.Symbol.Text.Substring(0, xx.Symbol.Text.Length - 1).Substring(1);
                                        var nodes2 = engine.parseExpression(xpath_expr,
                                                    new StaticContextBuilder()).evaluate(dynamicContext, new object[] { v })
                                                .Select(x => (x.NativeValue)).ToArray();
                                        if (_config.Verbose) System.Console.Error.WriteLine("Result size " + nodes2.Count());
                                        foreach (var z in nodes2)
                                        {
                                            if (z is AntlrTreeEditing.AntlrDOM.AntlrElement)
                                            {
                                                var q2 = z as AntlrTreeEditing.AntlrDOM.AntlrElement;
                                                var r2 = q2.AntlrIParseTree;
                                                if (r2 == null) throw new Exception("null value.");
                                                TreeEdits.MoveBefore(r2, place_holder);
                                            }
                                        }
                                    }

                                }
                                var before_del_frontier = TreeEdits.Frontier(atrees[0]).ToArray();
                                TreeEdits.Delete(tokstream, place_holder);
                                var after_del_frontier = TreeEdits.Frontier(atrees[0]).ToArray();
                                var sb = LanguageServer.TreeOutput.OutputTree(r, lexer, parser, null);
                            }
                        }
                    }
                }
                var parse_info_out = new AntlrJson.ParsingResultSet() { Text = text, FileName = fn, Lexer = lexer, Parser = parser, Stream = tokstream, Nodes = atrees };
                results.Add(parse_info_out);
            }
            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new AntlrJson.ParseTreeConverter());
            serializeOptions.WriteIndented = true;
            string js1 = JsonSerializer.Serialize(results.ToArray(), serializeOptions);
            System.Console.WriteLine(js1);
        }

        IEnumerable<string> EnumerateLines(TextReader reader)
        {
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                yield return line;
            }
        }

        string[] ReadAllResourceLines(System.Reflection.Assembly a, string resourceName)
        {
            using (Stream stream = a.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return EnumerateLines(reader).ToArray();
            }
        }

        string ReadAllResource(System.Reflection.Assembly a, string resourceName)
        {
            using (Stream stream = a.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public class Params
        {
            public string AttrName { get; set; }
            public string AttrValue { get; set; }
        }

        static string RemoveTrailingSlash(string str)
        {
            for (; ; )
            {
                if (str.EndsWith('/'))
                    str = str.Substring(0, str.Length - 1);
                else if (str.EndsWith('\\'))
                    str = str.Substring(0, str.Length - 1);
                else break;
            }
            return str;
        }

        static string Cap(string str)
        {
            if (str.Length == 0)
                return str;
            else if (str.Length == 1)
                return char.ToUpper(str[0]).ToString();
            else
                return char.ToUpper(str[0]) + str.Substring(1);
        }

        public void CopyFile(string from, string to)
        {
            from = from.Replace('\\', '/');
            to = to.Replace('\\', '/');
            var q = Path.GetDirectoryName(to).ToString().Replace('\\', '/');
            Directory.CreateDirectory(q);
            File.Copy(from, to, true);
        }

        public static string Localize(LineTranslationType encoding, string code)
        {
            var is_win = code.Contains("\r\n");
            var is_mac = code.Contains("\n\r");
            var is_uni = code.Contains("\n") && !(is_win || is_mac);
            if (encoding == LineTranslationType.CRLF)
            {
                if (is_win) return code;
                else if (is_mac) return code.Replace("\n\r", "\r\n");
                else if (is_uni) return code.Replace("\n", "\r\n");
                else return code;
            }
            if (encoding == LineTranslationType.CR)
            {
                if (is_win) return code.Replace("\r\n", "\n\r");
                else if (is_mac) return code;
                else if (is_uni) return code.Replace("\n", "\n\r");
                else return code;
            }
            if (encoding == LineTranslationType.LF)
            {
                if (is_win) return code.Replace("\r\n", "\n");
                else if (is_mac) return code.Replace("\n\r", "\n");
                else if (is_uni) return code;
                else return code;
            }
            return code;
        }
    }


    public class Goof : GetMemberBinder
    {
        public Goof(string name, bool ignoreCase) : base(name, ignoreCase)
        {
        }

        public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
        {
            return null;
        }
    }
}
