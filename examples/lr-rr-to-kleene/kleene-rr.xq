(: kleene-rr.xq
   Transform all directly right-recursive parser rules into Kleene-star form.

   Pattern:   b : PREFIX b | BASE ;
   Result:    b : ( PREFIX )* ( BASE ) ;

   Reconstruction: each top-level `element` child of `alternative` wraps a
   single atom (token or rule ref).  Its string-value gives the token text;
   joining those with a space gives correct ANTLR4 syntax.
:)
for $rule in //parserRuleSpec[
    RULE_REF/text() = ruleBlock/ruleAltList/labeledAlt/alternative
                          /*[name()="element"][last()]/atom/ruleref/*[1]/text()
]
let $name     := string($rule/RULE_REF/text())
let $alts     := $rule/ruleBlock/ruleAltList/labeledAlt/alternative
let $rrAlts   := $alts[*[name()="element"][last()]/atom/ruleref/*[1]/text() = $name]
let $baseAlts := $alts[not(*[name()="element"][last()]/atom/ruleref/*[1]/text() = $name)]
let $prefixes := for $a in $rrAlts
                 return string-join(
                     for $e in $a/*[name()="element"][position() < last()]
                     return normalize-space(string($e)),
                     ' ')
let $bases    := for $a in $baseAlts
                 return string-join(
                     for $e in $a/*[name()="element"]
                     return normalize-space(string($e)),
                     ' ')
let $new      := concat(
    '( ', string-join($prefixes, ' | '), ' )* ',
    '( ', string-join($bases, ' | '), ' )')
return replace value of node $rule/ruleBlock with $new
