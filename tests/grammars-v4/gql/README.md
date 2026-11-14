# GQL interpreter regression

The grammar and seven sample queries were copied from
[grammars-v4/gql](https://github.com/antlr/grammars-v4/tree/master/gql).
The regression covers case-insensitive Unicode identifier rules and checks
that AllStar keeps keywords, spaces, and punctuation as separate tokens.
