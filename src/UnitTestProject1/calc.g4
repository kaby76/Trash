// Combined Antlr4 grammar generated by Antlrvsix.
// Input grammar: C:\Users\kenne\Documents\AntlrVSIX2\UnitTestProject1\bin\Debug\netcoreapp3.1/../../../../UnitTestProject1/calc.y
// Date: 8/1/2020 7:49:49 AM

grammar calc;
input  :
  | input line
  ;
line  : '\n'
  | exp '\n'
  ;
exp  : NUM
  | exp '+' exp
  | exp '-' exp
  | exp '*' exp
  | exp '/' exp
  | '-' exp NEG
  | exp '^' exp
  | '(' exp ')'
  ;