namespace AntlrJson;

// Compiled, target-independent rewrite data stored in the parser .interp.
// Template operations are "capture", "splice", and "node".
public sealed record AstMappingTable(
    string Name, bool SingleChild, bool ErrorOtherwise, AstMappingRuleTable[] Rules);

public sealed record AstMappingRuleTable(
    string RuleName, AstPatternPartTable[] Pattern, AstTemplateTable Result);

public sealed record AstPatternPartTable(
    string Capture, string NodeName, string Literal, string AstKind,
    int Min, int Max);

public sealed record AstTemplateTable(
    string Op, string Name, string TextCapture, AstTemplateTable[] Children);
