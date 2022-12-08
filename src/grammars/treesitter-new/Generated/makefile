# Generated code from trgen 0.13.8
GENERATED = JSON5Parser.cs JSON5Lexer.cs 
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
