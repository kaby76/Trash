grammar AstMapping;

// Syntax of the external CST-to-AST rewrite mappings consumed by
// trparse --allstar --ast-map. Keep this in step with MappingParser in
// src/trparse/AllStarAtnParser/AstMapping.cs.

mapping
    : 'ast' identifier '{' statement* '}' EOF
    ;

statement
    : defaultStatement
    | mappingRule
    ;

defaultStatement
    : 'default' 'single-child' '->' 'child' ';'
    | 'default' 'otherwise' '->' 'error' ';'
    ;

mappingRule
    : identifier '(' patternList? ')' '->' template ';'
    ;

patternList
    : patternPart (',' patternPart)*
    ;

patternPart
    : (identifier ':')? (identifier | QuotedString)
      ('[' identifier ']')? quantifier?
    ;

quantifier
    : '?'
    | '*'
    | '+'
    ;

template
    : '...' identifier
    | '^' '(' identifier ('[' identifier ']')? template* ')'
    | identifier
    ;

// Keywords can also be rule, capture, and AST-node names in the current
// hand-written mapping parser.
identifier
    : Identifier
    | 'ast'
    | 'default'
    | 'single-child'
    | 'otherwise'
    | 'child'
    | 'error'
    ;

QuotedString
    : '\'' ('\\' . | ~['\\])* '\''
    ;

Identifier
    : [a-zA-Z_] [a-zA-Z0-9_-]*
    ;

LineComment
    : '//' ~[\r\n]* -> skip
    ;

Whitespace
    : [ \t\r\n\f]+ -> skip
    ;
