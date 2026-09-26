Start ::= Expr End
Expr ::= Expr '+' Term | Term
Term ::= Number ('*' Number)*

<?TOKENS?>
Number ::= Digit+
Digit ::= [0-9]
End ::= $
