Start ::= '[' (Word (',' Word)*)? ']' End

<?TOKENS?>
Word ::= [a-z]+
End ::= $
