# Elixir regression

The grammar and `examples/block.ex` come from grammars-v4/elixir. The CSharp
target checks the native parser in the grammars-v4 suite. AllStarParserTests
also regenerates interpreter tables and parses the same input, covering the
lexer rules that use Unicode properties such as `\p{L}` and `\p{Ll}`.
