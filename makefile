build:
	rm -rf */obj
	dotnet restore
	dotnet build

clean:
	rm -rf */obj */bin

