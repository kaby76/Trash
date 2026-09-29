(: Sort all ANTLR4 parser rules alphabetically, including EOF-terminated
   start rules. This script is for parser grammars; lexer rules are not
   reordered.

   dotnet trash parse Grammar.g4 | dotnet trash xquery -q sort-antlr4.xq
:)
let $rules := /grammarSpec/rules
let $parser-rules := $rules/ruleSpec[parserRuleSpec]
let $sorted :=
    for $rule in $parser-rules
    let $name := string($rule/parserRuleSpec/RULE_REF[1])
    order by lower-case($name), $name
    return $rule
let $newline := "
"
return (
    delete node $parser-rules,
    for $rule in $sorted
    return (
        insert node $newline as last into $rules,
        insert node $rule as last into $rules
    )
)
