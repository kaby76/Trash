(: encoded-to-antlr4.xq — Pass 2
   Convert iXML encoded character references to Antlr4 unicode escape syntax.

   In a rule body:          #9   ->  '\u0009'
   Inside a character set:  #9   ->  \u0009  (no surrounding quotes needed)
:)

(: Encoded literals in rule bodies: replace '#' with '\u and pad the hex
   digits to four places followed by a closing quote. :)
(for $e in //encoded
 let $h := string($e/hex/HEX_DIGITS)
 let $pad := substring("0000", 1, 4 - string-length($h))
 return (
   replace value of node $e/HASH with "'\u",
   replace value of node $e/hex/HEX_DIGITS with concat($pad, $h, "'")
 )),

(: '#' hex members inside character sets need no surrounding quotes. :)
(for $m in //set_/member[HASH]
 let $h := string($m/hex/HEX_DIGITS)
 let $pad := substring("0000", 1, 4 - string-length($h))
 return (
   replace value of node $m/HASH with "\u",
   replace value of node $m/hex/HEX_DIGITS with concat($pad, $h)
 ))
