(: sets-to-lexer.xq — Pass 4
   Extract inline character-set expressions to named Antlr4 lexer rules.

   In Antlr4, character classes ([...] or ~[...]) may only appear in lexer
   rules, not in parser rules.  For each charset node in the grammar this pass:
     1. Captures the current (already-normalised after Passes 1-3) text of
        the charset expression.
     2. Replaces the inline charset with a generated lexer token name SET_N.
     3. Appends a new lexer rule  SET_N : <charset> ;  at the end of the
        grammar, in document order.

   "insert as last into" is used for the append so that each new rule is
   inserted after the previous one, preserving SET_1 … SET_N order.
   A WS attribute carrying a newline is inserted immediately before each rule
   text so that every generated rule appears on its own line in the output.
:)
(for $c at $pos in //charset
 let $name := concat("SET_", string($pos))
 let $text := string($c)
 let $ws   := string(($c//@WS)[1])
 return (
   replace value of node $c with concat($ws, $name),
   insert node attribute WS {"&#10;"} as last into (//ixml)[1],
   insert node concat($name, " : ", $text, ";") as last into (//ixml)[1]
 ))
