not(
  $lookahead1 = (
    "IDENTIFIER", "MODULE", "OPEN", "REQUIRES", "EXPORTS", "OPENS",
    "TO", "USES", "PROVIDES", "WHEN", "WITH", "TRANSITIVE",
    "YIELD", "SEALED", "PERMITS", "RECORD", "VAR"
  )
  and $lookahead2 = "ASSIGN"
)
