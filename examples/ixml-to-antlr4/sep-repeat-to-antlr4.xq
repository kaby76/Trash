(: sep-repeat-to-antlr4.xq — Pass 4
   Expand iXML separator-quantifiers to standard Antlr4 repetition forms.

   iXML   f ++ sep   (one-or-more with separator)  ->  Antlr4: f (sep f)*
   iXML   f ** sep   (zero-or-more with separator)  ->  Antlr4: (f (sep f)*)?

   NOTE: The expansion uses string() to capture the factor and separator
   text after prior passes.  It works reliably for simple nonterminal
   separators (e.g. rule++RS).  Complex separators such as (-",", s)
   will produce approximate output that needs manual review.
:)

(: Expand one-or-more with separator: f ++ sep  ->  f (sep f)* :)
(for $r in //repeat1[DPLUS]
 let $f := string($r/factor)
 let $s := string($r/sep/factor)
 return (
   replace value of node $r/DPLUS with concat(" (", $s, " ", $f, ")*"),
   delete node $r/sep
 )),

(: Expand zero-or-more with separator: f ** sep  ->  (f (sep f)*)? :)
(for $r in //repeat0[DSTAR]
 let $f := string($r/factor)
 let $s := string($r/sep/factor)
 return (
   insert node "(" before $r/factor,
   replace value of node $r/DSTAR with concat(" (", $s, " ", $f, ")*)?"),
   delete node $r/sep
 ))
