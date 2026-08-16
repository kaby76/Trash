# Generated from trgen 2.3.0
set -e

if [[ -f Test.csproj ]]
then
    mv Test.csproj antlr4.csproj
fi

if [ -f transformGrammar.py ]; then python3 transformGrammar.py ; fi

version=4.13.1

antlr4 -v $version -encoding utf-8 -Dlanguage=CSharp   ANTLRv4Lexer.g4
antlr4 -v $version -encoding utf-8 -Dlanguage=CSharp   ANTLRv4Parser.g4


dotnet restore antlr4.csproj
dotnet build antlr4.csproj

exit 0
