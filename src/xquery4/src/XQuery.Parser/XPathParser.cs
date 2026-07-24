using XQuery.DataModel;
using XQuery.Parser.Ast;

namespace XQuery.Parser;

/// <summary>
/// Parser for XPath 4.0 expressions.
/// Uses recursive descent parsing.
/// </summary>
public class XPathParser
{
    protected readonly List<Token> _tokens;
    protected int _position;
    protected readonly Dictionary<string, string> _namespaces = new();

    public XPathParser(string input)
    {
        var lexer = new Lexer(input);
        _tokens = lexer.Tokenize();
        _position = 0;

        // Default namespaces
        _namespaces["xs"] = XdmQName.XsNamespace;
        _namespaces["fn"] = XdmQName.FnNamespace;
        _namespaces["map"] = XdmQName.MapNamespace;
        _namespaces["array"] = XdmQName.ArrayNamespace;
        _namespaces["math"] = XdmQName.MathNamespace;
        _namespaces["xml"] = XdmQName.XmlNamespace;
    }

    public void AddNamespace(string prefix, string uri)
    {
        _namespaces[prefix] = uri;
    }

    public ExprNode Parse()
    {
        var expr = ParseExpr();
        Expect(TokenType.Eof);
        return expr;
    }

    #region Expression Parsing

    // Expr ::= ExprSingle ("," ExprSingle)*
    protected virtual ExprNode ParseExpr()
    {
        var first = ParseExprSingle();

        if (!Check(TokenType.Comma))
            return first;

        var items = new List<ExprNode> { first };

        while (Match(TokenType.Comma))
        {
            items.Add(ParseExprSingle());
        }

        return new SequenceExpr { Items = items, Line = first.Line, Column = first.Column };
    }

    // ExprSingle ::= ForExpr | LetExpr | QuantifiedExpr | IfExpr | SwitchExpr | TryCatchExpr | OrExpr
    protected virtual ExprNode ParseExprSingle()
    {
        if (Check(TokenType.For))
            return ParseFlworExpr();
        if (Check(TokenType.Let))
            return ParseFlworExpr();
        if (Check(TokenType.Some) || Check(TokenType.Every))
            return ParseQuantifiedExpr();
        if (Check(TokenType.If))
            return ParseIfExpr();
        if (Check(TokenType.Switch))
            return ParseSwitchExpr();
        if (Check(TokenType.Try))
            return ParseTryCatchExpr();

        return ParseOrExpr();
    }

    // OrExpr ::= AndExpr ("or" AndExpr)*
    protected ExprNode ParseOrExpr()
    {
        var left = ParseAndExpr();

        while (Match(TokenType.Or))
        {
            var right = ParseAndExpr();
            left = new BinaryExpr
            {
                Left = left,
                Operator = BinaryOperator.Or,
                Right = right,
                Line = left.Line,
                Column = left.Column
            };
        }

        return left;
    }

    // AndExpr ::= ComparisonExpr ("and" ComparisonExpr)*
    protected ExprNode ParseAndExpr()
    {
        var left = ParseComparisonExpr();

        while (Match(TokenType.And))
        {
            var right = ParseComparisonExpr();
            left = new BinaryExpr
            {
                Left = left,
                Operator = BinaryOperator.And,
                Right = right,
                Line = left.Line,
                Column = left.Column
            };
        }

        return left;
    }

    // ComparisonExpr ::= OtherwiseExpr ((ValueComp | GeneralComp | NodeComp) OtherwiseExpr)?
    protected ExprNode ParseComparisonExpr()
    {
        var left = ParseOtherwiseExpr();

        var op = TryParseComparisonOperator();
        if (op.HasValue)
        {
            var right = ParseOtherwiseExpr();
            return new ComparisonExpr
            {
                Left = left,
                Operator = op.Value,
                Right = right,
                Line = left.Line,
                Column = left.Column
            };
        }

        return left;
    }

    private ComparisonOperator? TryParseComparisonOperator()
    {
        // Value comparison
        if (Match(TokenType.Eq)) return ComparisonOperator.Eq;
        if (Match(TokenType.Ne)) return ComparisonOperator.Ne;
        if (Match(TokenType.Lt)) return ComparisonOperator.Lt;
        if (Match(TokenType.Le)) return ComparisonOperator.Le;
        if (Match(TokenType.Gt)) return ComparisonOperator.Gt;
        if (Match(TokenType.Ge)) return ComparisonOperator.Ge;

        // General comparison
        if (Match(TokenType.Equal)) return ComparisonOperator.Equal;
        if (Match(TokenType.NotEqual)) return ComparisonOperator.NotEqual;
        if (Match(TokenType.LessThan)) return ComparisonOperator.LessThan;
        if (Match(TokenType.LessOrEqual)) return ComparisonOperator.LessOrEqual;
        if (Match(TokenType.GreaterThan)) return ComparisonOperator.GreaterThan;
        if (Match(TokenType.GreaterOrEqual)) return ComparisonOperator.GreaterOrEqual;

        // Node comparison
        if (Match(TokenType.Is)) return ComparisonOperator.Is;
        if (Match(TokenType.Precedes)) return ComparisonOperator.Precedes;
        if (Match(TokenType.Follows)) return ComparisonOperator.Follows;

        return null;
    }

    // OtherwiseExpr ::= StringConcatExpr ("otherwise" StringConcatExpr)*
    protected ExprNode ParseOtherwiseExpr()
    {
        var left = ParseStringConcatExpr();

        while (Match(TokenType.Otherwise))
        {
            var right = ParseStringConcatExpr();
            left = new OtherwiseExpr
            {
                Left = left,
                Right = right,
                Line = left.Line,
                Column = left.Column
            };
        }

        return left;
    }

    // StringConcatExpr ::= RangeExpr ("||" RangeExpr)*
    protected ExprNode ParseStringConcatExpr()
    {
        var left = ParseRangeExpr();

        while (Match(TokenType.Concat))
        {
            var right = ParseRangeExpr();
            left = new ConcatExpr
            {
                Left = left,
                Right = right,
                Line = left.Line,
                Column = left.Column
            };
        }

        return left;
    }

