# Generated code from Antlr4BuildTasks.dotnet-antlr v <version>
# Makefile for Arithmetic.g4
JAR = C:/Users/kenne/Downloads/antlr-4.9.1-complete.jar
CLASSPATH = $(JAR)\;.
.SUFFIXES: .g4 .dart
ANTLRGRAMMARS ?= $(wildcard *.g4)
GENERATED = ArithmeticLexer.dart ArithmeticParser.dart
SOURCES = $(GENERATED) \
    cli.dart
default: classes
classes: $(SOURCES)
	dart pub get
clean:
	rm -f *.tokens *.interp
	rm -f $(GENERATED)
	rm -f pubspec.lock
run:
	dart run cli.dart $(RUNARGS)
ArithmeticLexer.dart : Arithmetic.g4
	java -jar $(JAR) -Dlanguage=Dart  $\<
ArithmeticParser.dart : Arithmetic.g4
	java -jar $(JAR) -Dlanguage=Dart  $\<

