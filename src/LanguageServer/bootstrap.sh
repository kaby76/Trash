curl https://repo1.maven.org/maven2/org/antlr/antlr4/4.10.1/antlr4-4.10.1-complete.jar -o ./antlr4-4.10.1-complete.jar
java -jar antlr4-4.10.1-complete.jar -package LanguageServer -Dlanguage=CSharp -visitor -listener ANTLRv2Lexer.g4 ANTLRv2Parser.g4
java -jar antlr4-4.10.1-complete.jar -package LanguageServer -Dlanguage=CSharp -visitor -listener ANTLRv3Lexer.g4 ANTLRv3Parser.g4
java -jar antlr4-4.10.1-complete.jar -package LanguageServer -Dlanguage=CSharp -visitor -listener ANTLRv4Lexer.g4 ANTLRv4Parser.g4
java -jar antlr4-4.10.1-complete.jar -package LanguageServer -Dlanguage=CSharp -visitor -listener BisonLexer.g4 BisonParser.g4
java -jar antlr4-4.10.1-complete.jar -package LanguageServer -Dlanguage=CSharp -visitor -listener Iso14977Lexer.g4 Iso14977Parser.g4
java -jar antlr4-4.10.1-complete.jar -package LanguageServer -Dlanguage=CSharp -visitor -listener W3CebnfLexer.g4 W3CebnfParser.g4
java -jar antlr4-4.10.1-complete.jar -package LanguageServer -Dlanguage=CSharp -visitor -listener lbnfLexer.g4 lbnfParser.g4
java -jar antlr4-4.10.1-complete.jar -package LanguageServer -Dlanguage=CSharp -visitor -listener LarkLexer.g4 LarkParser.g4
java -jar antlr4-4.10.1-complete.jar -package LanguageServer -Dlanguage=CSharp -visitor -listener PestLexer.g4 PestParser.g4

