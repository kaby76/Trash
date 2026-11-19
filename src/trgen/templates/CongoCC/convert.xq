(: Convert a supported subset of a combined ANTLR4 grammar to CongoCC.
   Unsupported nodes fail explicitly. ANTLR4 prediction/precedence and CongoCC
   SCAN semantics are not translated yet. :)

declare variable $prefix external := '';

declare function local:unsupported($name) {
  fn:error((), fn:concat('CongoCC conversion does not support ', $name))
};

declare function local:escape-quotes($value, $position) {
  if ($position > fn:string-length($value)) then ''
  else
    let $c := fn:substring($value, $position, 1)
    return fn:concat(
      if ($c = '"') then '\"' else $c,
      local:escape-quotes($value, $position + 1))
};

declare function local:quote($value) {
  let $plain := if (fn:string-length($value) = 2 and
    fn:substring($value, 1, 1) = '\' and
    (fn:substring($value, 2, 1) = ']' or
     fn:substring($value, 2, 1) = '[' or
     fn:substring($value, 2, 1) = '-'))
    then fn:substring($value, 2, 1) else $value
  return fn:concat('"', local:escape-quotes($plain, 1), '"')
};

declare function local:literal($node) {
  let $raw := fn:string($node)
  return local:quote(fn:substring($raw, 2, fn:string-length($raw) - 2))
};

