using Xunit;
using XQuery.DataModel;
using XQuery.Engine;
using XQuery.Parser;
using XQuery.Parser.Ast;
using XQuery.IO;

namespace XQuery.Tests;

public class XQueryTests
{
    private ExprNode ParseXPath(string input)
    {
        var parser = new XPathParser(input);
        return parser.Parse();
    }

    #region Node Constructor Tests

    [Fact]
    public void ElementConstructor_Simple()
    {
        var expr = new ElementConstructorExpr
        {
            Name = new XdmQName("test"),
            IsComputed = false,
            Content = new List<ExprNode>()
        };

        var evaluator = new XQueryEvaluator();
        var result = evaluator.Evaluate(expr);

        Assert.Single(result);
        var elem = Assert.IsType<XdmElement>(result.First);
        Assert.Equal("test", elem.LocalName);
    }

    [Fact]
    public void ElementConstructor_WithText()
    {
        var textExpr = new TextConstructorExpr
        {
            Content = new StringLiteralExpr { Value = "Hello World" }
        };

        var expr = new ElementConstructorExpr
        {
            Name = new XdmQName("greeting"),
            IsComputed = false,
            Content = new List<ExprNode> { textExpr }
        };

        var evaluator = new XQueryEvaluator();
        var result = evaluator.Evaluate(expr);

        var elem = Assert.IsType<XdmElement>(result.First);
        Assert.Equal("Hello World", elem.StringValue);
    }

    [Fact]
    public void ElementConstructor_WithAttribute()
    {
        var attrExpr = new AttributeConstructorExpr
        {
            Name = new XdmQName("id"),
            Value = new StringLiteralExpr { Value = "123" }
        };

        var expr = new ElementConstructorExpr
        {
            Name = new XdmQName("item"),
            IsComputed = false,
            Content = new List<ExprNode> { attrExpr }
        };

        var evaluator = new XQueryEvaluator();
        var result = evaluator.Evaluate(expr);

        var elem = Assert.IsType<XdmElement>(result.First);
        Assert.Equal("123", elem.GetAttribute("id"));
    }

    [Fact]
    public void TextConstructor_Simple()
    {
        var expr = new TextConstructorExpr
        {
            Content = new StringLiteralExpr { Value = "Hello" }
        };

        var evaluator = new XQueryEvaluator();
        var result = evaluator.Evaluate(expr);

        var text = Assert.IsType<XdmText>(result.First);
        Assert.Equal("Hello", text.Value);
    }

    [Fact]
    public void CommentConstructor_Simple()
    {
        var expr = new CommentConstructorExpr
        {
            Content = new StringLiteralExpr { Value = " This is a comment " }
        };

        var evaluator = new XQueryEvaluator();
        var result = evaluator.Evaluate(expr);

        var comment = Assert.IsType<XdmComment>(result.First);
        Assert.Equal(" This is a comment ", comment.Value);
    }

    [Fact]
    public void PIConstructor_Simple()
    {
        var expr = new PIConstructorExpr
        {
            Target = "xml-stylesheet",
            Content = new StringLiteralExpr { Value = "type=\"text/css\" href=\"style.css\"" }
        };

        var evaluator = new XQueryEvaluator();
        var result = evaluator.Evaluate(expr);

        var pi = Assert.IsType<XdmProcessingInstruction>(result.First);
        Assert.Equal("xml-stylesheet", pi.Target);
        Assert.Contains("text/css", pi.Data);
    }

    [Fact]
    public void DocumentConstructor_Simple()
    {
        var elemExpr = new ElementConstructorExpr
        {
            Name = new XdmQName("root"),
            Content = new List<ExprNode>()
        };

        var expr = new DocumentConstructorExpr
        {
            Content = elemExpr
        };

        var evaluator = new XQueryEvaluator();
        var result = evaluator.Evaluate(expr);

        var doc = Assert.IsType<XdmDocument>(result.First);
        Assert.NotNull(doc.DocumentElement);
        Assert.Equal("root", doc.DocumentElement.LocalName);
    }

