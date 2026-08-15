# Generated from trgen 2.2.0
set -e

if [[ -f Test.csproj ]]
then
    mv Test.csproj w3cebnf.csproj
fi

if [ -f transformGrammar.py ]; then python3 transformGrammar.py ; fi

version=4.13.1

antlr4 -v $version -encoding utf-8 -Dlanguage=CSharp   W3CebnfLexer.g4
antlr4 -v $version -encoding utf-8 -Dlanguage=CSharp   W3CebnfParser.g4

dotnet restore w3cebnf.csproj
dotnet build w3cebnf.csproj

exit 0