declare function local:read-char($body, $position) {
  let $c := fn:substring($body, $position, 1)
  return if ($c = '\') then fn:substring($body, $position, 2) else $c
};
declare function local:class-items($body, $position) {
  if ($position > fn:string-length($body)) then ()
  else
    let $first := local:read-char($body, $position)
    let $next := $position + fn:string-length($first)
    let $is-range := fn:substring($body, $next, 1) = '-'
    let $last := if ($is-range) then local:read-char($body, $next + 1) else ''
    let $end := if ($is-range) then $next + 1 + fn:string-length($last) else $next
    return (
      if ($is-range)
        then fn:concat(local:quote($first), '-', local:quote($last))
        else local:quote($first),
      local:class-items($body, $end)
    )
};

declare function local:char-set($raw) {
  let $body := fn:substring($raw, 2, fn:string-length($raw) - 2)
  return fn:concat('[', fn:string-join(local:class-items($body, 1), ', '), ']')
};

declare function local:suffix($node) {
  if (fn:exists($node/ebnfSuffix/QUESTION)) then '?'
  else if (fn:exists($node/ebnfSuffix/STAR)) then '*'
  else if (fn:exists($node/ebnfSuffix/PLUS)) then '+'
  else ''
};

declare function local:parser-alt-list($list) {
  fn:string-join(
    for $alt in $list/alternative
    return fn:string-join(
      for $element in $alt/element
      return local:parser-element($element), ' '),
    ' | ')
};

declare function local:parser-element($node) {
  let $atom := $node/atom
  let $block := $node/ebnf/block
  let $body :=
    if (fn:exists($atom/ruleref/RULE_REF)) then fn:concat($prefix, fn:string($atom/ruleref/RULE_REF))
    else if (fn:exists($atom/terminalDef/TOKEN_REF))
      then fn:concat('<', fn:string($atom/terminalDef/TOKEN_REF), '>')
    else if (fn:exists($atom/terminalDef/STRING_LITERAL))
      then local:literal($atom/terminalDef/STRING_LITERAL)
    else if (fn:exists($block))
      then fn:concat('(', local:parser-alt-list($block/altList), ')',
        local:suffix($node/ebnf/blockSuffix))
    else local:unsupported('parser element')
  return if (local:suffix($node) = '') then $body
    else fn:concat('(', $body, ')', local:suffix($node))
};

declare function local:parser-rule($rule, $nl) {
  let $name := fn:string($rule/RULE_REF)
  let $alts := $rule/ruleBlock/ruleAltList/labeledAlt
  let $recursive := $alts[alternative/element[1]/atom/ruleref/RULE_REF = $name]
  let $base := $alts[not(alternative/element[1]/atom/ruleref/RULE_REF = $name)]
  let $body :=
    if (fn:empty($recursive))
      then fn:string-join(
        for $alt in $alts
        return local:parser-alt-list($alt),
        fn:concat($nl, '  | '))
    else if (fn:empty($base)) then local:unsupported('a directly left-recursive rule without a base alternative')
    else
      let $base-body := fn:string-join(
        for $alt in $base
        return local:parser-alt-list($alt), ' | ')
      let $tails := fn:string-join(
        for $alt in $recursive
        return fn:string-join(
          for $element in $alt/alternative/element[position() > 1]
          return local:parser-element($element), ' '), ' | ')
      return fn:concat('(', $base-body, ') ((', $tails, '))*')
  return fn:concat($prefix, $name, ' : ', $body, ' ;', $nl)
};
declare function local:lexer-alt-list($list) {
  fn:string-join(
    for $alt in $list/lexerAlt
    return fn:string-join(
      for $element in $alt/lexerElements/lexerElement
      return local:lexer-element($element), ' '),
    ' | ')
};

declare function local:set-element($node) {
  let $range := $node/characterRange/STRING_LITERAL
  return if (fn:exists($node/STRING_LITERAL))
    then local:literal($node/STRING_LITERAL)
    else if (fn:count($range) = 2)
      then fn:concat(local:literal($range[1]), '-', local:literal($range[2]))
    else local:unsupported('negated set element')
};

declare function local:not-set($node) {
  let $members := ($node/blockSet/setElement, $node/setElement)
  return fn:concat('~[', fn:string-join(
    for $member in $members return local:set-element($member), ', '), ']')
};
declare function local:lexer-element($node) {
  let $atom := $node/lexerAtom
  let $block := $node/lexerBlock
  let $range := $atom/characterRange/STRING_LITERAL
  let $body :=
    if (fn:exists($atom/LEXER_CHAR_SET)) then local:char-set(fn:string($atom/LEXER_CHAR_SET))
    else if (fn:exists($atom/terminalDef/STRING_LITERAL))
      then local:literal($atom/terminalDef/STRING_LITERAL)
    else if (fn:exists($atom/terminalDef/TOKEN_REF))
      then fn:concat('<', fn:string($atom/terminalDef/TOKEN_REF), '>')
    else if (fn:count($range) = 2)
      then fn:concat('[', local:literal($range[1]), '-', local:literal($range[2]), ']')
    else if (fn:exists($atom/notSet)) then local:not-set($atom/notSet)
    else if (fn:exists($block))
      then fn:concat('(', local:lexer-alt-list($block/lexerAltList), ')')
    else local:unsupported('lexer element')
  return if (local:suffix($node) = '') then $body
    else fn:concat('(', $body, ')', local:suffix($node))
};

declare function local:lexer-rule($rule, $nl) {
  let $alts := $rule/lexerRuleBlock/lexerAltList/lexerAlt
  let $name := fn:string($rule/TOKEN_REF)
  let $commands := $alts/lexerCommands/lexerCommand
  let $skip := fn:count($commands) = fn:count($alts) and
    fn:count($commands[lexerCommandName/identifier/RULE_REF = 'skip']) = fn:count($alts)
  let $regex := local:lexer-alt-list($rule/lexerRuleBlock/lexerAltList)
  return if (fn:exists($commands) and not($skip))
    then local:unsupported('lexer commands other than skip')
    else if (fn:exists($rule/FRAGMENT))
      then fn:concat('TOKEN : <#', $name, ' : ', $regex, '> ;', $nl)
    else if ($skip)
      then fn:concat('SKIP : <', $name, ' : ', $regex, '> ;', $nl)
    else fn:concat('TOKEN : <', $name, ' : ', $regex, '> ;', $nl)
};

let $grammar := /grammarSpec
let $name := fn:string($grammar/grammarDecl/identifier/*[1])
let $nl := '
'
return if (fn:count($grammar) != 1 or
  fn:exists($grammar/grammarDecl/grammarType/LEXER) or
  fn:exists($grammar/grammarDecl/grammarType/PARSER))
  then local:unsupported('anything other than one combined ANTLR4 grammar')
  else fn:concat(
    'PARSER_PACKAGE=generated;', $nl,
    'PARSER_CLASS=', $name, 'Parser;', $nl, $nl,
    fn:string-join(
      for $rule in $grammar/rules/ruleSpec/lexerRuleSpec
      return local:lexer-rule($rule, $nl), ''), $nl,
    fn:string-join(
      for $rule in $grammar/rules/ruleSpec/parserRuleSpec
      return local:parser-rule($rule, $nl), $nl)
  )