    #endregion

    #region XQuery Update Facility Tests

    [Fact]
    public void Insert_IntoElement()
    {
        var doc = XmlDocumentReader.Parse("<root><item id='1'/></root>");
        var context = new EvaluationContext { ContextItem = doc.DocumentElement };

        var newElement = new ElementConstructorExpr
        {
            Name = new XdmQName("newitem"),
            Content = new List<ExprNode>()
        };

        var insertExpr = new InsertExpr
        {
            Source = newElement,
            Target = new ContextItemExpr(),
            Position = InsertPosition.Into
        };

        var evaluator = new XQueryEvaluator(context);
        evaluator.Evaluate(insertExpr);

        Assert.Equal(2, doc.DocumentElement!.Children.Count);
    }

    [Fact]
    public void Insert_AsFirst()
    {
        var doc = XmlDocumentReader.Parse("<root><item id='1'/><item id='2'/></root>");
        var context = new EvaluationContext { ContextItem = doc.DocumentElement };

        var newElement = new ElementConstructorExpr
        {
            Name = new XdmQName("first"),
            Content = new List<ExprNode>()
        };

        var insertExpr = new InsertExpr
        {
            Source = newElement,
            Target = new ContextItemExpr(),
            Position = InsertPosition.AsFirst
        };

        var evaluator = new XQueryEvaluator(context);
        evaluator.Evaluate(insertExpr);

        Assert.Equal(3, doc.DocumentElement!.Children.Count);
        Assert.Equal("first", (doc.DocumentElement.Children[0] as XdmElement)?.LocalName);
    }

    [Fact]
    public void Delete_Element()
    {
        var doc = XmlDocumentReader.Parse("<root><item id='1'/><item id='2'/></root>");
        var context = new EvaluationContext { ContextItem = doc };

        // Delete first item
        var pathExpr = ParseXPath("root/item[@id='1']");

        var deleteExpr = new DeleteExpr
        {
            Target = pathExpr
        };

        var evaluator = new XQueryEvaluator(context);
        evaluator.Evaluate(deleteExpr);

        Assert.Single(doc.DocumentElement!.Children);
        Assert.Equal("2", (doc.DocumentElement.Children[0] as XdmElement)?.GetAttribute("id"));
    }

    [Fact]
    public void Replace_NodeValue()
    {
        var doc = XmlDocumentReader.Parse("<root><item>old value</item></root>");
        var context = new EvaluationContext { ContextItem = doc };

        var pathExpr = ParseXPath("root/item");

        var replaceExpr = new ReplaceExpr
        {
            ValueOf = true,
            Target = pathExpr,
            Replacement = new StringLiteralExpr { Value = "new value" }
        };

        var evaluator = new XQueryEvaluator(context);
        evaluator.Evaluate(replaceExpr);

        Assert.Equal("new value", doc.DocumentElement!.Children[0].StringValue);
    }

    [Fact]
    public void Replace_Node()
    {
        var doc = XmlDocumentReader.Parse("<root><olditem/></root>");
        var context = new EvaluationContext { ContextItem = doc };

        var pathExpr = ParseXPath("root/olditem");

        var newElement = new ElementConstructorExpr
        {
            Name = new XdmQName("newitem"),
            Content = new List<ExprNode>()
        };

        var replaceExpr = new ReplaceExpr
        {
            ValueOf = false,
            Target = pathExpr,
            Replacement = newElement
        };

        var evaluator = new XQueryEvaluator(context);
        evaluator.Evaluate(replaceExpr);

        Assert.Single(doc.DocumentElement!.Children);
        Assert.Equal("newitem", (doc.DocumentElement.Children[0] as XdmElement)?.LocalName);
    }

    [Fact]
    public void Rename_Element()
    {
        var doc = XmlDocumentReader.Parse("<root><oldname/></root>");
        var context = new EvaluationContext { ContextItem = doc };

        var pathExpr = ParseXPath("root/oldname");

        var renameExpr = new RenameExpr
        {
            Target = pathExpr,
            NewName = new StringLiteralExpr { Value = "newname" }
        };

        var evaluator = new XQueryEvaluator(context);
        evaluator.Evaluate(renameExpr);

        Assert.Equal("newname", (doc.DocumentElement!.Children[0] as XdmElement)?.LocalName);
    }

