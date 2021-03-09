# Generated code from Antlr4BuildTasks.dotnet-antlr v <version>
# Makefile for Arithmetic.g4
JAR = C:/Users/kenne/Downloads/antlr-4.9.1-complete.jar
CLASSPATH = $(JAR)<if(path_sep_semi)>\;<else>:<endif>.
.SUFFIXES: .g4 .java .class
.java.class:
	javac -cp $(CLASSPATH) $*.java
ANTLRGRAMMARS ?= $(wildcard *.g4)
GENERATED = ArithmeticLexer.java ArithmeticParser.java
SOURCES = $(GENERATED) \
    Program.java \
    ErrorListener.java
default: classes
classes: $(GENERATED) $(SOURCES:.java=.class)
clean:
	rm -f *.class
	rm -f *.interp
	rm -f *.tokens
	rm -f $(GENERATED)
run:
	java -classpath $(CLASSPATH) Program $(RUNARGS)
ArithmeticLexer.java : Arithmetic.g4
	java -jar $(JAR) $\<
ArithmeticParser.java : Arithmetic.g4
	java -jar $(JAR) $\<

