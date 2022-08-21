
build:
	dotnet restore --ignore-failed-sources
	dotnet build

bumpbase:
	sed 's/"Antlr4.Runtime.Standard" Version="[0-9][.][0-9][.][0-9]"/"Antlr4.Runtime.Standard" Version="4.10.1"/' AntlrTreeEditing.csproj > tem.tem; mv tem.tem AntlrTreeEditing.csproj

innuget:
	dotnet nuget add source 'c:\Users\Kenne\Documents\GitHub\AntlrTreeEditing\bin\Debug\' --name nuget-tree

publish:
	dotnet nuget push bin/Debug/AntlrTreeEditing.5.2.0.nupkg --api-key ${trashkey} --source https://api.nuget.org/v3/index.json

clean:
	rm -rf obj bin
	rm -rf */obj */bin
	rm -rf ${USERPROFILE}/.nuget/packages/AntlrTreeEditing
	rm -f bin/Debug/*.nupkg bin/Debug/*.snupkg