    [Fact]
    public void Transform_CopyModifyReturn()
    {
        var doc = XmlDocumentReader.Parse("<root><item>value</item></root>");
        var context = new EvaluationContext { ContextItem = doc };

        var transformExpr = new TransformExpr
        {
            CopyBindings = new List<CopyBinding>
            {
                new CopyBinding
                {
                    Variable = "copy",
                    Expression = new ContextItemExpr()
                }
            },
            ModifyExpr = new ReplaceExpr
            {
                ValueOf = true,
                Target = new VariableRefExpr { Name = "copy" },
                Replacement = new StringLiteralExpr { Value = "modified" }
            },
            ReturnExpr = new VariableRefExpr { Name = "copy" }
        };

        context.ContextItem = doc.DocumentElement!.Children[0];

        var evaluator = new XQueryEvaluator(context);
        var result = evaluator.Evaluate(transformExpr);

        // Original should be unchanged
        Assert.Equal("value", doc.DocumentElement!.Children[0].StringValue);

        // Result should be modified copy
        Assert.Equal("modified", result.First!.StringValue);
    }

    #endregion

    #region FLWOR with Update Tests

    [Fact]
    public void Flwor_WithInsert()
    {
        var doc = XmlDocumentReader.Parse("<root><container/></root>");
        var context = new EvaluationContext { ContextItem = doc };

        // for $i in (1, 2, 3) return insert <item>{$i}</item> into /root/container
        var containerExpr = ParseXPath("root/container");

        var flworExpr = new FlworExpr
        {
            Clauses = new List<FlworClause>
            {
                new ForClause
                {
                    Variable = "i",
                    Expression = new SequenceExpr
                    {
                        Items = new List<ExprNode>
                        {
                            new IntegerLiteralExpr { Value = 1 },
                            new IntegerLiteralExpr { Value = 2 },
                            new IntegerLiteralExpr { Value = 3 }
                        }
                    }
                }
            },
            Return = new InsertExpr
            {
                Source = new ElementConstructorExpr
                {
                    Name = new XdmQName("item"),
                    Content = new List<ExprNode>
                    {
                        new TextConstructorExpr
                        {
                            Content = new VariableRefExpr { Name = "i" }
                        }
                    }
                },
                Target = containerExpr,
                Position = InsertPosition.Into
            }
        };

        var evaluator = new XQueryEvaluator(context);
        evaluator.Evaluate(flworExpr);

        var container = doc.DocumentElement!.Children.OfType<XdmElement>()
            .FirstOrDefault(e => e.LocalName == "container");

        Assert.NotNull(container);
        Assert.Equal(3, container.Children.Count);
    }

    #endregion

    #region Module Tests

    [Fact]
    public void Module_VariableDeclaration()
    {
        var module = new ModuleNode
        {
            Prolog = new PrologNode
            {
                VariableDecls = new List<VariableDeclNode>
                {
                    new VariableDeclNode
                    {
                        Name = "x",
                        Value = new IntegerLiteralExpr { Value = 42 }
                    }
                }
            },
            Body = new VariableRefExpr { Name = "x" }
        };

        var evaluator = new XQueryEvaluator();
        var result = evaluator.EvaluateModule(module);

        Assert.Equal(42, (result.First as XdmAtomicValue)?.AsInteger());
    }

