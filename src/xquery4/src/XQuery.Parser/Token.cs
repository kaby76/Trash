namespace XQuery.Parser;

/// <summary>
/// Token types for XPath/XQuery lexer.
/// </summary>
public enum TokenType
{
    // End of input
    Eof,

    // Literals
    IntegerLiteral,
    DecimalLiteral,
    DoubleLiteral,
    StringLiteral,

    // Names
    NCName,
    QName,
    URIQualifiedName,       // Q{uri}local
    BracedURILiteral,       // Q{uri}

    // Operators - Arithmetic
    Plus,                   // +
    Minus,                  // -
    Asterisk,               // *
    Div,                    // div
    IDiv,                   // idiv
    Mod,                    // mod

    // Operators - Comparison
    Eq,                     // eq (value comparison)
    Ne,                     // ne
    Lt,                     // lt
    Le,                     // le
    Gt,                     // gt
    Ge,                     // ge
    Equal,                  // = (general comparison)
    NotEqual,               // !=
    LessThan,               // <
    LessOrEqual,            // <=
    GreaterThan,            // >
    GreaterOrEqual,         // >=
    Is,                     // is (node comparison)
    Precedes,               // <<
    Follows,                // >>

    // Operators - Logical
    And,                    // and
    Or,                     // or

    // Operators - Sequence
    Comma,                  // ,
    To,                     // to
    Union,                  // union or |
    Intersect,              // intersect
    Except,                 // except
    Pipe,                   // |

    // Operators - Path
    Slash,                  // /
    SlashSlash,             // //
    At,                     // @
    DotDot,                 // ..
    Dot,                    // .
    ColonColon,             // ::

    // Operators - Other
    Arrow,                  // =>
    ThinArrow,              // ->
    QuestionMark,           // ?
    Exclamation,            // !
    Colon,                  // :
    Assign,                 // :=
    Concat,                 // ||
    Otherwise,              // otherwise

    // Delimiters
    LeftParen,              // (
    RightParen,             // )
    LeftBracket,            // [
    RightBracket,           // ]
    LeftBrace,              // {
    RightBrace,             // }

    // Keywords - FLWOR
    For,
    Let,
    Where,
    OrderBy,
    Ascending,
    Descending,
    EmptyGreatest,
    EmptyLeast,
    Collation,
    Return,
    In,
    Satisfies,
    Some,
    Every,
    Group,
    By,
    Count,
    Allowing,
    Empty,
    At_KW,                  // at keyword (vs @ symbol)

    // Keywords - Conditional
    If,
    Then,
    Else,
    Switch,
    Case,
    Default,
    Typeswitch,

    // Keywords - Quantified
    SomeKW,                 // some (quantified)
    EveryKW,                // every (quantified)

    // Keywords - Node constructors
    Element,
    Attribute,
    Document,
    Text,
    Comment,
    ProcessingInstruction,
    Namespace,
    NamespaceNode,

    // Keywords - Sequence types
    Item,
    Node,
    SchemaElement,
    SchemaAttribute,
    EmptySequence,
    Function,
    Map,
    Array,
    Record,

    // Keywords - Type
    As,
    Of,
    Instance,
    Treat,
    Castable,
    Cast,

    // Keywords - Module/Prolog
    Module,
    NamespaceKW,
    Import,
    Schema,
    DeclareKW,
    Variable,
    External,
    FunctionKW,
    Option,
    Ordering,
    Ordered,
    Unordered,
    BaseUri,
    CopyNamespaces,
    PreserveKW,
    NoPreserve,
    Inherit,
    NoInherit,
    DefaultKW,
    BoundarySpace,
    Strip,
    Construction,

    // Keywords - Try/Catch
    Try,
    Catch,

    // Keywords - Validate
    Validate,
    Strict,
    Lax,
    Type,

    // Axis names
    Ancestor,
    AncestorOrSelf,
    Child,
    Descendant,
    DescendantOrSelf,
    Following,
    FollowingSibling,
    Parent,
    Preceding,
    PrecedingSibling,
    Self,

    // XQuery Update keywords
    Insert,
    Delete,
    Replace,
    Rename,
    Copy,
    Modify,
    With,
    Into,
    After,
    Before,
    First,
    Last,
    Value,
    Nodes,

    // Other
    Dollar,                 // $
    Hash,                   // #
    Percent,                // %
    Tilde,                  // ~

    // Unknown/Error
    Unknown
}

/// <summary>
/// Represents a token from the lexer.
/// </summary>
public class Token
{
    public TokenType Type { get; }
    public string Value { get; }
    public int Line { get; }
    public int Column { get; }
    public int Position { get; }

    public Token(TokenType type, string value, int line, int column, int position)
    {
        Type = type;
        Value = value;
        Line = line;
        Column = column;
        Position = position;
    }

    public override string ToString()
    {
        if (string.IsNullOrEmpty(Value))
            return Type.ToString();
        return $"{Type}: {Value}";
    }

    public static Token Eof(int line, int column, int position) =>
        new(TokenType.Eof, string.Empty, line, column, position);
}
