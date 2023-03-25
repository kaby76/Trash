
build:
	dotnet restore --ignore-failed-sources
	dotnet build

bumpbase:
	sed 's/"Antlr4.Runtime.Standard" Version="[0-9][.][0-9][.][0-9]"/"Antlr4.Runtime.Standard" Version="4.10.1"/' ParseTreeEditing.csproj > tem.tem; mv tem.tem ParseTreeEditing.csproj

innuget:
	dotnet nuget add source 'c:\Users\Kenne\Documents\GitHub\ParseTreeEditing\bin\Debug\' --name nuget-tree

publish:
	dotnet nuget push bin/Debug/ParseTreeEditing.5.2.0.nupkg --api-key ${trashkey} --source https://api.nuget.org/v3/index.json

clean:
	rm -rf obj bin
	rm -rf */obj */bin
	rm -rf ${USERPROFILE}/.nuget/packages/ParseTreeEditing
	rm -f bin/Debug/*.nupkg bin/Debug/*.snupkg

