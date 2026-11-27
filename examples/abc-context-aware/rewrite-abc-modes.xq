(: ABC's BODY mode can be inferred from the parser: header expects KFIELD,
   while body expects music tokens and FIELD. Move its distinct token rules
   into the default mode and remove the aliases and mode switch. This is an
   ABC-specific rewrite, not a general equivalence for lexer modes. :)
let $grammar := /grammarSpec
let $rules := $grammar/rules
let $body := $grammar/modeSpec[identifier/*/text() = 'BODY']
let $key := $rules/ruleSpec/lexerRuleSpec[TOKEN_REF/text() = 'KFIELD']
return
  if (fn:count($grammar) != 1 or
      fn:string($grammar/grammarDecl/identifier/*[1]) != 'abcLexer' or
      fn:count($body) != 1 or
      fn:count($key) != 1 or
      fn:count($key/lexerRuleBlock/lexerAltList/lexerAlt/lexerCommands/
        lexerCommand[fn:string(lexerCommandName) = 'mode' and
          lexerCommandExpr/identifier/*/text() = 'BODY']) != 1 or
      fn:count($body/lexerRuleSpec[TOKEN_REF/text() = ('BCOMMENT', 'BWS', 'XFIELD2', 'BFIELD')]) != 4)
    then fn:error((), 'Expected the original ABC lexer modes and aliases')
    else (
      delete node $key/lexerRuleBlock/lexerAltList/lexerAlt/lexerCommands,
      for $rule in $body/lexerRuleSpec[
          not(TOKEN_REF/text() = ('BCOMMENT', 'BWS', 'XFIELD2', 'BFIELD'))]
        return insert node element ruleSpec {
          element lexerRuleSpec { fn:concat('&#10;', fn:string($rule)) }
        } as last into $rules,
      delete node $body
    )
