parser grammar XPath3MetaParser;

options {
    tokenVocab = XPath3MetaLexer;
}

grammar_: production* EOF;
production: SYMBOL '::=' choice;
choice: sequence_or_difference ( '|' sequence_or_difference)*;
sequence_or_difference: ( item ( '-' item | item*))?;
item: primary ( '?' | '*' | '+')*;
primary: SYMBOL | STRING | SET | '(' choice ')' | CONSTRAINT;