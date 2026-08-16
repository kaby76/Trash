# Generated from trgen 2.3.0
set -e

if [[ -f Test.csproj ]]
then
    mv Test.csproj antlr2.csproj
fi

if [ -f transformGrammar.py ]; then python3 transformGrammar.py ; fi

version=4.13.1

antlr4 -v $version -encoding utf-8 -Dlanguage=CSharp   ANTLRv2Lexer.g4
antlr4 -v $version -encoding utf-8 -Dlanguage=CSharp   ANTLRv2Parser.g4


dotnet restore antlr2.csproj
dotnet build antlr2.csproj

exit 0
