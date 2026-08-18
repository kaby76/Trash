using System.Reflection;
using Xunit;
using XQuery.Parser;
using XQuery.Parser.Ast;

namespace XQuery.Tests;

/// <summary>
/// File-based grammar coverage tests. Each file under examples/ exercises one or
/// a small group of grammar constructs. Files are parsed end-to-end through the
/// ANTLR4 grammar and AST builder, so failures indicate either a grammar gap or
/// an unimplemented AstBuilder node.
///
/// File extension determines the parser used:
///   .xpath   — XPathParser  (XPath 4.0 grammar)
///   .xquery  — XQueryParser.Parse()        (XQuery 4.0 expression)
///   .xqm     — XQueryParser.ParseModule()  (XQuery 4.0 full module)
///
/// Example files live under:
///   examples/xpath4/   — XPath 4.0 constructs
///   examples/xquery4/  — XQuery 4.0 constructs
///
/// Files inside any _pending/ subdirectory are excluded from discovery.
/// Move a file into _pending/ when the grammar parses it but the AstBuilder
/// does not yet handle the resulting node.  Current pending areas:
///   xpath4/_pending/let/                         — let destructuring ($(...), $[...], ${ })
///   xpath4/_pending/constructors/                — namespace computed constructor
///   xpath4/_pending/if/                          — braced if: if (c) { e } (no else)
///   xpath4/_pending/kind-tests/                  — union node test: (element()|text())
///   xpath4/_pending/axes/                        — dynamic node test: axis::{ expr }
///   xpath4/_pending/arrow/                       — dynamic arrow target: expr => $f()
///   xquery4/_pending/typeswitch/                 — typeswitch (AstBuilder: "not yet supported")
///   xquery4/_pending/validate/                   — validate lax/strict (AstBuilder: NullReferenceException)
///   xquery4/_pending/constructors/               — ordered{}/unordered{} (AstBuilder: unknown primary)
///   xquery4/_pending/update/                     — rename node; transform/copy (grammar double-$ quirk)
///   xquery4/_pending/expressions/if/             — braced if
///   xquery4/_pending/expressions/kind-tests/     — union node test, dynamic node test
///   xquery4/_pending/expressions/axes/           — dynamic node test
///   xquery4/_pending/expressions/arrow/          — dynamic arrow target
///   xquery4/_pending/expressions/let/            — let destructuring
///   xquery4/_pending/expressions/for/            — window clauses (tumbling/sliding)
///   xquery4/_pending/expressions/extension/      — extension expressions (pragma)
///   xquery4/_pending/expressions/constructors/   — namespace constructor; direct XML constructors
///                                                   (dirCommentConstructor, dirPIConstructor, directConstructor)
///   xquery4 unreachable rules (dirElemConstructor commented out, so no element-content lexer mode):
///     quotStringLiteral — only reachable inside direct element constructor attribute values
///     cDataSection      — only reachable inside direct element constructor content
///   xquery4/_pending/expressions/types/           — complex castTarget: choiceItemType, enumerationType, typedArrayType, typedMapType, typedRecordType
///   xpath4/_pending/types/                        — complex castTarget: choiceItemType, enumerationType, typedArrayType, typedMapType, typedRecordType
///   xquery4/_pending/modules/prolog/             — declare type; declare record; while/trace clauses
///   xpath4/_pending/namespace/                   — URIQualifiedName Q{uri}local: ANTLR4 4.13.1 lexer bug; produces QName for 'Q' instead of URIQualifiedName
///   xquery4/_pending/expressions/namespace/      — same URIQualifiedName lexer bug
///</summary>
public class GrammarCoverageTests
{
    private static readonly string ExamplesDir = Path.Combine(
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
        "examples");

    // ── Data sources ──────────────────────────────────────────────────────────

    public static IEnumerable<object[]> XPath4Files()
    {
        var dir = Path.Combine(ExamplesDir, "xpath4");
        return Directory.GetFiles(dir, "*.xpath", SearchOption.AllDirectories)
            .Where(f => !f.Contains("_pending"))
            .OrderBy(f => f)
            .Select(f => new object[] { f });
    }

    public static IEnumerable<object[]> XQuery4ExprFiles()
    {
        var dir = Path.Combine(ExamplesDir, "xquery4");
        return Directory.GetFiles(dir, "*.xquery", SearchOption.AllDirectories)
            .Where(f => !f.Contains("_pending"))
            .OrderBy(f => f)
            .Select(f => new object[] { f });
    }

    public static IEnumerable<object[]> XQuery4ModuleFiles()
    {
        var dir = Path.Combine(ExamplesDir, "xquery4");
        return Directory.GetFiles(dir, "*.xqm", SearchOption.AllDirectories)
            .Where(f => !f.Contains("_pending"))
            .OrderBy(f => f)
            .Select(f => new object[] { f });
    }

    // ── Tests ─────────────────────────────────────────────────────────────────

    [Theory]
    [MemberData(nameof(XPath4Files))]
    public void XPath4_Parses(string file)
    {
        var input = File.ReadAllText(file);
        ExprNode ast = new XPathParser(input).Parse();
        Assert.NotNull(ast);
    }

    [Theory]
    [MemberData(nameof(XQuery4ExprFiles))]
    public void XQuery4_Expression_Parses(string file)
    {
        var input = File.ReadAllText(file);
        ExprNode ast = new XQueryParser(input).Parse();
        Assert.NotNull(ast);
    }

    [Theory]
    [MemberData(nameof(XQuery4ModuleFiles))]
    public void XQuery4_Module_Parses(string file)
    {
        var input = File.ReadAllText(file);
        ModuleNode module = new XQueryParser(input).ParseModule();
        Assert.NotNull(module);
    }
}
