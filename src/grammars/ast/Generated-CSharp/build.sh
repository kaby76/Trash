# Generated from trgen 4.4.0
set -e

if [[ -f Test.csproj ]]
then
    mv Test.csproj ast.csproj
fi

if [ -f transformGrammar.py ]; then python3 transformGrammar.py ; fi

version=4.13.1

antlr4 -v $version -encoding utf-8 -Dlanguage=CSharp   AstMapping.g4

dotnet restore ast.csproj
dotnet build ast.csproj

exit 0