    // RangeExpr ::= AdditiveExpr ("to" AdditiveExpr)?
    protected ExprNode ParseRangeExpr()
    {
        var left = ParseAdditiveExpr();

        if (Match(TokenType.To))
        {
            var right = ParseAdditiveExpr();
            return new RangeExpr
            {
                Start = left,
                End = right,
                Line = left.Line,
                Column = left.Column
            };
        }

        return left;
    }

    // AdditiveExpr ::= MultiplicativeExpr (("+" | "-") MultiplicativeExpr)*
    protected ExprNode ParseAdditiveExpr()
    {
        var left = ParseMultiplicativeExpr();

        while (Check(TokenType.Plus) || Check(TokenType.Minus))
        {
            var op = Match(TokenType.Plus) ? BinaryOperator.Add : BinaryOperator.Subtract;
            if (op == BinaryOperator.Subtract) Advance();
            var right = ParseMultiplicativeExpr();
            left = new BinaryExpr
            {
                Left = left,
                Operator = op,
                Right = right,
                Line = left.Line,
                Column = left.Column
            };
        }

        return left;
    }

    // MultiplicativeExpr ::= UnionExpr (("*" | "div" | "idiv" | "mod") UnionExpr)*
    protected ExprNode ParseMultiplicativeExpr()
    {
        var left = ParseUnionExpr();

        while (true)
        {
            BinaryOperator op;
            if (Match(TokenType.Asterisk)) op = BinaryOperator.Multiply;
            else if (Match(TokenType.Div)) op = BinaryOperator.Divide;
            else if (Match(TokenType.IDiv)) op = BinaryOperator.IntegerDivide;
            else if (Match(TokenType.Mod)) op = BinaryOperator.Modulo;
            else break;

            var right = ParseUnionExpr();
            left = new BinaryExpr
            {
                Left = left,
                Operator = op,
                Right = right,
                Line = left.Line,
                Column = left.Column
            };
        }

        return left;
    }

    // UnionExpr ::= IntersectExceptExpr (("union" | "|") IntersectExceptExpr)*
    protected ExprNode ParseUnionExpr()
    {
        var first = ParseIntersectExceptExpr();

        if (!Check(TokenType.Union) && !Check(TokenType.Pipe))
            return first;

        var operands = new List<ExprNode> { first };

        while (Match(TokenType.Union) || Match(TokenType.Pipe))
        {
            operands.Add(ParseIntersectExceptExpr());
        }

        return new UnionExpr { Operands = operands, Line = first.Line, Column = first.Column };
    }

    // IntersectExceptExpr ::= InstanceOfExpr (("intersect" | "except") InstanceOfExpr)*
    protected ExprNode ParseIntersectExceptExpr()
    {
        var left = ParseInstanceOfExpr();

        while (Check(TokenType.Intersect) || Check(TokenType.Except))
        {
            bool isIntersect = Match(TokenType.Intersect);
            if (!isIntersect) Advance();

            var right = ParseInstanceOfExpr();
            left = new IntersectExceptExpr
            {
                Left = left,
                IsIntersect = isIntersect,
                Right = right,
                Line = left.Line,
                Column = left.Column
            };
        }

        return left;
    }

    // InstanceOfExpr ::= TreatExpr ("instance" "of" SequenceType)?
    protected ExprNode ParseInstanceOfExpr()
    {
        var expr = ParseTreatExpr();

        if (Match(TokenType.Instance))
        {
            Expect(TokenType.Of);
            var type = ParseSequenceType();
            return new InstanceOfExpr
            {
                Expression = expr,
                Type = type,
                Line = expr.Line,
                Column = expr.Column
            };
        }

        return expr;
    }

    // TreatExpr ::= CastableExpr ("treat" "as" SequenceType)?
    protected ExprNode ParseTreatExpr()
    {
        var expr = ParseCastableExpr();

        if (Match(TokenType.Treat))
        {
            Expect(TokenType.As);
            var type = ParseSequenceType();
            return new TreatExpr
            {
                Expression = expr,
                Type = type,
                Line = expr.Line,
                Column = expr.Column
            };
        }

        return expr;
    }

    // CastableExpr ::= CastExpr ("castable" "as" SingleType)?
    protected ExprNode ParseCastableExpr()
    {
        var expr = ParseCastExpr();

        if (Match(TokenType.Castable))
        {
            Expect(TokenType.As);
            var (typeName, allowEmpty) = ParseSingleType();
            return new CastableExpr
            {
                Expression = expr,
                TargetType = typeName,
                AllowEmpty = allowEmpty,
                Line = expr.Line,
                Column = expr.Column
            };
        }

        return expr;
    }

    // CastExpr ::= ArrowExpr ("cast" "as" SingleType)?
    protected ExprNode ParseCastExpr()
    {
        var expr = ParseArrowExpr();

        if (Match(TokenType.Cast))
        {
            Expect(TokenType.As);
            var (typeName, allowEmpty) = ParseSingleType();
            return new CastExpr
            {
                Expression = expr,
                TargetType = typeName,
                AllowEmpty = allowEmpty,
                Line = expr.Line,
                Column = expr.Column
            };
        }

        return expr;
    }

    // ArrowExpr ::= UnaryExpr (("=>" | "->") ArrowFunctionSpecifier ArgumentList)*
    protected ExprNode ParseArrowExpr()
    {
        var expr = ParseUnaryExpr();

        while (Check(TokenType.Arrow) || Check(TokenType.ThinArrow))
        {
            bool isThinArrow = Check(TokenType.ThinArrow);
            Advance();

            var funcSpec = ParsePrimaryExpr();
            var args = new List<ExprNode>();

            if (Check(TokenType.LeftParen))
            {
                args = ParseArgumentList();
            }

            expr = new ArrowExpr
            {
                Argument = expr,
                Function = funcSpec,
                AdditionalArguments = args,
                IsThinArrow = isThinArrow,
                Line = expr.Line,
                Column = expr.Column
            };
        }

        return expr;
    }

    // UnaryExpr ::= ("-" | "+")* SimpleMapExpr
    protected ExprNode ParseUnaryExpr()
    {
        var ops = new List<UnaryOperator>();

        while (Check(TokenType.Minus) || Check(TokenType.Plus))
        {
            ops.Add(Check(TokenType.Minus) ? UnaryOperator.Minus : UnaryOperator.Plus);
            Advance();
        }

        var expr = ParseSimpleMapExpr();

        // Apply unary operators in reverse order
        for (int i = ops.Count - 1; i >= 0; i--)
        {
            expr = new UnaryExpr
            {
                Operator = ops[i],
                Operand = expr,
                Line = expr.Line,
                Column = expr.Column
            };
        }

        return expr;
    }

