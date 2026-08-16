# Generated from trgen 2.3.0
set -e

if [[ -f Test.csproj ]]
then
    mv Test.csproj iso14977.csproj
fi

if [ -f transformGrammar.py ]; then python3 transformGrammar.py ; fi

version=4.13.1

antlr4 -v $version -encoding utf-8 -Dlanguage=CSharp   Iso14977Lexer.g4
antlr4 -v $version -encoding utf-8 -Dlanguage=CSharp   Iso14977Parser.g4


dotnet restore iso14977.csproj
dotnet build iso14977.csproj

exit 0
