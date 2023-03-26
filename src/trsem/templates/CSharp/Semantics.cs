// Template generated code from trsem <version>

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using org.eclipse.wst.xml.xpath2.processor.util;
using System.Collections.Generic;
using System.Linq;

class Scoping
{
    public Dictionary\<TerminalNodeImpl, Classifications> classified { get; private set; } = new Dictionary<TerminalNodeImpl, Classifications>();

    public enum Classifications : int
    {<classifiers:{x |
        <x.Name>,}>
    }

    public Parser parser { get; set; }
    public IParseTree root { get; set; }
    ParseTreeEditing.AntlrDOM.ConvertToDOM ate = null;
    ParseTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = null;

    public void Walk()
    {
        ate = new ParseTreeEditing.AntlrDOM.ConvertToDOM();
        dynamicContext = ate.Try(root, parser);
        Visit(root);
    }

    public void Visit(IParseTree tree)
    {
        if (tree == null) return;
        if (tree is ParserRuleContext internal_node)
        {
            for (int i = 0; i \< internal_node.ChildCount; ++i)
            {
                var c = internal_node.GetChild(i);
                Visit(c);
            }
            return;
        }
        var terminal = tree as TerminalNodeImpl;
        var symbol = terminal.Symbol;
        if (symbol == null) return;

<classifiers:{x |
        if (symbol.Type == <ParserName>.<x.NodeName>)
        {
            org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
            var l = ate.FindDomNode(terminal);
            var nodes = engine.parseExpression("<x.Classifier>",
                    new StaticContextBuilder()).evaluate(dynamicContext, new object[] { l \})
                .Select(x => (x.NativeValue));
            if (nodes.Any())
            {
                classified[terminal] = Classifications.<x.Name>;
            \}
        \}
}>
    }
}