    // SimpleMapExpr ::= PathExpr ("!" PathExpr)*
    protected ExprNode ParseSimpleMapExpr()
    {
        var first = ParsePathExpr();

        if (!Check(TokenType.Exclamation))
            return first;

        var steps = new List<ExprNode> { first };

        while (Match(TokenType.Exclamation))
        {
            steps.Add(ParsePathExpr());
        }

        return new SimpleMapExpr { Steps = steps, Line = first.Line, Column = first.Column };
    }

    // PathExpr ::= ("/" RelativePathExpr?) | ("//" RelativePathExpr) | RelativePathExpr
    protected ExprNode ParsePathExpr()
    {
        var token = Current();

        if (Match(TokenType.Slash))
        {
            if (IsRelativePathStart())
            {
                var steps = ParseRelativePathExpr();
                return new PathExpr
                {
                    IsAbsolute = true,
                    Steps = steps,
                    Line = token.Line,
                    Column = token.Column
                };
            }
            return new PathExpr
            {
                IsAbsolute = true,
                IsRootOnly = true,
                Line = token.Line,
                Column = token.Column
            };
        }

        if (Match(TokenType.SlashSlash))
        {
            var steps = ParseRelativePathExpr();
            steps.Insert(0, CreateDescendantOrSelfStep(token));
            return new PathExpr
            {
                IsAbsolute = true,
                Steps = steps,
                Line = token.Line,
                Column = token.Column
            };
        }

        return ParseRelativePathExprOrPrimary();
    }

    private bool IsRelativePathStart()
    {
        return Check(TokenType.NCName) || Check(TokenType.QName) ||
               Check(TokenType.Asterisk) || Check(TokenType.At) ||
               Check(TokenType.Dot) || Check(TokenType.DotDot) ||
               Check(TokenType.LeftParen) || Check(TokenType.Dollar) ||
               IsNodeKindTest() || IsAxisName();
    }

    private ExprNode ParseRelativePathExprOrPrimary()
    {
        var first = ParseStepExpr();

        if (!Check(TokenType.Slash) && !Check(TokenType.SlashSlash))
            return first;

        var steps = new List<ExprNode> { first };

        while (true)
        {
            if (Match(TokenType.SlashSlash))
            {
                steps.Add(CreateDescendantOrSelfStep(Current()));
                steps.Add(ParseStepExpr());
            }
            else if (Match(TokenType.Slash))
            {
                steps.Add(ParseStepExpr());
            }
            else
            {
                break;
            }
        }

        return new PathExpr
        {
            IsAbsolute = false,
            Steps = steps,
            Line = first.Line,
            Column = first.Column
        };
    }

    private List<ExprNode> ParseRelativePathExpr()
    {
        var steps = new List<ExprNode>();
        steps.Add(ParseStepExpr());

        while (true)
        {
            if (Match(TokenType.SlashSlash))
            {
                steps.Add(CreateDescendantOrSelfStep(Current()));
                steps.Add(ParseStepExpr());
            }
            else if (Match(TokenType.Slash))
            {
                steps.Add(ParseStepExpr());
            }
            else
            {
                break;
            }
        }

        return steps;
    }

    private AxisStepExpr CreateDescendantOrSelfStep(Token token)
    {
        return new AxisStepExpr
        {
            Axis = Axis.DescendantOrSelf,
            NodeTest = new KindTestExpr { Kind = XdmNodeKind.Element },
            Line = token.Line,
            Column = token.Column
        };
    }

    // StepExpr ::= PostfixExpr | AxisStep
    protected ExprNode ParseStepExpr()
    {
        if (IsAxisStep())
            return ParseAxisStep();

        return ParsePostfixExpr();
    }

    private bool IsAxisStep()
    {
        // Abbreviated steps
        if (Check(TokenType.DotDot)) return true;
        if (Check(TokenType.At)) return true;

        // Axis names followed by ::
        if (IsAxisName() && Peek(1).Type == TokenType.ColonColon)
            return true;

        // Node tests (might be function calls, need lookahead)
        if (IsNodeKindTest())
            return true;

        // Name test (could be element test or function call)
        if (IsNameToken() || Check(TokenType.Asterisk))
        {
            // If followed by ::, it's an axis
            if (Peek(1).Type == TokenType.ColonColon)
                return true;

            // If followed by (, it's a function call (but not if it's a node kind test)
            if (Peek(1).Type == TokenType.LeftParen && !IsNodeKindTest())
                return false;

            // If map/array followed by {, it's a constructor, not a name test
            if ((Check(TokenType.Map) || Check(TokenType.Array)) && Peek(1).Type == TokenType.LeftBrace)
                return false;

            // Otherwise it's a name test
            return true;
        }

        return false;
    }

    private bool IsAxisName()
    {
        return Current().Type is TokenType.Ancestor or TokenType.AncestorOrSelf or
            TokenType.Child or TokenType.Descendant or TokenType.DescendantOrSelf or
            TokenType.Following or TokenType.FollowingSibling or TokenType.Parent or
            TokenType.Preceding or TokenType.PrecedingSibling or TokenType.Self or
            TokenType.Attribute or TokenType.Namespace;
    }

    private bool IsNodeKindTest()
    {
        return Current().Type is TokenType.Node or TokenType.Text or
            TokenType.Comment or TokenType.ProcessingInstruction or
            TokenType.Document or TokenType.Element or TokenType.Attribute or
            TokenType.SchemaElement or TokenType.SchemaAttribute or
            TokenType.NamespaceNode;
    }

    // AxisStep ::= (ReverseStep | ForwardStep) PredicateList
    protected ExprNode ParseAxisStep()
    {
        var token = Current();
        Axis axis;
        ExprNode nodeTest;

        // Abbreviated steps
        if (Match(TokenType.DotDot))
        {
            return new AxisStepExpr
            {
                Axis = Axis.Parent,
                NodeTest = new KindTestExpr { Kind = XdmNodeKind.Element },
                Predicates = ParsePredicateList(),
                Line = token.Line,
                Column = token.Column
            };
        }

        if (Match(TokenType.At))
        {
            axis = Axis.Attribute;
            nodeTest = ParseNodeTest();
        }
        else if (IsAxisName() && Peek(1).Type == TokenType.ColonColon)
        {
            axis = ParseAxisSpecifier();
            Expect(TokenType.ColonColon);
            nodeTest = ParseNodeTest();
        }
        else
        {
            axis = Axis.Child;
            nodeTest = ParseNodeTest();
        }

        var predicates = ParsePredicateList();

        return new AxisStepExpr
        {
            Axis = axis,
            NodeTest = nodeTest,
            Predicates = predicates,
            Line = token.Line,
            Column = token.Column
        };
    }