    [Fact]
    public void Module_FunctionDeclaration()
    {
        var module = new ModuleNode
        {
            Prolog = new PrologNode
            {
                FunctionDecls = new List<FunctionDeclNode>
                {
                    new FunctionDeclNode
                    {
                        Name = "double",
                        Parameters = new List<ParameterNode>
                        {
                            new ParameterNode { Name = "n" }
                        },
                        Body = new BinaryExpr
                        {
                            Left = new VariableRefExpr { Name = "n" },
                            Operator = BinaryOperator.Multiply,
                            Right = new IntegerLiteralExpr { Value = 2 }
                        }
                    }
                }
            },
            Body = new FunctionCallExpr
            {
                Name = "double",
                Arguments = new List<ExprNode> { new IntegerLiteralExpr { Value = 21 } }
            }
        };

        var evaluator = new XQueryEvaluator();
        var result = evaluator.EvaluateModule(module);

        Assert.Equal(42, (result.First as XdmAtomicValue)?.AsInteger());
    }

    [Fact]
    public void Module_NamespaceDeclaration()
    {
        var module = new ModuleNode
        {
            Prolog = new PrologNode
            {
                NamespaceDecls = new List<NamespaceDeclNode>
                {
                    new NamespaceDeclNode
                    {
                        Prefix = "ex",
                        Uri = "http://example.org"
                    }
                }
            },
            Body = new ElementConstructorExpr
            {
                Name = new XdmQName("http://example.org", "test", "ex"),
                Content = new List<ExprNode>()
            }
        };

        var evaluator = new XQueryEvaluator();
        var result = evaluator.EvaluateModule(module);

        var elem = Assert.IsType<XdmElement>(result.First);
        Assert.Equal("http://example.org", elem.NamespaceUri);
        Assert.Equal("ex", elem.Prefix);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void Integration_JsonToXml_Transform()
    {
        // Parse JSON
        var jsonItem = JsonDocumentReader.Parse("{\"name\": \"John\", \"age\": 30}");

        // Create XQuery context with JSON map
        var context = new EvaluationContext();
        context.SetVariable("data", new XdmSequence(jsonItem));

        // Build XML from JSON
        var elemExpr = new ElementConstructorExpr
        {
            Name = new XdmQName("person"),
            Content = new List<ExprNode>
            {
                new ElementConstructorExpr
                {
                    Name = new XdmQName("name"),
                    Content = new List<ExprNode>
                    {
                        new TextConstructorExpr
                        {
                            Content = new PostfixLookupExpr
                            {
                                Base = new VariableRefExpr { Name = "data" },
                                KeyExpr = new StringLiteralExpr { Value = "name" }
                            }
                        }
                    }
                },
                new ElementConstructorExpr
                {
                    Name = new XdmQName("age"),
                    Content = new List<ExprNode>
                    {
                        new TextConstructorExpr
                        {
                            Content = new PostfixLookupExpr
                            {
                                Base = new VariableRefExpr { Name = "data" },
                                KeyExpr = new StringLiteralExpr { Value = "age" }
                            }
                        }
                    }
                }
            }
        };

        var evaluator = new XQueryEvaluator(context);
        var result = evaluator.Evaluate(elemExpr);

        var person = Assert.IsType<XdmElement>(result.First);
        Assert.Equal("person", person.LocalName);

        var nameElem = person.GetElementsByName("name").FirstOrDefault();
        Assert.NotNull(nameElem);
        Assert.Equal("John", nameElem.StringValue);

        var ageElem = person.GetElementsByName("age").FirstOrDefault();
        Assert.NotNull(ageElem);
        Assert.Equal("30", ageElem.StringValue);
    }

    [Fact]
    public void Integration_XmlUpdate_WithOutput()
    {
        var doc = XmlDocumentReader.Parse("<catalog><book><title>Old Title</title></book></catalog>");
        var context = new EvaluationContext { ContextItem = doc };

        // Update the title
        var pathExpr = ParseXPath("catalog/book/title");

        var replaceExpr = new ReplaceExpr
        {
            ValueOf = true,
            Target = pathExpr,
            Replacement = new StringLiteralExpr { Value = "New Title" }
        };

        var evaluator = new XQueryEvaluator(context);
        evaluator.Evaluate(replaceExpr);

        // Serialize back to XML
        var xml = XmlSerializer.Serialize(doc, new SerializationOptions { OmitXmlDeclaration = true });
        Assert.Contains("New Title", xml);
    }

    #endregion
}
