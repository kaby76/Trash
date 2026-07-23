# XQuery 4.0 / XPath 4.0 Interpreter

A complete implementation of XQuery 4.0 and XPath 4.0 interpreters in C#, featuring the XDM 4.0 data model, full expression parsing, evaluation, and the XQuery Update Facility.

## Features

### Data Model (XDM 4.0)
- **Atomic Values**: Strings, numbers, booleans, dates, durations, QNames, and more
- **Nodes**: Documents, elements, attributes, text, comments, processing instructions
- **Sequences**: Ordered collections of items with full sequence operations
- **Maps**: Key-value collections (XPath 3.1+)
- **Arrays**: Ordered arrays (XPath 3.1+)
- **Functions**: First-class function items

### Parser
- Full XPath 4.0 expression parsing
- XQuery module and prolog support
- Context-sensitive keyword handling
- Comprehensive AST representation

### Evaluator
- Complete XPath expression evaluation
- FLWOR expressions (for, let, where, order by, return)
- Quantified expressions (some, every)
- Conditional expressions (if-then-else)
- Path expressions with all axes
- Function calls (built-in and user-defined)

### XQuery Update Facility
- `insert` - Insert nodes into documents
- `delete` - Remove nodes from documents
- `replace` - Replace nodes or values
- `rename` - Rename elements and attributes
- `transform` - Copy-modify-return expressions

### Function Library
- String functions (concat, substring, contains, etc.)
- Numeric functions (abs, floor, ceiling, round, etc.)
- Boolean functions (not, true, false)
- Sequence functions (count, sum, avg, min, max, etc.)
- Node functions (name, local-name, namespace-uri, etc.)
- Date/time functions
- Map and array functions

### I/O Support
- XML parsing and serialization
- JSON parsing and serialization
- YAML parsing

## Project Structure

```
xquery4/
├── src/
│   ├── XQuery.DataModel/     # XDM 4.0 data model types
│   ├── XQuery.Parser/        # Lexer, parser, and AST
│   ├── XQuery.Engine/        # XPath and XQuery evaluators
│   ├── XQuery.Functions/     # Built-in function library
│   └── XQuery.IO/            # XML, JSON, YAML readers/writers
├── tests/
│   └── XQuery.Tests/         # Comprehensive test suite
└── XQuery4.sln
```

## Building

```bash
dotnet build
```

## Running Tests

```bash
dotnet test
```

## Usage Examples

### Basic XPath Evaluation

```csharp
var doc = XmlDocumentReader.Parse("<root><item id='1'>Hello</item></root>");
var context = new EvaluationContext { ContextItem = doc };
var evaluator = new XPathEvaluator(context);

var parser = new XPathParser("//item[@id='1']/text()");
var result = evaluator.Evaluate(parser.Parse());
// result contains "Hello"
```

### XQuery Update

```csharp
var doc = XmlDocumentReader.Parse("<root><item/></root>");
var context = new EvaluationContext { ContextItem = doc.DocumentElement };

var insertExpr = new InsertExpr
{
    Source = new ElementConstructorExpr { Name = new XdmQName("newitem") },
    Target = new ContextItemExpr(),
    Position = InsertPosition.Into
};

var evaluator = new XQueryEvaluator(context);
evaluator.Evaluate(insertExpr);
// doc now contains <root><item/><newitem/></root>
```

### User-Defined Functions

```csharp
var module = new ModuleNode
{
    Prolog = new PrologNode
    {
        FunctionDecls = new List<FunctionDeclNode>
        {
            new FunctionDeclNode
            {
                Name = "double",
                Parameters = new List<ParameterNode> { new ParameterNode { Name = "n" } },
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
// result is 42
```

---

## Development Chat History