    private Axis ParseAxisSpecifier()
    {
        var token = Current();
        Advance();

        return token.Type switch
        {
            TokenType.Ancestor => Axis.Ancestor,
            TokenType.AncestorOrSelf => Axis.AncestorOrSelf,
            TokenType.Child => Axis.Child,
            TokenType.Descendant => Axis.Descendant,
            TokenType.DescendantOrSelf => Axis.DescendantOrSelf,
            TokenType.Following => Axis.Following,
            TokenType.FollowingSibling => Axis.FollowingSibling,
            TokenType.Parent => Axis.Parent,
            TokenType.Preceding => Axis.Preceding,
            TokenType.PrecedingSibling => Axis.PrecedingSibling,
            TokenType.Self => Axis.Self,
            TokenType.Attribute => Axis.Attribute,
            TokenType.Namespace => Axis.Namespace,
            _ => throw ParseError($"Expected axis name, got {token.Type}")
        };
    }

    // NodeTest ::= KindTest | NameTest
    protected ExprNode ParseNodeTest()
    {
        if (IsNodeKindTest() && Peek(1).Type == TokenType.LeftParen)
            return ParseKindTest();

        return ParseNameTest();
    }

    protected ExprNode ParseNameTest()
    {
        var token = Current();

        // Wildcard
        if (Match(TokenType.Asterisk))
        {
            return new NameTestExpr
            {
                IsWildcard = true,
                Line = token.Line,
                Column = token.Column
            };
        }

        // NCName:* or prefix:localname or keyword used as name
        if (IsNameToken())
        {
            var name = Current().Value;
            Advance();

            // Check for :*
            if (Check(TokenType.Colon) && Peek(1).Type == TokenType.Asterisk)
            {
                Advance(); // :
                Advance(); // *
                return new NameTestExpr
                {
                    Prefix = name,
                    IsLocalWildcard = true,
                    NamespaceUri = _namespaces.GetValueOrDefault(name),
                    Line = token.Line,
                    Column = token.Column
                };
            }

            // QName
            if (name.Contains(':'))
            {
                var parts = name.Split(':', 2);
                return new NameTestExpr
                {
                    Prefix = parts[0],
                    LocalName = parts[1],
                    NamespaceUri = _namespaces.GetValueOrDefault(parts[0]),
                    Line = token.Line,
                    Column = token.Column
                };
            }

            return new NameTestExpr
            {
                LocalName = name,
                Line = token.Line,
                Column = token.Column
            };
        }

        throw ParseError($"Expected name test, got {Current().Type}");
    }

    protected ExprNode ParseKindTest()
    {
        var token = Current();
        XdmNodeKind kind;

        switch (token.Type)
        {
            case TokenType.Node:
                kind = XdmNodeKind.Element; // node() matches all nodes
                break;
            case TokenType.Text:
                kind = XdmNodeKind.Text;
                break;
            case TokenType.Comment:
                kind = XdmNodeKind.Comment;
                break;
            case TokenType.ProcessingInstruction:
                kind = XdmNodeKind.ProcessingInstruction;
                break;
            case TokenType.Document:
                kind = XdmNodeKind.Document;
                break;
            case TokenType.Element:
                kind = XdmNodeKind.Element;
                break;
            case TokenType.Attribute:
                kind = XdmNodeKind.Attribute;
                break;
            case TokenType.NamespaceNode:
                kind = XdmNodeKind.Namespace;
                break;
            default:
                throw ParseError($"Expected kind test, got {token.Type}");
        }

        Advance();
        Expect(TokenType.LeftParen);

        XdmQName? name = null;
        XdmQName? typeName = null;

        // Optional element/attribute name
        if (!Check(TokenType.RightParen))
        {
            if (Check(TokenType.NCName) || Check(TokenType.QName))
            {
                name = ParseQName();

                // Optional type name
                if (Match(TokenType.Comma))
                {
                    typeName = ParseQName();
                }
            }
        }

        Expect(TokenType.RightParen);

        var kindTest = new KindTestExpr
        {
            Kind = kind,
            Name = name,
            TypeName = typeName,
            Line = token.Line,
            Column = token.Column
        };

        // Special case for node() which matches all node kinds
        if (token.Type == TokenType.Node)
        {
            // We'll handle this in the evaluator
        }

        return kindTest;
    }

    // PostfixExpr ::= PrimaryExpr (Predicate | ArgumentList | Lookup)*
    protected ExprNode ParsePostfixExpr()
    {
        var expr = ParsePrimaryExpr();
        var predicates = new List<ExprNode>();

        while (true)
        {
            if (Check(TokenType.LeftBracket))
            {
                predicates.AddRange(ParsePredicateList());
            }
            else if (Check(TokenType.LeftParen) && expr is not FunctionCallExpr)
            {
                // Dynamic function call
                var args = ParseArgumentList();
                expr = new FunctionCallExpr
                {
                    Name = "__dynamic__",
                    Arguments = new List<ExprNode> { expr }.Concat(args).ToList(),
                    Line = expr.Line,
                    Column = expr.Column
                };
            }
            else if (Check(TokenType.QuestionMark))
            {
                // Lookup
                expr = ParsePostfixLookup(expr);
            }
            else
            {
                break;
            }
        }

        if (predicates.Count > 0)
        {
            return new FilterExpr
            {
                Primary = expr,
                Predicates = predicates,
                Line = expr.Line,
                Column = expr.Column
            };
        }

        return expr;
    }

