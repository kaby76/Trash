(: Run on a parsed combined ANTLR4 grammar before trsplit. Each distinct
   parser literal becomes a structural lexerRuleSpec before existing lexer
   rules, so trsplit copies it into the generated lexer grammar. :)

declare function local:unused-prefix($names, $positions, $prefix) {
  if (some $i in $positions
      satisfies fn:concat($prefix, fn:string($i)) = $names)
    then local:unused-prefix($names, $positions, fn:concat($prefix, '_'))
    else $prefix
};

let $rules := (/grammarSpec/rules)[1]
let $first-lexer-rule := ($rules/ruleSpec[lexerRuleSpec])[1]
let $existing-literals := (
  for $rule in $rules/ruleSpec/lexerRuleSpec[not(FRAGMENT)]
  let $alts := $rule/lexerRuleBlock/lexerAltList/lexerAlt
  let $elements := $alts/lexerElements/lexerElement
  where fn:count($alts) = 1 and fn:count($elements) = 1 and
        not($alts/lexerCommands) and not($elements/ebnfSuffix)
  return $elements/lexerAtom/terminalDef/STRING_LITERAL/text()
)
let $literals := fn:distinct-values(
  $rules/ruleSpec/parserRuleSpec//STRING_LITERAL[
    parent::terminalDef or parent::setElement
  ]/text()[not(. = $existing-literals)]
)
let $names := $rules/ruleSpec/lexerRuleSpec/TOKEN_REF/text()
let $prefix := local:unused-prefix($names, 1 to fn:count($literals), 'TRSPLIT_LITERAL_')
return
  for $literal at $position in $literals
  let $rule := element ruleSpec {
    element lexerRuleSpec {
      fn:concat('&#10;', $prefix, fn:string($position), ' : ',
                fn:string($literal), ' ;')
    }
  }
  return
    if (fn:exists($first-lexer-rule))
      then insert node $rule before $first-lexer-rule
      else insert node $rule as last into $rules
