# Generated from trgen 2.3.0
set -e

if [[ -f Test.csproj ]]
then
    mv Test.csproj pest.csproj
fi

if [ -f transformGrammar.py ]; then python3 transformGrammar.py ; fi

version=4.13.1

antlr4 -v $version -encoding utf-8 -Dlanguage=CSharp   PestLexer.g4
antlr4 -v $version -encoding utf-8 -Dlanguage=CSharp   PestParser.g4


dotnet restore pest.csproj
dotnet build pest.csproj

exit 0
