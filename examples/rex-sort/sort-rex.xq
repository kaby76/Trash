(: Sort REx syntax and lexical productions independently, case-insensitively.
   Move parse-tree nodes (rather than rebuilding text), preserving comments and
   formatting attached to each rule. Non-rule lexical directives remain in the
   lexical section (and are emitted before the sorted productions).

   dotnet trash parse Grammar.rex | dotnet trash xquery -q sort-rex.xq
:)
let $syntax := /grammar_/syntaxDefinition
let $lexical := /grammar_/lexicalDefinition
let $syntax-rules :=
    for $rule in $syntax/syntaxProduction
    order by lower-case(string($rule/name[1])), string($rule/name[1])
    return $rule
let $lexical-rules :=
    for $rule in $lexical/lexicalProduction
    let $name := if ($rule/name) then string($rule/name[1]) else "."
    order by lower-case($name), $name
    return $rule
let $newline := "
"
return (
    delete node $syntax/syntaxProduction,
    for $rule in $syntax-rules
    return (
        insert node $newline as last into $syntax,
        insert node $rule as last into $syntax
    ),
    delete node $lexical/lexicalProduction,
    for $rule in $lexical-rules
    return (
        insert node $newline as last into $lexical,
        insert node $rule as last into $lexical
    )
)
