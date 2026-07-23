using System.Text;
using XQuery.DataModel;

namespace XQuery.Parser;

/// <summary>
/// Lexer for XPath 4.0 and XQuery 4.0 expressions.
/// </summary>
public class Lexer
{
    private readonly string _input;
    private int _position;
    private int _line;
    private int _column;
    private readonly List<Token> _tokens;
    private TokenType _lastTokenType;

    // Keywords that are context-sensitive
    private static readonly Dictionary<string, TokenType> Keywords = new(StringComparer.Ordinal)
    {
        // FLWOR
        ["for"] = TokenType.For,
        ["let"] = TokenType.Let,
        ["where"] = TokenType.Where,
        ["order"] = TokenType.OrderBy, // Note: "order by" is two tokens
        ["by"] = TokenType.By,
        ["ascending"] = TokenType.Ascending,
        ["descending"] = TokenType.Descending,
        ["collation"] = TokenType.Collation,
        ["return"] = TokenType.Return,
        ["in"] = TokenType.In,
        ["satisfies"] = TokenType.Satisfies,
        ["some"] = TokenType.Some,
        ["every"] = TokenType.Every,
        ["group"] = TokenType.Group,
        ["count"] = TokenType.Count,
        ["allowing"] = TokenType.Allowing,
        ["empty"] = TokenType.Empty,
        ["at"] = TokenType.At_KW,

        // Conditional
        ["if"] = TokenType.If,
        ["then"] = TokenType.Then,
        ["else"] = TokenType.Else,
        ["switch"] = TokenType.Switch,
        ["case"] = TokenType.Case,
        ["default"] = TokenType.Default,
        ["typeswitch"] = TokenType.Typeswitch,

        // Operators
        ["and"] = TokenType.And,
        ["or"] = TokenType.Or,
        ["div"] = TokenType.Div,
        ["idiv"] = TokenType.IDiv,
        ["mod"] = TokenType.Mod,
        ["eq"] = TokenType.Eq,
        ["ne"] = TokenType.Ne,
        ["lt"] = TokenType.Lt,
        ["le"] = TokenType.Le,
        ["gt"] = TokenType.Gt,
        ["ge"] = TokenType.Ge,
        ["is"] = TokenType.Is,
        ["to"] = TokenType.To,
        ["union"] = TokenType.Union,
        ["intersect"] = TokenType.Intersect,
        ["except"] = TokenType.Except,
        ["otherwise"] = TokenType.Otherwise,

        // Node constructors
        ["element"] = TokenType.Element,
        ["attribute"] = TokenType.Attribute,
        ["document"] = TokenType.Document,
        ["text"] = TokenType.Text,
        ["comment"] = TokenType.Comment,
        ["processing-instruction"] = TokenType.ProcessingInstruction,
        ["namespace"] = TokenType.Namespace,
        ["namespace-node"] = TokenType.NamespaceNode,

        // Sequence types
        ["item"] = TokenType.Item,
        ["node"] = TokenType.Node,
        ["schema-element"] = TokenType.SchemaElement,
        ["schema-attribute"] = TokenType.SchemaAttribute,
        ["empty-sequence"] = TokenType.EmptySequence,
        ["function"] = TokenType.Function,
        ["map"] = TokenType.Map,
        ["array"] = TokenType.Array,
        ["record"] = TokenType.Record,

        // Type
        ["as"] = TokenType.As,
        ["of"] = TokenType.Of,
        ["instance"] = TokenType.Instance,
        ["treat"] = TokenType.Treat,
        ["castable"] = TokenType.Castable,
        ["cast"] = TokenType.Cast,

        // Module/Prolog
        ["module"] = TokenType.Module,
        ["import"] = TokenType.Import,
        ["schema"] = TokenType.Schema,
        ["declare"] = TokenType.DeclareKW,
        ["variable"] = TokenType.Variable,
        ["external"] = TokenType.External,
        ["option"] = TokenType.Option,
        ["ordering"] = TokenType.Ordering,
        ["ordered"] = TokenType.Ordered,
        ["unordered"] = TokenType.Unordered,
        ["base-uri"] = TokenType.BaseUri,
        ["copy-namespaces"] = TokenType.CopyNamespaces,
        ["preserve"] = TokenType.PreserveKW,
        ["no-preserve"] = TokenType.NoPreserve,
        ["inherit"] = TokenType.Inherit,
        ["no-inherit"] = TokenType.NoInherit,
        ["boundary-space"] = TokenType.BoundarySpace,
        ["strip"] = TokenType.Strip,
        ["construction"] = TokenType.Construction,

        // Try/Catch
        ["try"] = TokenType.Try,
        ["catch"] = TokenType.Catch,

        // Validate
        ["validate"] = TokenType.Validate,
        ["strict"] = TokenType.Strict,
        ["lax"] = TokenType.Lax,
        ["type"] = TokenType.Type,

        // Axes
        ["ancestor"] = TokenType.Ancestor,
        ["ancestor-or-self"] = TokenType.AncestorOrSelf,
        ["child"] = TokenType.Child,
        ["descendant"] = TokenType.Descendant,
        ["descendant-or-self"] = TokenType.DescendantOrSelf,
        ["following"] = TokenType.Following,
        ["following-sibling"] = TokenType.FollowingSibling,
        ["parent"] = TokenType.Parent,
        ["preceding"] = TokenType.Preceding,
        ["preceding-sibling"] = TokenType.PrecedingSibling,
        ["self"] = TokenType.Self,

        // XQuery Update
        ["insert"] = TokenType.Insert,
        ["delete"] = TokenType.Delete,
        ["replace"] = TokenType.Replace,
        ["rename"] = TokenType.Rename,
        ["copy"] = TokenType.Copy,
        ["modify"] = TokenType.Modify,
        ["with"] = TokenType.With,
        ["into"] = TokenType.Into,
        ["after"] = TokenType.After,
        ["before"] = TokenType.Before,
        ["first"] = TokenType.First,
        ["last"] = TokenType.Last,
        ["value"] = TokenType.Value,
        ["nodes"] = TokenType.Nodes,
    };

