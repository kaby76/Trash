
# Generated code from Antlr4BuildTasks.dotnet-antlr v 2.2
# Makefile for Arithmetic.g4
JAR = C:/Users/kenne/Downloads/antlr-4.9.1-complete.jar
CLASSPATH = $(JAR)\;.
.SUFFIXES: .g4 .py
ANTLRGRAMMARS ?= $(wildcard *.g4)
GENERATED = ArithmeticLexer.py ArithmeticParser.py
SOURCES = $(GENERATED) \
    Program.py
default: classes
classes: $(SOURCES)
	pip install antlr4-python3-runtime
	pip install readchar
clean:
	rm -f *.tokens *.interp
	rm -f *.interp
	rm -f *.tokens
	rm -f $(GENERATED)
run:
	python Program.py $(RUNARGS)
ArithmeticLexer.py : Arithmetic.g4
	java -jar $(JAR) -Dlanguage=Python3  $<
ArithmeticParser.py : Arithmetic.g4
	java -jar $(JAR) -Dlanguage=Python3  $<

