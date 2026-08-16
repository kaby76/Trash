(: charset-to-antlr4.xq — Pass 3
   Convert iXML character-set member syntax to Antlr4 bracket-class notation.

   iXML  ["a"-"z"]         ->  Antlr4  [a-z]
   iXML  [";|"]            ->  Antlr4  [;|]
   iXML  [L]  (Unicode L)  ->  Antlr4  [\p{L}]
:)

(: Step 1: Strip quotes from string members inside character sets.
   e.g., [";"] -> [;],  ["@^-"] -> [@^-]. :)
(for $s in //set_/member/string_/(DQUOTE_STRING | SQUOTE_STRING)
 let $inner := substring(string($s), 2, string-length(string($s)) - 2)
 return replace value of node $s with $inner),

(: Step 2: Strip quotes from single-character range bounds.
   e.g., ["a"-"z"] -> [a-z],  ['0'-'9'] -> [0-9]. :)
(for $c in //range_/from_/character/(DQUOTE_STRING | SQUOTE_STRING)
 let $inner := substring(string($c), 2, string-length(string($c)) - 2)
 return replace value of node $c with $inner),
(for $c in //range_/to_/character/(DQUOTE_STRING | SQUOTE_STRING)
 let $inner := substring(string($c), 2, string-length(string($c)) - 2)
 return replace value of node $c with $inner),

(: Step 3: Remove the iXML alt-separator between charset members.
   Antlr4 juxtaposes character class members without a separator. :)
(for $sep in //set_/ALT_SEP
 return replace value of node $sep with ""),

(: Step 4: Rewrite Unicode category codes to Antlr4 \p{XX} syntax.
   e.g., [L] -> [\p{L}],  [Zs] -> [\p{Zs}],  [Nd] -> [\p{Nd}]. :)
(for $c in //class_/code/CODE
 return replace value of node $c with concat("\p{", string($c), "}"))
