(: Step 1: Add ';' after each parser rule definition. :)
(for $e in //rule_/expansions return insert node " ;" after $e),

(: Step 2: Add ';' after each lexer token definition. :)
(for $e in //*[name() = "token"]/expansions return insert node " ;" after $e),

(: Step 3: Strip the Lark inline/transparent marker '?' from rule names. :)
(for $r in //rule_/RULE[starts-with(., "?")]
 return replace value of node $r with substring(string($r), 2)),

(: Step 4: Convert string literals from double-quotes to single-quotes,
   escaping any embedded single-quotes as \' for Antlr4. :)
(for $s in //STRING
 let $inner := substring(string($s), 2, string-length(string($s)) - 2)
 let $escaped := fn:replace($inner, "'", "\'")
 let $new := concat("'", $escaped, "'")
 return replace value of node $s with $new)
