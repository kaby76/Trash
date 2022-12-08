# Tree Sitter converter

This is a Tree Sitter to EBNF converter. As Tree Sitter uses JavaScript to specify the grammar,
the parser here is a JavaScript parser. The tree walker (only implemented in C#), looks for
//propertyAssignment[propertyName/identifierName/identifier/Identifier[text()='rules']]/singleExpression/objectLiteral/propertyAssignment
nodes and the converts those to EBNF via a standard tree walking visitor.
