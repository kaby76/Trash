(: kleene-lr.xq
   Transform all directly left-recursive parser rules into Kleene-star form.

   Pattern:   a : a SUFFIX | BASE ;
   Result:    a : ( BASE ) ( SUFFIX )* ;

   Reconstruction: each top-level `element` child of `alternative` wraps a
   single atom (token or rule ref).  Its string-value gives the token text;
   joining those with a space gives correct ANTLR4 syntax.
:)
for $rule in //parserRuleSpec[
    RULE_REF/text() = ruleBlock/ruleAltList/labeledAlt/alternative
                          /*[name()="element"][1]/atom/ruleref/*[1]/text()
]
let $name     := string($rule/RULE_REF/text())
let $alts     := $rule/ruleBlock/ruleAltList/labeledAlt/alternative
let $lrAlts   := $alts[*[name()="element"][1]/atom/ruleref/*[1]/text() = $name]
let $baseAlts := $alts[not(*[name()="element"][1]/atom/ruleref/*[1]/text() = $name)]
let $suffixes := for $a in $lrAlts
                 return string-join(
                     for $e in $a/*[name()="element"][position() > 1]
                     return normalize-space(string($e)),
                     ' ')
let $bases    := for $a in $baseAlts
                 return string-join(
                     for $e in $a/*[name()="element"]
                     return normalize-space(string($e)),
                     ' ')
let $new      := concat(
    '( ', string-join($bases, ' | '), ' ) ',
    '( ', string-join($suffixes, ' | '), ' )*')
return replace value of node $rule/ruleBlock with $new
