(: A combined ANTLR4 grammar cannot declare custom channels. Map every
   channel(NAME) lexer command to the built-in HIDDEN channel, then remove
   the channels declaration. This intentionally merges the custom channels. :)
(
  for $command in //lexerCommand[lexerCommandName/identifier/RULE_REF/text() = "channel"]
  return replace value of node $command/lexerCommandExpr/identifier/TOKEN_REF with "HIDDEN",
  delete node /grammarSpec/prequelConstruct[channelsSpec]
)
