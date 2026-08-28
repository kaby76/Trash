using AntlrJson;
using ParseTreeEditing.UnvParseTreeDOM;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Trash
{
    class Command
    {
        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trconvert.readme.md"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public void Execute(Config config)
        {
            var input = ParsingResultIO.Read(config.File);
            var data = input.Results;
            List<ParsingResultSet> results = new List<ParsingResultSet>();
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
                var ty = new Regex("Parser$").Replace(Path.GetFileNameWithoutExtension(parser.GrammarFileName), "");
                switch (ty)
                {
                    case "ANTLRv4":
                    {
                        switch (config.Type)
                        {
                            case "KocmanLLK":
                            {
                                ConvertAntlr4.ToKocmanLLK(trees, parser, lexer, fn);
                                var tuple = new ParsingResultSet()
                                {
                                    FileName = fn,
                                    Nodes = trees,
                                    Lexer = lexer,
                                    Parser = parser
                                };
                                results.Add(tuple);
                                break;
                            }
                            case "Pegjs":
                            {
                                ConvertAntlr4.ToPegjs(trees, parser, lexer, fn);
                                var tuple = new ParsingResultSet()
                                {
                                    FileName = fn,
                                    Nodes = trees,
                                    Lexer = lexer,
                                    Parser = parser
                                };
                                results.Add(tuple);
                                break;
                            }
                            default:
                                System.Console.WriteLine("Unhandled conversion to.");
                                break;
                        }
                        break;
                    }

                    case "ANTLRv3":
                    {
                        ConvertAntlr3.ToAntlr4(trees, parser, lexer, fn);
                        var tuple = new ParsingResultSet()
                        {
                            FileName = new Regex("[^.]+$").Replace(fn, "g4"),
                            Nodes = trees,
                            Lexer = lexer,
                            Parser = parser
                        };
                        results.Add(tuple);
                        break;
                    }

                    case "Bison":
                    {
                        ConvertBison.ToAntlr4(trees, parser, lexer, fn);
                        var tuple = new ParsingResultSet()
                        {
                            FileName = new Regex("[^.]+$").Replace(fn, "g4"),
                            Nodes = trees,
                            Lexer = lexer,
                            Parser = parser
                        };
                        results.Add(tuple);
                        break;
                    }

                    case "rex":
                    {
                        ConvertRex.ToAntlr4(trees, parser, lexer, fn);
                        var tuple = new ParsingResultSet()
                        {
                            FileName = fn,
                            Nodes = trees,
                            Lexer = lexer,
                            Parser = parser
                        };
                        results.Add(tuple);
                        break;
                    }

                    default:
                        {
                            System.Console.WriteLine("Unknown type for conversion.");
                            break;
                        }
                }
            }

            using var output = System.Console.OpenStandardOutput();
            ParsingResultIO.WriteBundle(output, input, results, config.Format);
        }
    }
}