This project was developed through an interactive conversation with Claude (Anthropic's AI assistant). Below is the complete chat history from the development session.

### Session Summary

The conversation continued from a previous session where most of the XQuery 4.0 implementation was completed. The earlier session had implemented:

- XDM 4.0 data model (XdmItem, XdmNode, XdmSequence, XdmMap, XdmArray, etc.)
- XPath 4.0 parser with full expression support
- XPath evaluator with function library
- IO handlers for XML, JSON, and YAML
- Serializers for multiple output formats

The pending task was implementing the XQuery evaluation engine with update facility.

### Current Session Chat Log

---

**User:** fix the failing tests

**Assistant:** Let me run the tests to verify the GetFunction fix and see the current status.

[Ran tests - 2 failures remaining: Module_FunctionDeclaration and Parse_Predicate]

I see the issue. The function lookup is happening in `FunctionLibrary.Call`, not checking the context first. Let me look at how `VisitFunctionCall` works.

[Investigated the code flow through XPathEvaluator.VisitFunctionCall and EvaluationContext.GetFunction]

The code already checks context first. The issue is that `FunctionCallExpr.QName` automatically adds the `fn:` namespace for functions without a prefix. The function was registered as `"double"` (no namespace) but is being looked up as `fn:double`. My fix in `GetFunction` should handle this.

[Rebuilt and ran specific test - Module_FunctionDeclaration now passes]

Down to 1 failure. Let me examine the Parse_Predicate test.

The test parses "item[1]" and expects a FilterExpr. However, the parser is returning an AxisStepExpr.

The test expects a FilterExpr for `item[1]`, but in XPath, `item[1]` is actually an AxisStep (selecting child elements named "item" filtered by position). FilterExpr is for filtering primary expressions like `$x[1]` or `(//item)[1]`. The test expectation is incorrect.

[Fixed the test to correctly expect AxisStepExpr and added a new Parse_FilterExpr test]

[Ran all tests]

```
Passed!  - Failed:     0, Passed:   230, Skipped:     0, Total:   230
```

All 230 tests pass. Here's a summary of the fixes made:

1. **Module_FunctionDeclaration test** - Fixed by updating `EvaluationContext.GetFunction()` to handle user-defined functions that may be called with the `fn:` namespace prefix. When a function call has the default function namespace but the function was registered without a namespace, the lookup now tries both.

2. **Parse_Predicate test** - The test had incorrect expectations. In XPath, `item[1]` is an AxisStep expression (selecting child elements named "item" filtered by position), not a FilterExpr. FilterExpr is for filtering primary expressions like variable references (`$x[1]`). I corrected the test to expect `AxisStepExpr` and added a new `Parse_FilterExpr` test that properly tests FilterExpr with `$x[1]`.

---

**User:** commit these fixes

**Assistant:** [Checked git status - directory was not a git repository]

This directory is not a git repository. Would you like me to initialize one?

**User:** Yes, initialize git

**Assistant:** [Initialized git repository, created .gitignore, staged all files]

```
[master (root-commit) 17054f1] Initial commit: XQuery 4.0 and XPath 4.0 interpreter implementation
 42 files changed, 14957 insertions(+)
```

Done. Created initial commit `17054f1` with the complete XQuery 4.0/XPath 4.0 implementation:

- 42 files changed, 14,957 insertions
- All 230 tests passing
- Includes .gitignore for .NET projects

---

**User:** Thanks. Could you write out the entire chat history we had here, placing it in a readme.md file, along with a description of what the code implements? Then, please check that into the repo.

**Assistant:** [Created this README.md file]

---

### Key Files Modified During This Session

1. **src/XQuery.DataModel/EvaluationContext.cs** - Added fallback logic in `GetFunction()` to find user-defined functions called with the `fn:` namespace prefix.

2. **tests/XQuery.Tests/ParserTests.cs** - Fixed `Parse_Predicate` test to correctly expect `AxisStepExpr` instead of `FilterExpr`, and added new `Parse_FilterExpr` test.

### Previous Session Work (From Summary)

The previous session implemented:

- **XQueryEvaluator.cs** - XQuery evaluation with node constructors and update facility
- **Parser fixes** - Added `IsNameToken()` helper for context-sensitive keyword handling
- **EvaluationContext** - Moved to XQuery.DataModel to fix circular dependency
- **Various data model updates** - Added methods like `InsertChildAt`, `Rename`, `RemoveAttribute` to support update operations

---

## License

This project was created as a demonstration of XQuery 4.0 and XPath 4.0 implementation in C#.
