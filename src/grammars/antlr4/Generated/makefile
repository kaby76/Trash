# Generated code from trgen 0.18.1
GENERATED = ANTLRv4Lexer.cs ANTLRv4Parser.cs 
default:
	dotnet restore
	dotnet build
run:
	@trwdog dotnet run $(RUNARGS)
clean:
	dotnet clean
	rm -rf bin obj
test:
	bash test.sh
