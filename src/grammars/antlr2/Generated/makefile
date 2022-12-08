# Generated code from trgen 0.18.0
GENERATED = ANTLRv2Lexer.cs ANTLRv2Parser.cs 
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
