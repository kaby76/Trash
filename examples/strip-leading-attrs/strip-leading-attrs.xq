(: Strip hidden-channel token attributes that precede the first real token
   (LEXER or PARSER) inside grammarType.

   Background: the Antlr4 parser stores all tokens — including off-channel
   ones such as block comments, line comments, and whitespace — as attributes
   on the enclosing rule context.  In grammarDecl/grammarType those attributes
   accumulate before the 'lexer'/'parser' keyword.  Because XDM attributes
   have no document-order position relative to child elements, they cannot be
   filtered with following-sibling::LEXER.  Instead, we reconstruct the
   grammarDecl element from scratch:

   • grammarType  — rebuilt, keeping only child nodes that do NOT precede
                    LEXER/PARSER (i.e. the keyword itself and everything after)
   • all other children of grammarDecl (identifier, SEMI, …) — passed through
     unchanged

   The result is a valid grammarDecl subtree with the leading noise removed.
:)

for $g in //grammarDecl
let $gt := $g/grammarType
return element {node-name($g)} {
  $g/@*,
  element {node-name($gt)} {
    for $n in $gt/node()
    return
      if ($n/following-sibling::LEXER or $n/following-sibling::PARSER) then ()
      else $n
  },
  for $c in $g/node()[not(self::grammarType)]
  return $c
}
