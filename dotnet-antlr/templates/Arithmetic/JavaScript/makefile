
# Makefile for Arithmetic.g4
JAR = C:/Users/kenne/Downloads/antlr-4.9.1-complete.jar
CLASSPATH = $(JAR)\;.
.SUFFIXES: .g4 .js
ANTLRGRAMMARS ?= $(wildcard *.g4)
GENERATED = ArithmeticLexer.js ArithmeticParser.js
SOURCES = $(GENERATED) \
    index.js
default: classes
classes: $(SOURCES)
	npm install
clean:
	rm -rf node_modules
	rm -f package-lock.json
	rm -f *.interp
	rm -f *.tokens
	rm -f $(GENERATED)
run:
	node index.js $(RUNARGS)
ArithmeticLexer.js : Arithmetic.g4
	java -jar $(JAR) -Dlanguage=JavaScript  $<
ArithmeticParser.js : Arithmetic.g4
	java -jar $(JAR) -Dlanguage=JavaScript  $<

