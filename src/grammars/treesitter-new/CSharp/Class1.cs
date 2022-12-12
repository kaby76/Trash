using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using org.eclipse.wst.xml.xpath2.processor.util;
using System.Linq;
using System.Text;
using System;
using Workspaces;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;


// Best discussion of code that converts json to ebnf.
// https://github.com/tree-sitter/tree-sitter/issues/1013#issuecomment-805787544

public class Class1 : JSON5BaseVisitor<object>
{
    public void Start(JSON5Parser.Json5Context node)
    {
        if (node == null) return;
        var sb = new StringBuilder();
        sb.AppendLine("grammar foo;");
        sb.AppendLine();
        using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(_tree, _parser))
        {
            org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
            var nodes = engine.parseExpression(
                "/json5/value/obj/pair[key/STRING/text()='\"rules\"']/value/obj/pair",
                new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree as JSON5Parser.PairContext).ToList();
            foreach (var r in nodes)
            {
                var lhs = r.key()?.GetText().Replace("\"","");
                var rhs = r.value();
                sb.Append(Fix(lhs) + " : ");
				toEBNF(rhs, sb);
                sb.Append(" ;");
                sb.AppendLine();
                sb.AppendLine();
            }
        }
        var grammar = sb.ToString();
        // Parse.
        Document doc = DDD.CreateDoc(grammar);
        DDD.ParseDoc(doc, 10);
        var result = LanguageServer.Transform.RemoveUselessParentheses(doc);
        if (result.Count > 0)
        {
            System.Console.WriteLine(result.First().Value.Replace("  ", " "));
        }
    }

    private IParseTree _tree;
    private Parser _parser;

    private Class1(IParseTree tree, Parser parser)
    {
        _tree = tree;
        _parser = parser;
    }

    public static void MyMain(IParseTree tree, Parser parser)
    {
        var visitor = new Class1(tree, parser);
        visitor.Start(tree as JSON5Parser.Json5Context);
    }

    public string TypeOf(JSON5Parser.ValueContext value)
    {
        return value.obj().pair().Where(p => p.key().STRING().GetText() == "\"type\"").Select(x => x.value().STRING().GetText().Replace("\"", "").ToUpper()).First();
    }

    public string NameOf(JSON5Parser.ValueContext value)
    {
        return value.obj().pair().Where(p => p.key().STRING().GetText() == "\"name\"").Select(x => x.value().STRING().GetText().Replace("\"", "")).First();
    }

    public string ValueOf(JSON5Parser.ValueContext value)
    {
        return value.obj().pair().Where(p => p.key().STRING().GetText() == "\"value\"").Select(x => x.value().STRING().GetText().Replace("\"", "")).First();
    }

    public JSON5Parser.ValueContext Context(JSON5Parser.ValueContext value)
	{
        var v1 = value.obj();
        var v2 = v1.pair();
        var v3 = v2.Where(p => p.key().STRING().GetText() == "\"content\"").ToList();
        var v4 = v3.Select(p => p.value()).ToList();
        var v5 = v4.First();
        return v5;
		//return value.obj().pair().Where(p => p.key().STRING().GetText() == "\"context\"").Select(x => x.value()).First();
	}

	public System.Collections.Generic.List<JSON5Parser.ValueContext> Members(JSON5Parser.ValueContext value)
	{
        System.Collections.Generic.List<JSON5Parser.ValueContext> z = value.obj().pair().Where(p => p.key().STRING().GetText() == "\"members\"").Select(x => x.value().arr()).SelectMany(a => a.value()).ToList();
		return z;
	}

	private void toEBNF(JSON5Parser.ValueContext value, StringBuilder sb)
	{
		var t = TypeOf(value);
		switch (t)
		{
			case "ALIAS":
				sb.Append(" ( ");
				toEBNF(Context(value), sb);
				sb.Append(" ) ");
				break;

			case "CHOICE":
				{
					var members = Members(value);
					sb.Append(" ( ");
					bool first = true;
					foreach (var v in members)
					{
						if (!first) sb.Append(" | ");
						toEBNF(v, sb);
                        first = false;
					}
					sb.Append(" ) ");
				}
				break;

            case "REPEAT":
                sb.Append(" ( ");
                toEBNF(Context(value), sb);
                sb.Append(" )* ");
                break;

            case "REPEAT1":
                sb.Append(" ( ");
                toEBNF(Context(value), sb);
                sb.Append(" )+ ");
                break;

            case "BLANK":
                sb.Append(" empty ");
                break;

            case "FIELD":
                // Fields are equivalent to rule element labels in Antlr.
                // https://github.com/antlr/antlr4/blob/master/doc/parser-rules.md#rule-element-labels
                sb.Append(" " + NameOf(value) + " = ");
                toEBNF(Context(value), sb);
                break;

           case "IMMEDIATE_TOKEN":
                sb.Append(" ( /* no preceeding ws */ ");
                toEBNF(Context(value), sb);
                sb.Append(" ) ");
                break;

            case "PATTERN":
                {
                    var p = ValueOf(value).Replace("\\d", "[0-9]");
                    sb.Append(" '" + PerformEscapes(p) + "' ");
                }
                break;

            case "PREC":
            case "PREC_DYNAMIC":
            case "PREC_LEFT":
            case "PREC_RIGHT":
                sb.Append(" ( ");
                toEBNF(Context(value), sb);
                sb.Append(" ) ");
                break;

            case "SEQ":
				{
					var members = Members(value);
					sb.Append(" ( ");
					bool first = true;
					foreach (var v in members)
					{
						toEBNF(v, sb);
					}
					sb.Append(" ) ");
				}
				break;

            case "TOKEN":
                sb.Append(" ( ");
                toEBNF(Context(value), sb);
                sb.Append(" ) ");
                break;

            case "SYMBOL":
                sb.Append(" " + Fix(NameOf(value)) + " ");
                break;

            case "STRING":
                sb.Append(" '" + PerformEscapes(ValueOf(value)) + "' ");
                break;

            default:
                System.Console.Error.WriteLine("Not handled " + t);
				break;
            //    throw ("Unknown rule type: " + rule.type);
        }
	}

    string Fix(string before)
    {
        if (before.Length > 1 && before[0] == '_') before = before.Substring(1) + "_";
        return before;
    }

    string ToLiteral(string input)
    {
        using (StringWriter writer = new StringWriter())
        {
            string literal = Regex.Escape(input);
            literal = literal.Replace(string.Format("\" +{0}\t\"", Environment.NewLine), "");
            return literal;
        }
    }

    string PerformEscapes(string s)
    {
        StringBuilder new_s = new StringBuilder();
        new_s.Append(s.Replace("\\","\\\\").Replace("'","\\'"));
        return new_s.ToString();
    }

    public class DDD
    {
        public static Workspace _workspace { get; set; } = new Workspace();

        public static int QuietAfter = 10;

        public static void ParseDoc(Document document, int quiet_after, string grammar = null)
        {
            document.Changed = true;
            if (grammar != null) document.ParseAs = grammar;
            var pd = LanguageServer.ParsingResultsFactory.Create(document);
            if (pd != null) pd.QuietAfter = quiet_after;
            var workspace = document.Workspace;
            _ = new LanguageServer.Module().Compile(workspace);
        }

        public static Document ReadDoc(string path)
        {
            string file_name = path;
            Document document = _workspace.FindDocument(file_name);
            if (document == null)
            {
                throw new Exception("File does not exist.");
            }
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(file_name))
                {
                    // Read the stream to a string, and write the string to the console.
                    string str = sr.ReadToEnd();
                    document.Code = str;
                }
            }
            catch (IOException)
            {
                throw;
            }
            return document;
        }

        public static Document CreateDoc(string code)
        {
            string file_name = "temp.g4";
            Document document = new Workspaces.Document(file_name);
            Project project = _workspace.FindProject("Misc");
            if (project == null)
            {
                project = new Project("Misc", "Misc", "Misc");
                _workspace.AddChild(project);
            }
            project.AddDocument(document);
            document.Code = code;
            var pr = LanguageServer.ParsingResultsFactory.Create(document);
            return document;
        }

        public static Document CreateDoc(string path, string code)
        {
            string file_name = path;
            Document document = _workspace.FindDocument(file_name);
            if (document == null)
            {
                document = new Workspaces.Document(file_name);
                Project project = _workspace.FindProject("Misc");
                if (project == null)
                {
                    project = new Project("Misc", "Misc", "Misc");
                    _workspace.AddChild(project);
                }
                project.AddDocument(document);
            }
            document.Code = code;
            return document;
        }

        public static void EnactEdits(Dictionary<string, string> results)
        {
            if (results.Count > 0)
            {
                foreach (var res in results)
                {
                    if (res.Value == null) continue;
                    var new_doc = CreateDoc(res.Key, res.Value);
                    ParseDoc(new_doc, QuietAfter);
                }
            }
            else
            {
                System.Console.Error.WriteLine("no changes");
            }
        }
    }
}