    private ExprNode ParsePostfixLookup(ExprNode baseExpr)
    {
        Expect(TokenType.QuestionMark);

        if (Match(TokenType.Asterisk))
        {
            return new PostfixLookupExpr
            {
                Base = baseExpr,
                IsWildcard = true,
                Line = baseExpr.Line,
                Column = baseExpr.Column
            };
        }

        ExprNode? keyExpr = null;

        if (Check(TokenType.IntegerLiteral))
        {
            keyExpr = new IntegerLiteralExpr { Value = long.Parse(Current().Value) };
            Advance();
        }
        else if (Check(TokenType.NCName))
        {
            keyExpr = new StringLiteralExpr { Value = Current().Value };
            Advance();
        }
        else if (Check(TokenType.LeftParen))
        {
            Advance();
            keyExpr = ParseExpr();
            Expect(TokenType.RightParen);
        }

        return new PostfixLookupExpr
        {
            Base = baseExpr,
            KeyExpr = keyExpr,
            Line = baseExpr.Line,
            Column = baseExpr.Column
        };
    }

    private List<ExprNode> ParsePredicateList()
    {
        var predicates = new List<ExprNode>();

        while (Match(TokenType.LeftBracket))
        {
            predicates.Add(ParseExpr());
            Expect(TokenType.RightBracket);
        }

        return predicates;
    }

    // PrimaryExpr ::= Literal | VarRef | ParenthesizedExpr | ContextItemExpr |
    //                 FunctionCall | FunctionItemExpr | MapConstructor | ArrayConstructor |
    //                 UnaryLookup
    protected ExprNode ParsePrimaryExpr()
    {
        var token = Current();

        // Literals
        if (Check(TokenType.IntegerLiteral))
        {
            var value = long.Parse(Current().Value);
            Advance();
            return new IntegerLiteralExpr { Value = value, Line = token.Line, Column = token.Column };
        }

        if (Check(TokenType.DecimalLiteral))
        {
            var value = decimal.Parse(Current().Value, System.Globalization.CultureInfo.InvariantCulture);
            Advance();
            return new DecimalLiteralExpr { Value = value, Line = token.Line, Column = token.Column };
        }

        if (Check(TokenType.DoubleLiteral))
        {
            var value = double.Parse(Current().Value, System.Globalization.CultureInfo.InvariantCulture);
            Advance();
            return new DoubleLiteralExpr { Value = value, Line = token.Line, Column = token.Column };
        }

        if (Check(TokenType.StringLiteral))
        {
            var value = Current().Value;
            Advance();
            return new StringLiteralExpr { Value = value, Line = token.Line, Column = token.Column };
        }

        // Variable reference
        if (Match(TokenType.Dollar))
        {
            var name = ParseVarName();
            return new VariableRefExpr
            {
                Name = name.LocalName,
                Prefix = name.Prefix,
                NamespaceUri = name.NamespaceUri,
                Line = token.Line,
                Column = token.Column
            };
        }

        // Context item
        if (Match(TokenType.Dot))
        {
            return new ContextItemExpr { Line = token.Line, Column = token.Column };
        }

        // Parenthesized expression or empty sequence
        if (Match(TokenType.LeftParen))
        {
            if (Match(TokenType.RightParen))
            {
                return new SequenceExpr { Items = new List<ExprNode>(), Line = token.Line, Column = token.Column };
            }

            var inner = ParseExpr();
            Expect(TokenType.RightParen);
            return new ParenthesizedExpr { Inner = inner, Line = token.Line, Column = token.Column };
        }

        // Map constructor
        if (Match(TokenType.Map))
        {
            return ParseMapConstructor(token);
        }

        // Array constructor
        if (Match(TokenType.Array))
        {
            return ParseArrayConstructor(token, true);
        }

        if (Match(TokenType.LeftBracket))
        {
            return ParseArrayConstructor(token, false);
        }

        // Inline function
        if (Check(TokenType.Function) && Peek(1).Type == TokenType.LeftParen)
        {
            return ParseInlineFunction();
        }

        // Function call or named function reference
        // Keywords can also be function names (e.g., count, string, concat)
        if (IsNameToken() || IsNodeKindTest())
        {
            return ParseFunctionCallOrRef();
        }

        // Unary lookup
        if (Match(TokenType.QuestionMark))
        {
            return ParseUnaryLookup(token);
        }

        throw ParseError($"Unexpected token in primary expression: {token.Type}");
    }

    private ExprNode ParseMapConstructor(Token token)
    {
        Expect(TokenType.LeftBrace);

        var entries = new List<MapEntry>();

        if (!Check(TokenType.RightBrace))
        {
            do
            {
                var key = ParseExprSingle();
                Expect(TokenType.Colon);
                var value = ParseExprSingle();
                entries.Add(new MapEntry { Key = key, Value = value });
            } while (Match(TokenType.Comma));
        }

        Expect(TokenType.RightBrace);

        return new MapConstructorExpr { Entries = entries, Line = token.Line, Column = token.Column };
    }

    private ExprNode ParseArrayConstructor(Token token, bool isCurly)
    {
        if (isCurly)
        {
            Expect(TokenType.LeftBrace);

            ExprNode? content = null;
            if (!Check(TokenType.RightBrace))
            {
                content = ParseExpr();
            }

            Expect(TokenType.RightBrace);

            return new ArrayConstructorExpr
            {
                Members = content != null ? new List<ExprNode> { content } : new List<ExprNode>(),
                IsCurly = true,
                Line = token.Line,
                Column = token.Column
            };
        }
        else
        {
            // Square bracket array
            var members = new List<ExprNode>();

            if (!Check(TokenType.RightBracket))
            {
                do
                {
                    members.Add(ParseExprSingle());
                } while (Match(TokenType.Comma));
            }

            Expect(TokenType.RightBracket);

            return new ArrayConstructorExpr
            {
                Members = members,
                IsCurly = false,
                Line = token.Line,
                Column = token.Column
            };
        }
    }

    private ExprNode ParseInlineFunction()
    {
        var token = Current();
        Advance(); // function
        Expect(TokenType.LeftParen);

        var parameters = new List<ParameterNode>();

        if (!Check(TokenType.RightParen))
        {
            do
            {
                Expect(TokenType.Dollar);
                var name = ParseNCName();
                SequenceTypeNode? type = null;

                if (Match(TokenType.As))
                {
                    type = ParseSequenceType();
                }

                parameters.Add(new ParameterNode { Name = name, Type = type });
            } while (Match(TokenType.Comma));
        }

        Expect(TokenType.RightParen);

        SequenceTypeNode? returnType = null;
        if (Match(TokenType.As))
        {
            returnType = ParseSequenceType();
        }

        Expect(TokenType.LeftBrace);
        var body = ParseExpr();
        Expect(TokenType.RightBrace);

        return new InlineFunctionExpr
        {
            Parameters = parameters,
            ReturnType = returnType,
            Body = body,
            Line = token.Line,
            Column = token.Column
        };
    }

