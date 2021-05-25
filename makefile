clean:
	rm -rf */obj */bin

build:
	dotnet restore
	dotnet build
