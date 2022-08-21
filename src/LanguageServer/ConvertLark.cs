namespace LanguageServer
{
    using Antlr4.Runtime;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ConvertLark
    {
        public ConvertLark() { }

        public Dictionary<string, string> Try(string ffn, string input, string out_type = "antlr4")
        {
            Dictionary<string, string> results = new Dictionary<string, string>();
            bool convert_undefined_to_terminals = true;
            string now = DateTime.Now.ToString();
            StringBuilder errors = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            AntlrInputStream str = new AntlrInputStream(input);
            LarkLexer lexer = new LarkLexer(str);
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            LarkParser parser = new LarkParser(tokens);
            var elistener = new ErrorListener<IToken>(parser, lexer, 0);
            parser.AddErrorListener(elistener);
            var tree = parser.start();
            string error_file_name = ffn.Replace(".g4", ".txt");
            if (elistener.had_error)
            {
                results.Add(ffn.Replace(".lark", ".txt"), errors.ToString());
                return results;
            }
            else
            {
                errors.AppendLine("File " + ffn + " parsed successfully.");
                errors.AppendLine("Date: " + now);
            }

            // Get list of tokens. Convert this to a list of capitalized names.
            Dictionary<string, string> terminals = new Dictionary<string, string>();
            Dictionary<string, string> nonterminals = new Dictionary<string, string>();
            List<Tuple<string, List<List<string>>>> rules = new List<Tuple<string, List<List<string>>>>();

            // Collect terminals.
            using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext =
                    new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(tree, parser))
            {
                org.eclipse.wst.xml.xpath2.processor.Engine engine =
                    new org.eclipse.wst.xml.xpath2.processor.Engine();
                var nodes = engine.parseExpression(
                        @"//token_decls//token_decl/id[position() = 1]",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree);
                foreach (var token in nodes)
                {
                    string tok = token.GetText();
                    string cap_tok = tok.Length == 1 ? char.ToUpper(tok[0]).ToString() :
                        (char.ToUpper(tok[0]) + tok.Substring(1));
                    terminals.Add(tok, cap_tok);
                }
            }

            // Collect rules.
            using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext =
                    new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(tree, parser))
            {
                org.eclipse.wst.xml.xpath2.processor.Engine engine =
                    new org.eclipse.wst.xml.xpath2.processor.Engine();
                var nodes = engine.parseExpression(
                        @"//rules",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement));
                foreach (var rule in nodes)
                {
                    var r = rule.AntlrIParseTree;
                    var lhs = r.GetChild(0).GetText();
                    string cap_tok = lhs.Length == 1 ? char.ToLower(lhs[0]).ToString() :
                        (char.ToLower(lhs[0]) + lhs.Substring(1));
                    nonterminals[lhs] = cap_tok;
                    var rhs = new List<List<string>>();
                    var rhses = engine.parseExpression(
                            @".//rhses_1/rhs",
                            new StaticContextBuilder()).evaluate(
                            dynamicContext, new object[] { rule })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement));
                    foreach (var r1 in rhses)
                    {
                        rhs.Add(new List<string>());
                        var sym = engine.parseExpression(
                                @".//symbol",
                                new StaticContextBuilder()).evaluate(
                                dynamicContext, new object[] { r1 })
                            .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree);
                        foreach (var s in sym)
                        {
                            List<string> l = rhs.Last();
                            l.Insert(0, s.GetText());
                        }
                    }
                    rules.Add(new Tuple<string, List<List<string>>>(lhs, rhs));
                }
            }

            // First, collect information about the grammar.
  //          BisonGrammarListener listener = new BisonGrammarListener();
  //          ParseTreeWalker.Default.Walk(listener, tree);


            if (convert_undefined_to_terminals)
            {
                foreach (Tuple<string, List<List<string>>> r in rules)
                {
                    List<List<string>> rhs = r.Item2;
                    foreach (List<string> s in rhs)
                    {
                        foreach (string c in s)
                        {
                            if (rules.Where(rr => rr.Item1 == c).Any())
                            {
                                continue;
                            }

                            if (terminals.ContainsKey(c))
                            {
                                continue;
                            }

                            if (c[0] == '\'')
                            {
                                continue;
                            }

                            if (c == "error")
                            {
                                continue;
                            }
                            // RHS symbol is not a non-terminal and not a %token terminal.
                            // Enter it as a terminal.
                            string tok = c;
                            string cap_tok = tok.Length == 1 ? char.ToUpper(tok[0]).ToString() :
                                (char.ToUpper(tok[0]) + tok.Substring(1));
                            terminals.Add(tok, cap_tok);
                            // Note that in the error list.
                            errors.AppendLine("Symbol " + c + " not declared, assuming it is a terminal.");
                        }
                    }
                }
            }

            // Convert any nonterminals which are keywords in Antlr...
            foreach (Tuple<string, List<List<string>>> r in rules)
            {
                if (!terminals.ContainsKey(r.Item1)
                    && (r.Item1 == "options"
                        || r.Item1 == "grammar"
                        || r.Item1 == "tokenVocab"
                        || r.Item1 == "lexer"
                        || r.Item1 == "parser"
                        || r.Item1 == "rule"
                    ))
                {
                    if (!nonterminals.ContainsKey(r.Item1))
                        nonterminals.Add(r.Item1, r.Item1 + "_nonterminal");
                }
                else
                {
                    if (! nonterminals.ContainsKey(r.Item1))
                        nonterminals.Add(r.Item1, r.Item1);
                }
            }

            // Get the name of the grammar.
            string name = System.IO.Path.GetFileNameWithoutExtension(ffn);
            sb.AppendLine("// Combined Antlr4 grammar generated.");
            sb.AppendLine("// Input grammar: " + ffn);
            sb.AppendLine("// Date: " + now);
            sb.AppendLine();
            sb.AppendLine("grammar " + name + ";");

            // Output the rules.
            foreach (Tuple<string, List<List<string>>> r in rules)
            {
                string lhs = r.Item1;
                lhs = nonterminals[lhs];
                nonterminals.TryGetValue(lhs, out string nlhs);
                if (nlhs != null && nlhs != "") lhs = nlhs;
                sb.Append(lhs);
                List<List<string>> rhs = r.Item2;
                bool first = true;
                foreach (List<string> s in rhs)
                {
                    sb.Append(first ? "  :" : "  |");
                    first = false;
                    foreach (string c in s)
                    {
                        var re = c;
                        if (terminals.ContainsKey(re)) re = terminals[re];
                        else if (nonterminals.ContainsKey(re)) re = nonterminals[re];
                        sb.Append(" " + re);
                    }

                    sb.AppendLine();
                }
                sb.AppendLine("  ;");
            }

            // Check for use of error symbol. The semantics is for the parser to look for
            // the ERROR token.
            bool found = false;
            foreach (Tuple<string, List<List<string>>> r in rules)
            {
                List<List<string>> rhs = r.Item2;
                foreach (List<string> s in rhs)
                {
                    foreach (string c in s)
                    {
                        if (c == "error")
                        {
                            found = true;
                            goto bigexit;
                        }
                    }
                }
            }
        bigexit:
            if (found)
            {
                sb.AppendLine("error : ERROR ;");
            }
            results.Add(ffn.Replace(".lark", ".g4"), sb.ToString());
            results.Add(ffn.Replace(".lark", ".txt"), errors.ToString());
            return results;
        }
    }
}
