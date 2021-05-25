clean:
	rm -rf */obj */bin

build:
	rm -rf */obj
	dotnet restore
	dotnet build
