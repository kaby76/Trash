(: ungroup-rule.xq
   Expand the first plain grouped alternative in a named parser rule.

   Pattern:   a : (X | Y) B C | D ;
   Result:    a : X B C | Y B C | D ;

   Each inner alternative of the group becomes a separate top-level alternative,
   with the elements before and after the group prepended / appended.

   External parameter
   ------------------
   $ruleName  — name of the parser rule to transform (e.g. 'a')

   Usage
   -----
   dotnet trash parse A.g4 | dotnet trash xquery --query ungroup-rule.xq 'a'

   Only the first plain group (no *, +, ? suffix) in the rule is expanded per
   invocation.  Run multiple times to expand further groups.

   Convention for external variables
   -----------------------------------
   Use standard XQuery:  declare variable $name external;
   Pass values as positional arguments after --query <file>.
   They are bound in declaration order.
:)
declare variable $ruleName external;

for $rule in //parserRuleSpec[RULE_REF/text() = $ruleName]

(: All top-level alternatives of the rule :)
let $alts   := $rule/ruleBlock/ruleAltList/labeledAlt/alternative

(: The first alternative that contains a plain group — ebnf with no *, +, ? suffix :)
let $target := ($alts[*[name()="element"]/ebnf[not(ebnfSuffix)]])[1]

(: All element children of that alternative, in order :)
let $elems  := $target/*[name()="element"]

(: 1-based position of the first grouped element :)
let $gPos   := (for $i in 1 to count($elems)
                return if ($elems[$i]/ebnf[not(ebnfSuffix)]) then $i else ())[1]

(: Inner alternatives of the group :)
let $gAlts  := $elems[$gPos]/ebnf/block/altList/alternative

(: Text of elements before and after the group :)
let $before := string-join(
                 for $e in $elems[position() < $gPos]
                 return normalize-space(string($e)), ' ')
let $after  := string-join(
                 for $e in $elems[position() > $gPos]
                 return normalize-space(string($e)), ' ')

(: Alternatives that have no plain group — kept verbatim :)
let $others := for $a in $alts[not(*[name()="element"]/ebnf[not(ebnfSuffix)])]
               return string-join(
                 for $e in $a/*[name()="element"]
                 return normalize-space(string($e)), ' ')

(: Expand: one alternative per inner alternative, with before/after wrapped around it :)
let $expanded := for $ga in $gAlts
                 let $mid := string-join(
                   for $e in $ga/*[name()="element"]
                   return normalize-space(string($e)), ' ')
                 return string-join(($before[. != ''], $mid, $after[. != '']), ' ')

let $new := string-join(($expanded, $others), ' | ')
return replace value of node $rule/ruleBlock with $new
