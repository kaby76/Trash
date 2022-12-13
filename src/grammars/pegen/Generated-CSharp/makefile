# Generated code from trgen 0.19.0
GENERATED = pegen_v3_10Lexer.cs pegen_v3_10Parser.cs 
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
