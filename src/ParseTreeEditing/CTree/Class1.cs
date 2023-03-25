using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CTree
{
    public class Class1 : AstParserBaseVisitor<List<IParseTree>>
    {
        private Parser _parser;
        //private Lexer _lexer;
        private Dictionary<string, object> _env;

        public Class1(Parser parser, Dictionary<string, object> env)
        {
            _parser = parser;
            _env = env;
        }

        public IParseTree CreateTree(string ast_string)
        {
            var ast_stream = CharStreams.fromString(ast_string);
            var ast_lexer = new AstLexer(ast_stream);
            var ast_tokens = new CommonTokenStream(ast_lexer);
            var ast_parser = new AstParserParser(ast_tokens);
            ast_parser.BuildParseTree = true;
            var listener = new ErrorListener<IToken>();
            ast_parser.AddErrorListener(listener);
            IParseTree ast = ast_parser.ast();
            if (listener.had_error) throw new Exception();
            var convert = Convert(ast);
            return convert.First();
        }

        private List<IParseTree> Convert(IParseTree tree)
        {
            var result = this.Visit(tree);
            return result;
        }

        public override List<IParseTree> VisitAst(AstParserParser.AstContext context)
        {
            return VisitNode(context.node());
        }

        public override List<IParseTree> VisitNode(AstParserParser.NodeContext context)
        {
            var id = context.ID();
            var id_name = id.GetText().ToLower() + "context";
            var pt = _parser.GetType();
            var list = pt.GetTypeInfo().DeclaredNestedTypes.Where(t => t.Name.ToLower() == id_name);
            if (list.Count() != 1)
            {
                throw new Exception();
            }
            TypeInfo mapped_type = list.First();
            var mapped_node = Activator.CreateInstance(mapped_type, new object[] {null, 0}) as IParseTree;
            foreach (var c in context.children)
            {
                if (c is AstParserParser.NodeContext)
                {
                    var mcl = VisitNode(c as AstParserParser.NodeContext);
                    var mc = mcl.First();
                    if (mc is TerminalNodeImpl)
                    {
                        var _mc = mc as TerminalNodeImpl;
                        var _mapped_node = mapped_node as ParserRuleContext;
                        _mc.Parent = _mapped_node;
                        _mapped_node.AddChild(_mc);
                    }
                    else
                    {
                        var _mc = mc as ParserRuleContext;
                        var _mapped_node = mapped_node as ParserRuleContext;
                        _mc.Parent = _mapped_node;
                        _mapped_node.AddChild(_mc);
                    }
                }
                else if (c is AstParserParser.ValContext)
                {
                    var mcl = VisitVal(c as AstParserParser.ValContext);
                    if (mcl != null)
                    {
                        foreach (var mc in mcl)
                        {
                            if (mc is TerminalNodeImpl)
                            {
                                var _mc = mc as TerminalNodeImpl;
                                var _mapped_node = mapped_node as ParserRuleContext;
                                _mc.Parent = _mapped_node;
                                _mapped_node.AddChild(_mc);
                            }
                            else
                            {
                                var _mc = mc as ParserRuleContext;
                                var _mapped_node = mapped_node as ParserRuleContext;
                                _mc.Parent = _mapped_node;
                                _mapped_node.AddChild(_mc);
                            }
                        }
                    }
                }
            }
            return new List<IParseTree>() { mapped_node };
        }

        public override List<IParseTree> VisitVal(AstParserParser.ValContext context)
        {
            var id = context.ID();
            var id_name = id.GetText();
            if (!_env.ContainsKey(id_name))
                return null;
            var val = _env[id_name];
            if (val is List<IParseTree>)
                return val as List<IParseTree>;
            return new List<IParseTree>() { val as IParseTree };
        }
    }
}
