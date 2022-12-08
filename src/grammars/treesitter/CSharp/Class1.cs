using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using org.eclipse.wst.xml.xpath2.processor.util;
using System.Linq;

public class Class1 : JavaScriptParserBaseVisitor<object>
{
    public override object VisitSingleExpression([NotNull] JavaScriptParser.SingleExpressionContext context)
    {
        var name = context?.singleExpression()?.FirstOrDefault()?.identifier()?.Identifier()?.GetText();
        var name2 = context?.singleExpression()?.FirstOrDefault()?.singleExpression()?.FirstOrDefault()?.identifier()?.Identifier()?.GetText();
        if (name == "seq")
        {
            var args = context.arguments().argument();
            System.Console.Write("(");
            foreach (var a in args)
            {
                Visit(a);
            }
            System.Console.Write(" )");
        }
        else if (name == "choice")
        {
            var args = context.arguments().argument();
            System.Console.Write("(");
            bool first = true;
            foreach (var a in args)
            {
                if (!first) System.Console.Write(" | ");
                first = false;
                Visit(a);
            }
            System.Console.Write(" )");
        }
        else if (name == "repeat")
        {
            var args = context.arguments().argument();
            System.Console.Write("(");
            foreach (var a in args)
            {
                Visit(a);
            }
            System.Console.Write(" )* ");
        }
        else if (name == "repeat1")
        {
            var args = context.arguments().argument();
            System.Console.Write("(");
            foreach (var a in args)
            {
                Visit(a);
            }
            System.Console.Write(" )+ ");
        }
        else if (name == "optional")
        {
            var args = context.arguments().argument();
            System.Console.Write("(");
            foreach (var a in args)
            {
                Visit(a);
            }
            System.Console.Write(" )? ");
        }
        else if (name2 == "prec")
        {
            var args = context.arguments().argument();
            System.Console.Write("(");
            var count = args.Count();
            var rest = args;
            if (count == 0)
            { }
            else if (count == 1)
            {
                Visit(rest.First());
            }
            else
            {
                foreach (var a in args.Skip(1))
                {
                    Visit(a);
                }
            }
            System.Console.Write(" ) ");
        }
        else if (name == "prec")
        {
            var args = context.arguments().argument();
            System.Console.Write("(");
            foreach (var a in args.Skip(1))
            {
                Visit(a);
            }
            System.Console.Write(" ) ");
        }
        else if (name == "token")
        {
            var args = context.arguments().argument();
            System.Console.Write("(");
            foreach (var a in args)
            {
                Visit(a);
            }
            System.Console.Write(" ) ");
        }
        else if (name == "field")
        {
            var args = context.arguments().argument();
            System.Console.Write("(");
            foreach (var a in args)
            {
                Visit(a);
            }
            System.Console.Write(" ) ");
        }
        else if (name == "$")
        {
            var nt = context.identifierName()?.identifier()?.Identifier()?.GetText();
            System.Console.Write(" " + nt);
        }
        else if (context.literal() != null)
        {
            Visit(context.literal());
        }
        return null;
    }

    public override object VisitLiteral([NotNull] JavaScriptParser.LiteralContext context)
    {
        System.Console.Write(" " + context.GetText());
        return Visit(context.GetChild(0));
    }

    public static void MyMain(IParseTree tree, Parser parser)
    {
        var visitor = new Class1();
        using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(tree, parser))
        {
            org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
            var nodes = engine.parseExpression(
                @"
	//propertyAssignment[propertyName/identifierName/identifier/Identifier[text()='rules']]
			    /singleExpression/objectLiteral/propertyAssignment
			    ",
                new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();

            foreach (var r in nodes)
            {
                var lhs = (r as JavaScriptParser.PropertyAssignmentContext)?.propertyName()?.identifierName()?.identifier()?.Identifier()?.GetText();
                var rhs = (r as JavaScriptParser.PropertyAssignmentContext)?
                      .singleExpression()?.FirstOrDefault()?.anonymousFunction()?.arrowFunctionBody();
                System.Console.Write(lhs + " : ");
                visitor.Visit(rhs);
                System.Console.Write(" ;");
                System.Console.WriteLine();
            }
        }
    }
}