    private ExprNode ParseFunctionCallOrRef()
    {
        var token = Current();
        var qname = ParseQName();

        // Named function reference: name#arity
        if (Match(TokenType.Hash))
        {
            var arityToken = Current();
            if (!Check(TokenType.IntegerLiteral))
                throw ParseError("Expected arity after #");

            var arity = int.Parse(arityToken.Value);
            Advance();

            return new NamedFunctionRefExpr
            {
                Name = qname.LocalName,
                Prefix = qname.Prefix,
                NamespaceUri = qname.NamespaceUri,
                Arity = arity,
                Line = token.Line,
                Column = token.Column
            };
        }

        // Function call
        var args = ParseArgumentList();

        return new FunctionCallExpr
        {
            Name = qname.LocalName,
            Prefix = qname.Prefix,
            NamespaceUri = qname.NamespaceUri,
            Arguments = args,
            Line = token.Line,
            Column = token.Column
        };
    }

    private List<ExprNode> ParseArgumentList()
    {
        Expect(TokenType.LeftParen);

        var args = new List<ExprNode>();

        if (!Check(TokenType.RightParen))
        {
            do
            {
                if (Check(TokenType.QuestionMark))
                {
                    // Placeholder for partial application
                    Advance();
                    args.Add(new ContextItemExpr()); // Placeholder marker
                }
                else
                {
                    args.Add(ParseExprSingle());
                }
            } while (Match(TokenType.Comma));
        }

        Expect(TokenType.RightParen);
        return args;
    }

    private ExprNode ParseUnaryLookup(Token token)
    {
        if (Match(TokenType.Asterisk))
        {
            return new LookupExpr { IsWildcard = true, Line = token.Line, Column = token.Column };
        }

        ExprNode? keyExpr = null;

        if (Check(TokenType.IntegerLiteral))
        {
            keyExpr = new IntegerLiteralExpr { Value = long.Parse(Current().Value) };
            Advance();
        }
        else if (Check(TokenType.NCName))
        {
            keyExpr = new StringLiteralExpr { Value = Current().Value };
            Advance();
        }
        else if (Check(TokenType.LeftParen))
        {
            Advance();
            keyExpr = ParseExpr();
            Expect(TokenType.RightParen);
        }

        return new LookupExpr { KeyExpr = keyExpr, Line = token.Line, Column = token.Column };
    }

    #endregion

    #region Control Flow Expressions

    protected ExprNode ParseIfExpr()
    {
        var token = Current();
        Expect(TokenType.If);
        Expect(TokenType.LeftParen);
        var condition = ParseExpr();
        Expect(TokenType.RightParen);
        Expect(TokenType.Then);
        var thenExpr = ParseExprSingle();
        Expect(TokenType.Else);
        var elseExpr = ParseExprSingle();

        return new IfExpr
        {
            Condition = condition,
            Then = thenExpr,
            Else = elseExpr,
            Line = token.Line,
            Column = token.Column
        };
    }

    protected ExprNode ParseSwitchExpr()
    {
        var token = Current();
        Expect(TokenType.Switch);
        Expect(TokenType.LeftParen);
        var operand = ParseExpr();
        Expect(TokenType.RightParen);

        var cases = new List<SwitchCaseClause>();

        while (Check(TokenType.Case))
        {
            Advance();
            var values = new List<ExprNode>();

            do
            {
                values.Add(ParseExprSingle());
            } while (Match(TokenType.Case));

            Expect(TokenType.Return);
            var result = ParseExprSingle();

            cases.Add(new SwitchCaseClause { Values = values, Result = result });
        }

        Expect(TokenType.Default);
        Expect(TokenType.Return);
        var defaultResult = ParseExprSingle();

        return new SwitchExpr
        {
            Operand = operand,
            Cases = cases,
            Default = defaultResult,
            Line = token.Line,
            Column = token.Column
        };
    }

    protected ExprNode ParseQuantifiedExpr()
    {
        var token = Current();
        bool isSome = Check(TokenType.Some);
        Advance();

        var bindings = new List<QuantifiedBinding>();

        do
        {
            Expect(TokenType.Dollar);
            var varName = ParseNCName();

            SequenceTypeNode? type = null;
            if (Match(TokenType.As))
            {
                type = ParseSequenceType();
            }

            Expect(TokenType.In);
            var expr = ParseExprSingle();

            bindings.Add(new QuantifiedBinding
            {
                Variable = varName,
                Type = type,
                Expression = expr
            });
        } while (Match(TokenType.Comma));

        Expect(TokenType.Satisfies);
        var satisfies = ParseExprSingle();

        return new QuantifiedExpr
        {
            IsSome = isSome,
            Bindings = bindings,
            Satisfies = satisfies,
            Line = token.Line,
            Column = token.Column
        };
    }

    protected virtual ExprNode ParseFlworExpr()
    {
        var token = Current();
        var clauses = new List<FlworClause>();

        // Initial for/let clause
        if (Check(TokenType.For))
            clauses.Add(ParseForClause());
        else
            clauses.Add(ParseLetClause());

        // Additional clauses
        while (true)
        {
            if (Check(TokenType.For))
                clauses.Add(ParseForClause());
            else if (Check(TokenType.Let))
                clauses.Add(ParseLetClause());
            else if (Check(TokenType.Where))
                clauses.Add(ParseWhereClause());
            else if (Check(TokenType.OrderBy) || (Check(TokenType.NCName) && Current().Value == "stable"))
                clauses.Add(ParseOrderByClause());
            else if (Check(TokenType.Group))
                clauses.Add(ParseGroupByClause());
            else if (Check(TokenType.Count))
                clauses.Add(ParseCountClause());
            else
                break;
        }

        Expect(TokenType.Return);
        var returnExpr = ParseExprSingle();

        return new FlworExpr
        {
            Clauses = clauses,
            Return = returnExpr,
            Line = token.Line,
            Column = token.Column
        };
    }

