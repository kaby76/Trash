# Generated code from trgen 0.8.9
GENERATED = ArithmeticLexer.cs ArithmeticParser.cs 
default:
	dotnet restore
	dotnet build
run:
	trwdog dotnet run $(RUNARGS)
clean:
	dotnet clean
	rm -rf bin obj
test:
	bash test.sh
