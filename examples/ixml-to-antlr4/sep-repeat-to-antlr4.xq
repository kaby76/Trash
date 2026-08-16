(: sep-repeat-to-antlr4.xq — Pass 5
   Expand iXML separator-quantifiers to standard Antlr4 repetition forms.

   iXML   f ++ sep   (one-or-more with separator)  ->  Antlr4: f (sep f)*
   iXML   f ** sep   (zero-or-more with separator)  ->  Antlr4: (f (sep f)*)?

   Serialization walks descendant-or-self::* and, for each node, emits
   its @WS attribute (if any) followed by its text content (for leaf
   elements only, to avoid double-counting from parent elements).
   fn:normalize-space() trims any leading/trailing whitespace introduced
   by the walk.  This ensures hidden-channel WS stored as attributes is
   preserved — fixing separators like (-[";|"], s) which previously
   lost the space between SET_N and s.
:)

(: Expand one-or-more with separator: f ++ sep  ->  f (sep f)* :)
(for $r in //repeat1[DPLUS]
 let $f := fn:normalize-space(fn:string-join(
               for $n in $r/factor/descendant-or-self::*
               return concat(string($n/@WS), if ($n/*) then "" else string($n))
           , ""))
 let $s := fn:normalize-space(fn:string-join(
               for $n in $r/sep/factor/descendant-or-self::*
               return concat(string($n/@WS), if ($n/*) then "" else string($n))
           , ""))
 return (
   replace value of node $r/DPLUS with concat(" (", $s, " ", $f, ")*"),
   delete node $r/sep
 )),

(: Expand zero-or-more with separator: f ** sep  ->  (f (sep f)*)? :)
(for $r in //repeat0[DSTAR]
 let $f := fn:normalize-space(fn:string-join(
               for $n in $r/factor/descendant-or-self::*
               return concat(string($n/@WS), if ($n/*) then "" else string($n))
           , ""))
 let $s := fn:normalize-space(fn:string-join(
               for $n in $r/sep/factor/descendant-or-self::*
               return concat(string($n/@WS), if ($n/*) then "" else string($n))
           , ""))
 return (
   insert node "(" before $r/factor,
   replace value of node $r/DSTAR with concat(" (", $s, " ", $f, ")*)?"),
   delete node $r/sep
 ))
