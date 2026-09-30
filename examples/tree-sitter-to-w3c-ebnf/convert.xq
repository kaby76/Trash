(: Convert the ANTLR JSON parse tree for a Tree-sitter grammar.json to W3C EBNF.
   The TS_PATTERN and TS_UNSUPPORTED symbols deliberately remain undefined. :)

declare function local:property($obj, $key) {
  ($obj/pair[string(STRING) = concat('"', $key, '"')]/value)[1]
};

declare function local:unquote($value) {
  let $s := string(($value/STRING, $value)[1])
  return substring($s, 2, string-length($s) - 2)
};

declare function local:name($s) {
  concat('r_', fn:replace($s, '[^A-Za-z0-9_.-]', '_'))
};

declare function local:members($obj, $separator) {
  string-join(
    (for $member in local:property($obj, 'members')/arr/value
     return local:emit($member/obj)),
    $separator)
};

declare function local:emit($obj) {
  let $kind := local:unquote(local:property($obj, 'type'))
  return
    if ($kind = 'SYMBOL') then
      local:name(local:unquote(local:property($obj, 'name')))
    else if ($kind = 'STRING') then
      let $raw := string(local:property($obj, 'value')/STRING)
      return
        if (contains($raw, '\')) then 'TS_UNSUPPORTED'
        else $raw
    else if ($kind = 'PATTERN') then 'TS_PATTERN'
    else if ($kind = 'BLANK') then '/* empty */'
    else if ($kind = 'SEQ') then
      concat('(', local:members($obj, ' '), ')')
    else if ($kind = 'CHOICE') then
      concat('(', local:members($obj, ' | '), ')')
    else if ($kind = 'REPEAT') then
      concat('(', local:emit(local:property($obj, 'content')/obj), ')*')
    else if ($kind = 'REPEAT1') then
      concat('(', local:emit(local:property($obj, 'content')/obj), ')+')
    else if ($kind = ('FIELD', 'ALIAS', 'TOKEN', 'IMMEDIATE_TOKEN',
                      'PREC', 'PREC_LEFT', 'PREC_RIGHT', 'PREC_DYNAMIC')) then
      local:emit(local:property($obj, 'content')/obj)
    else 'TS_UNSUPPORTED'
};

let $root := (/json/value/obj)[1]
let $rules := local:property($root, 'rules')/obj
return concat(
  '/* Generated from Tree-sitter grammar.json. See README for lossy constructs. */', '&#10;',
  string-join(
    (for $rule in $rules/pair
     return concat(
       local:name(local:unquote($rule/STRING)),
       ' ::= ',
       local:emit($rule/value/obj))),
    '&#10;'))
