(: Update the parser tree with the lexer rules bound by trcombine. :)
declare variable $combined-name external;
declare variable $lexer-rules external;

(
  replace value of node /grammarSpec/grammarDecl/grammarType with "grammar ",
  replace value of node /grammarSpec/grammarDecl/identifier with $combined-name,
  for $prequel in /grammarSpec/prequelConstruct[optionsSpec/option[identifier = "tokenVocab"]]
  return
    if (count($prequel/optionsSpec/option) = 1) then
      delete node $prequel
    else
      for $option in $prequel/optionsSpec/option[identifier = "tokenVocab"]
      return (
        delete node $option/following-sibling::SEMI[1],
        delete node $option
      ),
  insert node $lexer-rules after /grammarSpec/rules
)