    public Lexer(string input)
    {
        _input = input ?? string.Empty;
        _position = 0;
        _line = 1;
        _column = 1;
        _tokens = new List<Token>();
        _lastTokenType = TokenType.Eof;
    }

    public List<Token> Tokenize()
    {
        _tokens.Clear();

        while (!IsAtEnd())
        {
            SkipWhitespaceAndComments();
            if (IsAtEnd()) break;

            var token = NextToken();
            if (token != null)
            {
                _tokens.Add(token);
                _lastTokenType = token.Type;
            }
        }

        _tokens.Add(Token.Eof(_line, _column, _position));
        return _tokens;
    }

    private Token? NextToken()
    {
        int startLine = _line;
        int startColumn = _column;
        int startPosition = _position;

        char c = Current();

        // String literals
        if (c == '"' || c == '\'')
            return ScanString(c, startLine, startColumn, startPosition);

        // Numbers
        if (char.IsDigit(c) || (c == '.' && char.IsDigit(Peek(1))))
            return ScanNumber(startLine, startColumn, startPosition);

        // URI-qualified name Q{...}
        if (c == 'Q' && Peek(1) == '{')
            return ScanURIQualifiedName(startLine, startColumn, startPosition);

        // Operators and punctuation
        var opToken = ScanOperator(startLine, startColumn, startPosition);
        if (opToken != null) return opToken;

        // Names (NCName, QName, keywords)
        if (IsNameStartChar(c))
            return ScanName(startLine, startColumn, startPosition);

        // Unknown character
        Advance();
        return new Token(TokenType.Unknown, c.ToString(), startLine, startColumn, startPosition);
    }

    private Token ScanString(char quote, int startLine, int startColumn, int startPosition)
    {
        Advance(); // consume opening quote
        var sb = new StringBuilder();

        while (!IsAtEnd())
        {
            char c = Current();
            if (c == quote)
            {
                if (Peek(1) == quote)
                {
                    // Escaped quote
                    sb.Append(quote);
                    Advance();
                    Advance();
                }
                else
                {
                    // End of string
                    Advance();
                    break;
                }
            }
            else
            {
                sb.Append(c);
                Advance();
            }
        }

        return new Token(TokenType.StringLiteral, sb.ToString(), startLine, startColumn, startPosition);
    }

