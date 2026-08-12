(: Step 6: Convert %ignore TERMINAL statements to Antlr4 '-> skip' lexer commands.
   For each %ignore X, find the token rule X and change its trailing ' ;' to ' -> skip ;'.
   Input: output of import-common.xq (common token rules already inlined). :)

(: Step 6a: Replace the trailing ';' with '-> skip ;' on each ignored token rule. :)
(for $name in //item[statement[IGNORE]]/statement/expansions//TOKEN
 let $tok := //*[name()="token"][TOKEN = $name]
 let $semi := $tok/expansions/following-sibling::node()[1]
 return replace value of node $semi with " -> skip ;"),

(: Step 6b: Delete all %ignore items. :)
(delete node //item[statement[IGNORE]])
