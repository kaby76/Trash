(: Render a parsed ANTLR4 grammar in the class-like syntax sketched in
   https://github.com/antlr-ng/antlr-ng/issues/129#issuecomment-6771744669 .
   This is illustrative output, not a currently accepted antlr-ng dialect. :)

declare function local:source($node) {
  (: Hidden WS may be attached to a parent rather than its original token.
     Emit token separators explicitly; retain comments and token contents. :)
  string-join(
    for $part in $node/descendant-or-self::*
    return concat(
      string-join(
        for $attribute in $part/@*
        return if (name($attribute) = 'WS') then ''
               else concat(string($attribute), '&#10;'),
        ''
      ),
      if ($part/* or string($part) = '') then ''
      else concat(string($part), ' ')
    ),
    ''
  )
};
declare function local:indent($node, $pad) {
  let $source := local:source($node)
  let $normalized := fn:replace($source, '\r\n?', '&#10;')
  let $trimmed := fn:replace(fn:replace($normalized, '^\s+', ''), '\s+$', '')
  return concat($pad, fn:replace($trimmed, '&#10;', concat('&#10;', $pad)))
};

declare function local:render-rules($rules, $pad) {
  string-join(
    for $rule in $rules
    return concat(local:indent($rule, $pad), '&#10;'),
    ''
  )
};

declare function local:priority($rules, $pad) {
  let $tokens := $rules[not(FRAGMENT)]
  return
    if (count($tokens) lt 2) then ''
    else concat(
      $pad, 'order {&#10;',
      string-join(
        for $i in 1 to (count($tokens) - 1)
        return concat($pad, '  ', string($tokens[$i]/TOKEN_REF),
                      ' before ', string($tokens[$i + 1]/TOKEN_REF), ';'),
        '&#10;'
      ),
      '&#10;', $pad, '}&#10;'
    )
};

let $grammar := (/grammarSpec)[1]
let $name := string($grammar/grammarDecl/identifier)
let $lexer := $grammar/rules/ruleSpec/lexerRuleSpec
let $parser := $grammar/rules/ruleSpec/parserRuleSpec
let $modes := $grammar/modeSpec
return concat(
  'grammar ', $name, ' {&#10;',
  local:render-rules($grammar/prequelConstruct, '  '),
  if ($lexer or $modes) then
    concat(
      '  lexer {&#10;',
      local:render-rules($lexer, '    '),
      local:priority($lexer, '    '),
      string-join(
        for $mode in $modes
        return concat(
          '    mode ', string($mode/identifier), ' {&#10;',
          local:render-rules($mode/lexerRuleSpec, '      '),
          local:priority($mode/lexerRuleSpec, '      '),
          '    }&#10;'
        ),
        ''
      ),
      '  }&#10;'
    )
  else '',
  if ($parser) then
    concat('  parser {&#10;', local:render-rules($parser, '    '), '  }&#10;')
  else '',
  '}'
)
