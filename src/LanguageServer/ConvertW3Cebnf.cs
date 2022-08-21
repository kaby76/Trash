namespace LanguageServer
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    // https://www.w3.org/TR/REC-xml/#sec-notation

    public class ConvertW3Cebnf
	{
		string suffix = ".ebnf";
		
        public ConvertW3Cebnf()
        {
        }

        public Dictionary<string, string> Try(string ffn, string input, string out_type = "antlr4")
        {
            Dictionary<string, string> results = new Dictionary<string, string>();
            var now = DateTime.Now.ToString();
            var errors = new StringBuilder();
            var str = new AntlrInputStream(input);
            var lexer = new W3CebnfLexer(str);
            var tokens = new CommonTokenStream(lexer);
            var parser = new W3CebnfParser(tokens);
            var elistener = new ErrorListener<IToken>(parser, lexer, 0);
            parser.AddErrorListener(elistener);
            var tree = parser.prods();
            var error_file_name = ffn;
            error_file_name = error_file_name.EndsWith(suffix)
                ? (error_file_name.Substring(0, error_file_name.Length - suffix.Length) + ".txt") : error_file_name;

            var new_ffn = ffn;
            new_ffn = new_ffn.EndsWith(suffix)
                ? (new_ffn.Substring(0, new_ffn.Length - suffix.Length) + ".g4") : new_ffn;

            if (elistener.had_error)
            {
                results.Add(error_file_name, errors.ToString());
                return results;
            }
            else
            {
                errors.AppendLine("File " + ffn + " parsed successfully.");
                errors.AppendLine("Date: " + now);
            }

            var name = System.IO.Path.GetFileName(ffn);
            name = System.IO.Path.GetFileNameWithoutExtension(ffn);

            var (text_before, other) = TreeEdits.TextToLeftOfLeaves(tokens, tree);

            TreeEdits.InsertBefore(tree.GetChild(0), "grammar " + name + ";" + Environment.NewLine);

            using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext =
                new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(tree, parser))
            {
                org.eclipse.wst.xml.xpath2.processor.Engine engine =
                    new org.eclipse.wst.xml.xpath2.processor.Engine();
                var nodes = engine.parseExpression(
                        @"//rhs",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree);
                foreach (var n in nodes) TreeEdits.InsertAfter(n, " ;");
            }

            using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext =
                new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(tree, parser))
            {
                org.eclipse.wst.xml.xpath2.processor.Engine engine =
                    new org.eclipse.wst.xml.xpath2.processor.Engine();
                var nodes = engine.parseExpression(
                        @"//PPEQ",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree);
                foreach (var n in nodes) TreeEdits.Replace(n,
                    ":");
            }

            // Convert double-quoted string literals to single quote.
            {
                using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext =
                    new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(tree, parser))
                {
                    org.eclipse.wst.xml.xpath2.processor.Engine engine =
                     new org.eclipse.wst.xml.xpath2.processor.Engine();
                    var nodes = engine.parseExpression(
                            @"//STRING",
                            new StaticContextBuilder()).evaluate(
                            dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree);
                    foreach (var n in nodes)
                    {
                        var text = n.GetText();
                        if (text.Length == 0) continue;
                        if (text[0] != '"') continue;
                        text = text.Substring(1, text.Length - 2);
                        StringBuilder ss = new StringBuilder();
                        ss.Append("'");
                        foreach (var c in text)
                        {
                            if (c == '"') ss.Append("\\");
                            else ss.Append(c);
                        }
                        ss.Append("'");
                        var new_sym = new TerminalNodeImpl(new CommonToken(ANTLRv4Lexer.STRING_LITERAL)
                        { Line = -1, Column = -1, Text = ss.ToString() });
                        text_before.TryGetValue(n as TerminalNodeImpl, out string v);
                        if (v != null)
                            text_before.Add(new_sym, v);
                        TreeEdits.Replace(n, new_sym);
                    }
                }
            }

            StringBuilder sb = new StringBuilder();
            TreeEdits.Reconstruct(sb, tree, text_before);
            var new_code = sb.ToString();
            results.Add(new_ffn, new_code);
            results.Add(ffn.Replace(suffix, ".txt"), errors.ToString());
            return results;
        }
    }
}
