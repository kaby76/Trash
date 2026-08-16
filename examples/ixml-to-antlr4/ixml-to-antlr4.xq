(: ixml-to-antlr4.xq — Pass 1
   Core structural syntax transforms from iXML to Antlr4.

   iXML rule form:  [mark] name [':'|'='] alts '.'
   Antlr4 rule:      name ':' alts ';'
:)

(: Step 1: Remove rule-head marks ('@', '^', '-').
   iXML marks control output serialisation; they have no Antlr4 equivalent. :)
(for $m in //rule_/mark/(AT | CARET | MINUS)
 return replace value of node $m with ""),

(: Step 2: Remove marks from nonterminal references. :)
(for $m in //nonterminal/mark/(AT | CARET | MINUS)
 return replace value of node $m with ""),

(: Step 3: Remove tmarks ('-', '^') from terminal literals and character sets. :)
(for $m in //tmark/(CARET | MINUS)
 return replace value of node $m with ""),

(: Step 4: Remove the iXML sequence-separator ','.
   iXML sequences items with ','; Antlr4 juxtaposes them. :)
(for $c in //alt/COMMA
 return replace value of node $c with ""),

(: Step 5: Replace iXML alternative separators (';' or '|') with Antlr4 '|'.
   Only targets alts-level separators, not those inside character sets. :)
(for $sep in //alts/ALT_SEP
 return replace value of node $sep with " |"),

(: Step 6: Normalise the assignment operator: iXML accepts '=' as well as ':';
   Antlr4 uses ':' only. :)
(for $a in //rule_/ASSIGN[. = "="]
 return replace value of node $a with ":"),

(: Step 7: Replace the rule-terminating '.' with Antlr4 ';'. :)
(for $d in //rule_/DOT | //version/DOT
 return replace value of node $d with ";"),

(: Step 8: Convert iXML comments '{...}' to Antlr4 block-comment '/* ... */'. :)
(for $c in //COMMENT
 let $inner := substring(string($c), 2, string-length(string($c)) - 2)
 return replace value of node $c with concat("/* ", $inner, " */")),

(: Step 9: Convert double-quoted iXML string literals to Antlr4 single-quoted.
   e.g., "A" -> 'A', "+" -> '+', "ixml" -> 'ixml'.
   Handles embedded double-quotes (iXML escapes them as "") and embedded
   single-quotes (Antlr4 escapes them as \').
   Single-quoted iXML strings are already valid Antlr4 and are left unchanged. :)
(for $s in //string_/DQUOTE_STRING
 let $inner     := substring(string($s), 2, string-length(string($s)) - 2)
 let $unescaped := fn:replace($inner, '""', '"')
 let $escaped   := fn:replace($unescaped, "'", "\\'")
 let $new       := concat("'", $escaped, "'")
 return replace value of node $s with $new)