    private Token ScanNumber(int startLine, int startColumn, int startPosition)
    {
        var sb = new StringBuilder();
        bool hasDecimal = false;
        bool hasExponent = false;

        // Integer part
        while (!IsAtEnd() && char.IsDigit(Current()))
        {
            sb.Append(Current());
            Advance();
        }

        // Decimal part
        if (!IsAtEnd() && Current() == '.' && char.IsDigit(Peek(1)))
        {
            hasDecimal = true;
            sb.Append('.');
            Advance();
            while (!IsAtEnd() && char.IsDigit(Current()))
            {
                sb.Append(Current());
                Advance();
            }
        }

        // Exponent part
        if (!IsAtEnd() && (Current() == 'e' || Current() == 'E'))
        {
            hasExponent = true;
            sb.Append(Current());
            Advance();

            if (!IsAtEnd() && (Current() == '+' || Current() == '-'))
            {
                sb.Append(Current());
                Advance();
            }

            while (!IsAtEnd() && char.IsDigit(Current()))
            {
                sb.Append(Current());
                Advance();
            }
        }

        var tokenType = hasExponent ? TokenType.DoubleLiteral :
                       hasDecimal ? TokenType.DecimalLiteral :
                       TokenType.IntegerLiteral;

        return new Token(tokenType, sb.ToString(), startLine, startColumn, startPosition);
    }

    private Token ScanURIQualifiedName(int startLine, int startColumn, int startPosition)
    {
        Advance(); // consume 'Q'
        Advance(); // consume '{'

        var uri = new StringBuilder();
        while (!IsAtEnd() && Current() != '}')
        {
            uri.Append(Current());
            Advance();
        }

        if (!IsAtEnd())
            Advance(); // consume '}'

        // Check if there's a local name following
        if (!IsAtEnd() && IsNameStartChar(Current()))
        {
            var localName = ScanNCName();
            return new Token(TokenType.URIQualifiedName,
                $"Q{{{uri}}}{localName}", startLine, startColumn, startPosition);
        }

        return new Token(TokenType.BracedURILiteral,
            $"Q{{{uri}}}", startLine, startColumn, startPosition);
    }

    private Token? ScanOperator(int startLine, int startColumn, int startPosition)
    {
        char c = Current();
        char next = Peek(1);

        switch (c)
        {
            case '+': Advance(); return new Token(TokenType.Plus, "+", startLine, startColumn, startPosition);
            case '-':
                if (next == '>')
                {
                    Advance(); Advance();
                    return new Token(TokenType.ThinArrow, "->", startLine, startColumn, startPosition);
                }
                Advance();
                return new Token(TokenType.Minus, "-", startLine, startColumn, startPosition);
            case '*': Advance(); return new Token(TokenType.Asterisk, "*", startLine, startColumn, startPosition);
            case ',': Advance(); return new Token(TokenType.Comma, ",", startLine, startColumn, startPosition);
            case '(': Advance(); return new Token(TokenType.LeftParen, "(", startLine, startColumn, startPosition);
            case ')': Advance(); return new Token(TokenType.RightParen, ")", startLine, startColumn, startPosition);
            case '[': Advance(); return new Token(TokenType.LeftBracket, "[", startLine, startColumn, startPosition);
            case ']': Advance(); return new Token(TokenType.RightBracket, "]", startLine, startColumn, startPosition);
            case '{': Advance(); return new Token(TokenType.LeftBrace, "{", startLine, startColumn, startPosition);
            case '}': Advance(); return new Token(TokenType.RightBrace, "}", startLine, startColumn, startPosition);
            case '$': Advance(); return new Token(TokenType.Dollar, "$", startLine, startColumn, startPosition);
            case '#': Advance(); return new Token(TokenType.Hash, "#", startLine, startColumn, startPosition);
            case '%': Advance(); return new Token(TokenType.Percent, "%", startLine, startColumn, startPosition);
            case '~': Advance(); return new Token(TokenType.Tilde, "~", startLine, startColumn, startPosition);
            case '@': Advance(); return new Token(TokenType.At, "@", startLine, startColumn, startPosition);
            case '?': Advance(); return new Token(TokenType.QuestionMark, "?", startLine, startColumn, startPosition);

            case '|':
                if (next == '|')
                {
                    Advance(); Advance();
                    return new Token(TokenType.Concat, "||", startLine, startColumn, startPosition);
                }
                Advance();
                return new Token(TokenType.Pipe, "|", startLine, startColumn, startPosition);

            case '!':
                if (next == '=')
                {
                    Advance(); Advance();
                    return new Token(TokenType.NotEqual, "!=", startLine, startColumn, startPosition);
                }
                Advance();
                return new Token(TokenType.Exclamation, "!", startLine, startColumn, startPosition);

            case '=':
                if (next == '>')
                {
                    Advance(); Advance();
                    return new Token(TokenType.Arrow, "=>", startLine, startColumn, startPosition);
                }
                Advance();
                return new Token(TokenType.Equal, "=", startLine, startColumn, startPosition);

            case '<':
                if (next == '=')
                {
                    Advance(); Advance();
                    return new Token(TokenType.LessOrEqual, "<=", startLine, startColumn, startPosition);
                }
                if (next == '<')
                {
                    Advance(); Advance();
                    return new Token(TokenType.Precedes, "<<", startLine, startColumn, startPosition);
                }
                Advance();
                return new Token(TokenType.LessThan, "<", startLine, startColumn, startPosition);

            case '>':
                if (next == '=')
                {
                    Advance(); Advance();
                    return new Token(TokenType.GreaterOrEqual, ">=", startLine, startColumn, startPosition);
                }
                if (next == '>')
                {
                    Advance(); Advance();
                    return new Token(TokenType.Follows, ">>", startLine, startColumn, startPosition);
                }
                Advance();
                return new Token(TokenType.GreaterThan, ">", startLine, startColumn, startPosition);

            case '/':
                if (next == '/')
                {
                    Advance(); Advance();
                    return new Token(TokenType.SlashSlash, "//", startLine, startColumn, startPosition);
                }
                Advance();
                return new Token(TokenType.Slash, "/", startLine, startColumn, startPosition);

            case '.':
                if (next == '.')
                {
                    Advance(); Advance();
                    return new Token(TokenType.DotDot, "..", startLine, startColumn, startPosition);
                }
                if (!char.IsDigit(next))
                {
                    Advance();
                    return new Token(TokenType.Dot, ".", startLine, startColumn, startPosition);
                }
                return null; // Let number scanner handle it

            case ':':
                if (next == ':')
                {
                    Advance(); Advance();
                    return new Token(TokenType.ColonColon, "::", startLine, startColumn, startPosition);
                }
                if (next == '=')
                {
                    Advance(); Advance();
                    return new Token(TokenType.Assign, ":=", startLine, startColumn, startPosition);
                }
                Advance();
                return new Token(TokenType.Colon, ":", startLine, startColumn, startPosition);
        }

        return null;
    }

