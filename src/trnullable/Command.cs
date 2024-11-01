using org.eclipse.wst.xml.xpath2.processor.util;
using ParseTreeEditing.UnvParseTreeDOM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;

namespace Trash.trnullable;

class Command
{
    Dictionary<string, bool> is_nullable = new Dictionary<string, bool>();

    public string Help()
    {
        using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trnullable.readme.md"))
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

            for (;;)
            {
                lines = System.Console.In.ReadToEnd();
                if (lines != null && lines != "")
                {
                    break;
                }
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
        if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("starting deserialization");
        var data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
        if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("deserialized");
        foreach (var in_tuple in data)
        {
            var nodes = in_tuple.Nodes;
            var lexer = in_tuple.Lexer;
            var parser = in_tuple.Parser;
            var fn = in_tuple.FileName;
            foreach (var node in nodes)
            {
                org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                var ate = new ParseTreeEditing.UnvParseTreeDOM.ConvertToDOM();
                ParseTreeEditing.UnvParseTreeDOM.AntlrDynamicContext dynamicContext = ate.Try(node);

                List<UnvParseTreeElement> is_par = null;
                List<UnvParseTreeElement> is_lex = null;
                List<string> name_ = null;
                List<string> ss = null;
                is_par = engine.parseExpression(
                        @"/grammarSpec/grammarDecl/grammarType/PARSER",
                        new StaticContextBuilder()).evaluate(dynamicContext,
                        new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement))
                    .ToList();
                is_lex = engine.parseExpression(
                        @"/grammarSpec/grammarDecl/grammarType/LEXER",
                        new StaticContextBuilder()).evaluate(dynamicContext,
                        new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                name_ = engine.parseExpression(
                        @"/grammarSpec/grammarDecl/identifier/(TOKEN_REF | RULE_REF)/text()",
                        new StaticContextBuilder()).evaluate(dynamicContext,
                        new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as UnvParseTreeText).NodeValue as string).ToList();
                ss = engine.parseExpression(
                        @"//parserRuleSpec[ruleBlock//TOKEN_REF/text()='EOF']/RULE_REF/text()",
                        new StaticContextBuilder()).evaluate(dynamicContext,
                        new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as UnvParseTreeText).NodeValue as string).ToList();

                var is_parser_grammar = is_par.Count() != 0;
                var is_lexer_grammar = is_lex.Count() != 0;
                var is_combined = !is_parser_grammar && !is_lexer_grammar;
                var grammar_name = name_.First();
                var start_symbol = ss.FirstOrDefault();
                if (!is_parser_grammar && !is_combined)
                {
                    continue;
                }
                // Gather up parser rules.
                var parser_rules = new List<UnvParseTreeElement>();
                parser_rules = engine.parseExpression(
                        @"//parserRuleSpec",
                        new StaticContextBuilder()).evaluate(dynamicContext,
                        new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as UnvParseTreeElement)).ToList();

                bool changed;
                do
                {
                    changed = false;
                    foreach (var parser_rule in parser_rules)
                    {
                        var name = parser_rule
                            .GetChildren("RULE_REF")
                            .First()
                            .GetChildrenText()
                            .First();

                        if (is_nullable.ContainsKey(name))
                        {
                            continue;
                        }
                        // Check if this rule is nullable.
                        bool? v = CheckRHS(parser_rule);
                        switch (v)
                        {
                            case true:
                            case false:
                                is_nullable.Add(name, (bool)v);
                                changed = true;
                                break;
                            case null:
                                break;
                        }
                    }
                } while (changed);
                foreach (var pair in is_nullable)
                {
                    System.Console.WriteLine(pair.Key + " " + pair.Value);
                }
            }
        }
    }

    private bool? CheckRHS(UnvParseTreeElement n)
    {
        var c = n.GetChildren("ruleBlock").First();
        return VisitRuleBlock(c);
    }

    private bool? VisitRuleBlock(UnvParseTreeElement n)
    {
        var c = n.GetChildren("ruleAltList").First();
        return VisitRuleAltList(c);
    }

    private bool? VisitRuleAltList(UnvParseTreeElement n)
    {
        var cl = n.GetChildren("labeledAlt");
        bool? result = false;
        foreach (var c in cl)
        {
            var rc = VisitLabeledAlt(c);
            if (rc == null)
            {
                result = null;
                continue;
            }
            var b = (bool)rc;
            if (b) return true;
        }
        return result;
    }

    private bool? VisitAltList(UnvParseTreeElement n)
    {
        var cl = n.GetChildren("alternative");
        bool? result = false;
        foreach (var c in cl)
        {
            var rc = VisitAlternative(c);
            if (rc == null)
            {
                result = null;
                continue;
            }
            var b = (bool)rc;
            if (b) return true;
        }
        return result;
    }

    public bool? VisitLabeledAlt(UnvParseTreeElement n)
    {
        var c = n.GetChildren("alternative").First();
        var rc = VisitAlternative(c);
        if (rc == null) return null;
        var b = (bool)rc;
        if (b) return true;
        else return false;
    }

    public bool? VisitAlternative(UnvParseTreeElement n)
    {
        var cl = n.GetChildren("element");
        if (!cl.Any()) return true;
        bool? result = true;
        foreach (var c in cl)
        {
            var rc = VisitElement(c);
            if (rc == null)
            {
                result = null;
                continue;
            }
            var b = (bool)rc;
            if (!b) return false;
        }
        return result;
    }

    public bool? VisitElement(UnvParseTreeElement n)
    {
        var c = n.Children.First();
        var s = n.GetChildren("ebnfSuffix").FirstOrDefault();
        if (s != null)
        {
            // Determine type of suffix.
            if (s.GetChildren("QUESTION").Any())
            {
                // Optional.
                return true;
            }
            else if (s.GetChildren("STAR").Any())
            {
                // Zero or more.
                return true;
            }
            else if (s.GetChildren("PLUS").Any())
            {
                // One or more.
                // Fall through to determine if item is nullable.
            }
        }
        bool? result = null;
        switch (c.GetName())
        {
            case "labeledElement":
                {
                    var v = VisitLabeledElement(c);
                    if (v == null) return null;
                    else if (v == true) return true;
                    else return false;
                    break;
                }
            case "atom":
                {
                    var v = VisitAtom(c);
                    if (v == null) return null;
                    else if (v == true) return true;
                    else return false;
                    break;
                }
            case "ebnf":
                {
                    var v = VisitEbnf(c);
                    if (v == null) return null;
                    else if (v == true) return true;
                    else return false;
                    break;
                }
            default:
                break;
        }
        return result;
    }

    private bool? VisitLabeledElement(UnvParseTreeElement n)
    {
        var a = n.GetChildren("atom").FirstOrDefault();
        var b = n.GetChildren("block").FirstOrDefault();
        if (a != null)
        {
            return VisitAtom(a);
        }
        else if (b != null)
        {
            return VisitRuleBlock(b);
        }
        else
        {
            return false;
        }
    }

    private bool? VisitAtom(UnvParseTreeElement n)
    {
        var c = n.Children.First();
        switch (c.GetName())
        {
            case "terminalDef":
                return false;
            case "ruleref":
                return VisitRuleref(c);
            case "notSet":
                return false;
            case "DOT":
                return false;
            default:
                return false;
        }
    }

    private bool? VisitRuleref(UnvParseTreeElement n)
    {
        var rr = n.GetChildren("RULE_REF").First();
        var t = rr.GetChildrenText().First();
        if (Char.IsUpper(t[0]))
            return false;
        // Find rule in is_nullable.
        if (is_nullable.ContainsKey(t))
        {
            return is_nullable[t];
        }
        else
        {
            return null;
        }
    }

    private bool? VisitEbnf(UnvParseTreeElement n)
    {
        var b = n.GetChildren("block").First();
        var s = n.GetChildren("blockSuffix").FirstOrDefault();
        if (s != null)
        {
            s = s.GetChildren("ebnfSuffix").First();
            // Determine type of suffix.
            if (s.GetChildren("QUESTION").Any())
            {
                // Optional.
                return true;
            }
            else if (s.GetChildren("STAR").Any())
            {
                // Zero or more.
                return true;
            }
            else if (s.GetChildren("PLUS").Any())
            {
                // One or more.
                // Fall through to determine if item is nullable.
            }
        }
        return VisitBlock(b);
    }

    private bool? VisitBlock(UnvParseTreeElement n)
    {
        var c = n.GetChildren("altList").First();
        return VisitAltList(c);
    }

}

