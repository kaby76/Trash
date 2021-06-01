rm -rf Generated/
trgen
dotnet restore Generated/Test.csproj
dotnet build Generated/Test.csproj
trparse -i "1+2" | trst > st
diff st Gold/