    private ForClause ParseForClause()
    {
        var token = Current();
        Expect(TokenType.For);
        Expect(TokenType.Dollar);
        var varName = ParseNCName();

        SequenceTypeNode? type = null;
        if (Match(TokenType.As))
        {
            type = ParseSequenceType();
        }

        bool allowingEmpty = false;
        if (Match(TokenType.Allowing))
        {
            Expect(TokenType.Empty);
            allowingEmpty = true;
        }

        string? posVar = null;
        if (Match(TokenType.At_KW))
        {
            Expect(TokenType.Dollar);
            posVar = ParseNCName();
        }

        Expect(TokenType.In);
        var expr = ParseExprSingle();

        return new ForClause
        {
            Variable = varName,
            Type = type,
            AllowingEmpty = allowingEmpty,
            PositionalVariable = posVar,
            Expression = expr,
            Line = token.Line,
            Column = token.Column
        };
    }

    private LetClause ParseLetClause()
    {
        var token = Current();
        Expect(TokenType.Let);
        Expect(TokenType.Dollar);
        var varName = ParseNCName();

        SequenceTypeNode? type = null;
        if (Match(TokenType.As))
        {
            type = ParseSequenceType();
        }

        Expect(TokenType.Assign);
        var expr = ParseExprSingle();

        return new LetClause
        {
            Variable = varName,
            Type = type,
            Expression = expr,
            Line = token.Line,
            Column = token.Column
        };
    }

    private WhereClause ParseWhereClause()
    {
        var token = Current();
        Expect(TokenType.Where);
        var condition = ParseExprSingle();

        return new WhereClause
        {
            Condition = condition,
            Line = token.Line,
            Column = token.Column
        };
    }

    private OrderByClause ParseOrderByClause()
    {
        var token = Current();
        bool stable = false;

        if (Check(TokenType.NCName) && Current().Value == "stable")
        {
            Advance();
            stable = true;
        }

        // "order by" might be lexed as a single token or two
        if (Check(TokenType.OrderBy))
        {
            Advance();
        }
        else if (Check(TokenType.NCName) && Current().Value == "order")
        {
            Advance();
            Expect(TokenType.By);
        }

        var specs = new List<OrderSpec>();

        do
        {
            var expr = ParseExprSingle();
            bool descending = false;
            bool emptyGreatest = false;
            string? collation = null;

            if (Match(TokenType.Ascending))
                descending = false;
            else if (Match(TokenType.Descending))
                descending = true;

            if (Match(TokenType.Empty))
            {
                if (Match(TokenType.NCName) && Previous().Value == "greatest")
                    emptyGreatest = true;
            }

            if (Match(TokenType.Collation))
            {
                if (Check(TokenType.StringLiteral))
                {
                    collation = Current().Value;
                    Advance();
                }
            }

            specs.Add(new OrderSpec
            {
                Expression = expr,
                Descending = descending,
                EmptyGreatest = emptyGreatest,
                Collation = collation
            });
        } while (Match(TokenType.Comma));

        return new OrderByClause
        {
            Stable = stable,
            Specs = specs,
            Line = token.Line,
            Column = token.Column
        };
    }

    private GroupByClause ParseGroupByClause()
    {
        var token = Current();
        Expect(TokenType.Group);
        Expect(TokenType.By);

        var specs = new List<GroupSpec>();

        do
        {
            Expect(TokenType.Dollar);
            var varName = ParseNCName();

            SequenceTypeNode? type = null;
            if (Match(TokenType.As))
            {
                type = ParseSequenceType();
            }

            ExprNode? expr = null;
            if (Match(TokenType.Assign))
            {
                expr = ParseExprSingle();
            }

            string? collation = null;
            if (Match(TokenType.Collation))
            {
                if (Check(TokenType.StringLiteral))
                {
                    collation = Current().Value;
                    Advance();
                }
            }

            specs.Add(new GroupSpec
            {
                Variable = varName,
                Type = type,
                Expression = expr,
                Collation = collation
            });
        } while (Match(TokenType.Comma));

        return new GroupByClause
        {
            Specs = specs,
            Line = token.Line,
            Column = token.Column
        };
    }

    private CountClause ParseCountClause()
    {
        var token = Current();
        Expect(TokenType.Count);
        Expect(TokenType.Dollar);
        var varName = ParseNCName();

        return new CountClause
        {
            Variable = varName,
            Line = token.Line,
            Column = token.Column
        };
    }

    protected ExprNode ParseTryCatchExpr()
    {
        var token = Current();
        Expect(TokenType.Try);
        Expect(TokenType.LeftBrace);
        var tryExpr = ParseExpr();
        Expect(TokenType.RightBrace);

        var catchClauses = new List<CatchClause>();

        while (Match(TokenType.Catch))
        {
            var errors = new List<XdmQName>();

            // Parse catch error list
            do
            {
                if (Match(TokenType.Asterisk))
                {
                    errors.Add(new XdmQName("*")); // Wildcard
                }
                else
                {
                    errors.Add(ParseQName());
                }
            } while (Match(TokenType.Pipe));

            Expect(TokenType.LeftBrace);
            var handler = ParseExpr();
            Expect(TokenType.RightBrace);

            catchClauses.Add(new CatchClause
            {
                Errors = errors,
                Handler = handler
            });
        }

        return new TryCatchExpr
        {
            TryExpr = tryExpr,
            CatchClauses = catchClauses,
            Line = token.Line,
            Column = token.Column
        };
    }

    #endregion

    #region Type Parsing

    protected SequenceTypeNode ParseSequenceType()
    {
        // Check for empty-sequence()
        if (Check(TokenType.EmptySequence))
        {
            Advance();
            Expect(TokenType.LeftParen);
            Expect(TokenType.RightParen);
            return new SequenceTypeNode
            {
                ItemType = new ItemTypeNode { Kind = ItemTypeKind.Empty },
                Occurrence = OccurrenceIndicator.ZeroOrMore
            };
        }

        var itemType = ParseItemType();
        var occurrence = OccurrenceIndicator.ExactlyOne;

        if (Match(TokenType.QuestionMark))
            occurrence = OccurrenceIndicator.ZeroOrOne;
        else if (Match(TokenType.Asterisk))
            occurrence = OccurrenceIndicator.ZeroOrMore;
        else if (Match(TokenType.Plus))
            occurrence = OccurrenceIndicator.OneOrMore;

        return new SequenceTypeNode
        {
            ItemType = itemType,
            Occurrence = occurrence
        };
    }

