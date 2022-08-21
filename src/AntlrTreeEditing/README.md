# AntlrTreeEditing

This library contains a collection of routines for editing
Antlr4 parse trees. [As noted by Parr](https://theantlrguy.atlassian.net/wiki/spaces/~admin/blog/2012/12/08/524353/Tree+rewriting+in+ANTLR+v4),
Antlr4 does not have
methods to do tree transformations. Parr felt that editing
trees was not necessary because the focus was parsing, information
extraction, and translation. For information extraction, Antlr
includes a rudimentary XPath engine, but it's a far cry from
XPath v1 let alone XPath v3. For translation, Parr assumes you would
be using string templates, or possibly something like ASF+SDF in the future.
Antlr does itself perform some tree rewriting, but it does so
"manually". The methods are cumbersome and error-prone when used.

This library tries to fill in the gap slightly between the very low-level 
parse node editing methods and a full-blown transformation system like
ASF+SDF with a few practical routines. It contains:

* a repurposed and clean up port of the Eclipse XPath v2 for use with
Antlr parse trees;
* a tree construction method from an s-expression notation and
an Antlr parser and lexer;
* node replace and delete methods.
* an observer parse tree node so as to allow notification of observers
when a parse tree changes.

# XPath

The XPath engine included here is based on a port of
the Eclipse XPath engine,
 which is part of the [XSLT webtools](https://wiki.eclipse.org/XSLT_Project).
This engine was known as the
[Pychopath XPath engine](http://psychopath.sourceforge.net/). The parser was initally
a hand-written parser, but Harwell made an Antlr parser grammar. I updated the parser
grammar from version 2 to version 3, and redid most of the tree visitor walker that converts
the parse tree into the AST represention used by the engine. I also added output
methods to print out the AST. I also
added a wrapper to Antlr parse trees to make it look like
a "DOM".

    org.eclipse.wst.xml.xpath2.processor.Engine engine = new Engine();
    var input = System.IO.File.ReadAllText("../AntlrDOM/ANTLRv4Parser.g4");
    var (tree, parser, lexer) = AntlrDOM.Parse.Try(input);
    AntlrDynamicContext dynamicContext = AntlrDOM.ConvertToDOM.Try(tree, parser);
    var expression = engine.parseExpression("//ruleSpec", new StaticContextBuilder());
    object[] contexts = new object[] {dynamicContext.Document};
    var rs = expression.evaluate(dynamicContext, contexts);
    OutputResultSet(expression, rs, parser);
    if (rs.size() != 65) throw new Exception();



# CTree

    var c1 = new CTree.Class1(parser, lexer, new Dictionary<string, IParseTree>());
    var c2 = c1.CreateTree("( ruleAltList ( labeledAlt ( alternative )))");


# Replace

    // Replace all remaining ";" with ",". 
    TreeEdits.Replace(tree, (in IParseTree n, out bool c) =>
    {
        c = true;
        if (!nodes.Contains(n) && n != last) return null;
        var t = n as TerminalNodeImpl;
        var new_sym = new TerminalNodeImpl(new CommonToken(ANTLRv4Lexer.COMMA)
            {Line = -1, Column = -1, Text = ","});
        text_before.TryGetValue(t, out string v);
        if (v != null)
            text_before.Add(new_sym, v);
        return new_sym;
    });


# Delete

    // Delete tha last ";" in tokens list--change in syntax.
    var last = nodes.Last();
    TreeEdits.Delete(last, (in IParseTree n, out bool c) =>
    {
        c = false;
        return n;
    });
