(: Expand ASCII case-insensitive lexer syntax into explicit alternatives.
   Run on an ANTLRv4 lexer grammar with caseInsensitive = true. :)

declare function local:letter($c) {
  fn:matches($c, '^[A-Za-z]$')
};

declare function local:other-case($c) {
  if ($c = fn:upper-case($c)) then fn:lower-case($c)
  else fn:upper-case($c)
};

(: Keep escapes intact. In particular, do not mistake hex digits in a
   Unicode escape or letters in a Unicode property for literal letters. :)
declare function local:escape-length($s, $i) {
  if (fn:substring($s, $i + 1, 1) = 'u') then 6
  else if ((fn:substring($s, $i + 1, 1) = 'p' or
            fn:substring($s, $i + 1, 1) = 'P') and
           fn:substring($s, $i + 2, 1) = '{')
       then fn:string-length(fn:substring-before(fn:substring($s, $i + 3), '}')) + 4
  else 2
};

declare function local:has-letter($s, $i) {
  if ($i > fn:string-length($s)) then false()
  else if (fn:substring($s, $i, 1) = '\')
       then local:has-letter($s, $i + local:escape-length($s, $i))
  else if (local:letter(fn:substring($s, $i, 1))) then true()
  else local:has-letter($s, $i + 1)
};

(: 'Select' becomes [Ss][Ee][Ll][Ee][Cc][Tt]. :)
declare function local:fold-literal-body($s, $i) {
  if ($i > fn:string-length($s)) then ''
  else
    let $c := fn:substring($s, $i, 1)
    return
      if ($c = '\') then
        let $n := local:escape-length($s, $i)
        return fn:concat("'", fn:substring($s, $i, $n), "'",
                         local:fold-literal-body($s, $i + $n))
      else if (local:letter($c))
        then fn:concat('[', $c, local:other-case($c), ']',
                       local:fold-literal-body($s, $i + 1))
      else fn:concat("'", $c, "'", local:fold-literal-body($s, $i + 1))
};

(: Append only the missing ASCII counterparts to a lexer character set.
   [A-F0-9] becomes [A-F0-9a-f], including in negated sets. :)
declare function local:set-counterparts($s, $i) {
  if ($i > fn:string-length($s)) then ''
  else
    let $c := fn:substring($s, $i, 1)
    let $next := fn:substring($s, $i + 2, 1)
    return
      if ($c = '\')
        then local:set-counterparts($s, $i + local:escape-length($s, $i))
      else if (local:letter($c) and
               fn:substring($s, $i + 1, 1) = '-' and
               local:letter($next) and
               ($c = fn:upper-case($c)) = ($next = fn:upper-case($next)))
        then fn:concat(local:other-case($c), '-', local:other-case($next),
                       local:set-counterparts($s, $i + 3))
      else if (local:letter($c))
        then fn:concat(local:other-case($c), local:set-counterparts($s, $i + 1))
      else local:set-counterparts($s, $i + 1)
};

let $grammar := (/grammarSpec)[1]
let $options := $grammar/prequelConstruct/optionsSpec
let $case-option := $options/option[
  identifier/RULE_REF/text() = 'caseInsensitive' and
  optionValue/identifier/RULE_REF/text() = 'true'
]
return (
  for $range in $grammar//lexerRuleSpec//characterRange
  let $first := string($range/STRING_LITERAL[1])
  let $last := string($range/STRING_LITERAL[2])
  let $a := fn:substring($first, 2, 1)
  let $z := fn:substring($last, 2, 1)
  where fn:exists($case-option) and
        fn:string-length($first) = 3 and fn:string-length($last) = 3 and
        local:letter($a) and local:letter($z) and
        ($a = fn:upper-case($a)) = ($z = fn:upper-case($z))
  return replace value of node $range with
    fn:concat('[', $a, '-', $z, local:other-case($a), '-', local:other-case($z), ']'),

  for $literal in $grammar//lexerRuleSpec//STRING_LITERAL[not(parent::characterRange)]
  let $raw := string($literal)
  let $body := fn:substring($raw, 2, fn:string-length($raw) - 2)
  where fn:exists($case-option) and local:has-letter($body, 1)
  return replace value of node $literal with local:fold-literal-body($body, 1),

  for $set in $grammar//lexerRuleSpec//LEXER_CHAR_SET
  let $raw := string($set)
  let $body := fn:substring($raw, 2, fn:string-length($raw) - 2)
  let $extra := local:set-counterparts($body, 1)
  where fn:exists($case-option) and $extra != ''
  return replace value of node $set with fn:concat('[', $body, $extra, ']'),

  if (fn:exists($case-option) and fn:count($options/option) = fn:count($case-option))
    then delete node $grammar/prequelConstruct[optionsSpec/option[identifier/RULE_REF/text() = 'caseInsensitive']]
    else (
      delete node $case-option/following-sibling::SEMI[1],
      delete node $case-option
    )
)