    protected ItemTypeNode ParseItemType()
    {
        var token = Current();

        // item()
        if (Check(TokenType.Item))
        {
            Advance();
            Expect(TokenType.LeftParen);
            Expect(TokenType.RightParen);
            return new ItemTypeNode { Kind = ItemTypeKind.Item };
        }

        // Node kind tests
        if (IsNodeKindTest())
        {
            var kindTest = ParseKindTest() as KindTestExpr;
            return new ItemTypeNode
            {
                Kind = kindTest!.Kind switch
                {
                    XdmNodeKind.Element => ItemTypeKind.Element,
                    XdmNodeKind.Attribute => ItemTypeKind.Attribute,
                    XdmNodeKind.Document => ItemTypeKind.Document,
                    XdmNodeKind.Text => ItemTypeKind.Text,
                    XdmNodeKind.Comment => ItemTypeKind.Comment,
                    XdmNodeKind.ProcessingInstruction => ItemTypeKind.ProcessingInstruction,
                    XdmNodeKind.Namespace => ItemTypeKind.Namespace,
                    _ => ItemTypeKind.Node
                },
                ElementName = kindTest.Name,
                TypeName = kindTest.TypeName
            };
        }

        // function(...)
        if (Check(TokenType.Function))
        {
            Advance();
            Expect(TokenType.LeftParen);
            // Skip function signature for now
            int depth = 1;
            while (depth > 0 && !IsAtEnd())
            {
                if (Check(TokenType.LeftParen)) depth++;
                if (Check(TokenType.RightParen)) depth--;
                Advance();
            }
            return new ItemTypeNode { Kind = ItemTypeKind.Function };
        }

        // map(...)
        if (Check(TokenType.Map))
        {
            Advance();
            Expect(TokenType.LeftParen);
            // Skip map type for now
            int depth = 1;
            while (depth > 0 && !IsAtEnd())
            {
                if (Check(TokenType.LeftParen)) depth++;
                if (Check(TokenType.RightParen)) depth--;
                Advance();
            }
            return new ItemTypeNode { Kind = ItemTypeKind.Map };
        }

        // array(...)
        if (Check(TokenType.Array))
        {
            Advance();
            Expect(TokenType.LeftParen);
            // Skip array type for now
            int depth = 1;
            while (depth > 0 && !IsAtEnd())
            {
                if (Check(TokenType.LeftParen)) depth++;
                if (Check(TokenType.RightParen)) depth--;
                Advance();
            }
            return new ItemTypeNode { Kind = ItemTypeKind.Array };
        }

        // Atomic type (QName)
        var typeName = ParseQName();
        return new ItemTypeNode
        {
            Kind = ItemTypeKind.AtomicType,
            TypeName = typeName
        };
    }

    protected (XdmQName typeName, bool allowEmpty) ParseSingleType()
    {
        var typeName = ParseQName();
        bool allowEmpty = Match(TokenType.QuestionMark);
        return (typeName, allowEmpty);
    }

    #endregion

    #region Helper Methods

    protected XdmQName ParseQName()
    {
        var token = Current();

        if (Check(TokenType.QName))
        {
            var parts = token.Value.Split(':', 2);
            Advance();
            return new XdmQName(
                _namespaces.GetValueOrDefault(parts[0], string.Empty),
                parts[1],
                parts[0]);
        }

        if (Check(TokenType.NCName))
        {
            var name = token.Value;
            Advance();
            return new XdmQName(name);
        }

        // Handle keyword being used as a name
        if (token.Type >= TokenType.For && token.Type <= TokenType.Nodes)
        {
            var name = token.Value;
            Advance();
            return new XdmQName(name);
        }

        throw ParseError($"Expected QName or NCName, got {token.Type}");
    }

    protected (string LocalName, string? Prefix, string? NamespaceUri) ParseVarName()
    {
        var qname = ParseQName();
        return (qname.LocalName, qname.HasPrefix ? qname.Prefix : null, qname.HasNamespace ? qname.NamespaceUri : null);
    }

    protected string ParseNCName()
    {
        var token = Current();

        if (Check(TokenType.NCName))
        {
            Advance();
            return token.Value;
        }

        // Handle keyword being used as a name
        if (token.Type >= TokenType.For && token.Type <= TokenType.Nodes)
        {
            Advance();
            return token.Value;
        }

        throw ParseError($"Expected NCName, got {token.Type}");
    }

    protected Token Current() => _tokens[_position];
    protected Token Previous() => _tokens[_position - 1];
    protected Token Peek(int offset) =>
        _position + offset < _tokens.Count ? _tokens[_position + offset] : _tokens[^1];

    protected bool IsAtEnd() => Current().Type == TokenType.Eof;

    protected bool Check(TokenType type) => !IsAtEnd() && Current().Type == type;

    /// <summary>
    /// Checks if the current token can be used as a name (NCName, QName, or keyword usable as name).
    /// </summary>
    protected bool IsNameToken()
    {
        var type = Current().Type;
        return type == TokenType.NCName ||
               type == TokenType.QName ||
               (type >= TokenType.For && type <= TokenType.Nodes);
    }

    /// <summary>
    /// Checks if the token at the given offset can be used as a name.
    /// </summary>
    protected bool IsNameToken(int offset)
    {
        var type = Peek(offset).Type;
        return type == TokenType.NCName ||
               type == TokenType.QName ||
               (type >= TokenType.For && type <= TokenType.Nodes);
    }

    protected bool Match(TokenType type)
    {
        if (Check(type))
        {
            Advance();
            return true;
        }
        return false;
    }

    protected void Advance()
    {
        if (!IsAtEnd()) _position++;
    }

    protected void Expect(TokenType type)
    {
        // Special case for EOF - Check() returns false at EOF
        if (type == TokenType.Eof)
        {
            if (!IsAtEnd())
                throw ParseError($"Expected {type}, got {Current().Type}");
            return;
        }
        if (!Check(type))
            throw ParseError($"Expected {type}, got {Current().Type}");
        Advance();
    }

    protected Exception ParseError(string message)
    {
        var token = Current();
        return new XdmException("XPST0003", $"{message} at line {token.Line}, column {token.Column}");
    }

    #endregion
}
