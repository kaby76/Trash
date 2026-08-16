# Generated from trgen 2.3.0
set -e

if [[ -f Test.csproj ]]
then
    mv Test.csproj antlr3.csproj
fi

if [ -f transformGrammar.py ]; then python3 transformGrammar.py ; fi

version=4.13.1

antlr4 -v $version -encoding utf-8 -Dlanguage=CSharp   ANTLRv3Lexer.g4
antlr4 -v $version -encoding utf-8 -Dlanguage=CSharp   ANTLRv3Parser.g4


dotnet restore antlr3.csproj
dotnet build antlr3.csproj

exit 0