    private Token ScanName(int startLine, int startColumn, int startPosition)
    {
        var name = ScanNCName();

        // Check for QName (prefix:localname)
        if (!IsAtEnd() && Current() == ':' && Peek(1) != ':')
        {
            if (IsNameStartChar(Peek(1)))
            {
                Advance(); // consume ':'
                var localName = ScanNCName();
                var qname = $"{name}:{localName}";

                // Check if it's a keyword with prefix (it won't be)
                return new Token(TokenType.QName, qname, startLine, startColumn, startPosition);
            }
        }

        // Check if it's a keyword
        if (Keywords.TryGetValue(name, out var keywordType))
            return new Token(keywordType, name, startLine, startColumn, startPosition);

        return new Token(TokenType.NCName, name, startLine, startColumn, startPosition);
    }

    private string ScanNCName()
    {
        var sb = new StringBuilder();

        if (IsNameStartChar(Current()))
        {
            sb.Append(Current());
            Advance();
        }

        while (!IsAtEnd() && IsNameChar(Current()))
        {
            sb.Append(Current());
            Advance();
        }

        return sb.ToString();
    }

    private void SkipWhitespaceAndComments()
    {
        while (!IsAtEnd())
        {
            char c = Current();

            // Whitespace
            if (char.IsWhiteSpace(c))
            {
                Advance();
                continue;
            }

            // XPath/XQuery comment (: ... :)
            if (c == '(' && Peek(1) == ':')
            {
                SkipComment();
                continue;
            }

            break;
        }
    }

    private void SkipComment()
    {
        Advance(); // consume '('
        Advance(); // consume ':'
        int depth = 1;

        while (!IsAtEnd() && depth > 0)
        {
            if (Current() == '(' && Peek(1) == ':')
            {
                depth++;
                Advance();
                Advance();
            }
            else if (Current() == ':' && Peek(1) == ')')
            {
                depth--;
                Advance();
                Advance();
            }
            else
            {
                Advance();
            }
        }
    }

    private static bool IsNameStartChar(char c)
    {
        return char.IsLetter(c) || c == '_';
    }

    private static bool IsNameChar(char c)
    {
        return char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == '.';
    }

    private char Current()
    {
        if (IsAtEnd()) return '\0';
        return _input[_position];
    }

    private char Peek(int offset)
    {
        int pos = _position + offset;
        if (pos >= _input.Length) return '\0';
        return _input[pos];
    }

    private void Advance()
    {
        if (IsAtEnd()) return;

        if (_input[_position] == '\n')
        {
            _line++;
            _column = 1;
        }
        else
        {
            _column++;
        }

        _position++;
    }

    private bool IsAtEnd() => _position >= _input.Length;
}
