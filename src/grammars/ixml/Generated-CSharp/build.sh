# Generated from trgen 2.2.0
set -e

if [[ -f Test.csproj ]]
then
    mv Test.csproj ixml.csproj
fi

if [ -f transformGrammar.py ]; then python3 transformGrammar.py ; fi

version=4.13.1

antlr4 -v $version -encoding utf-8 -Dlanguage=CSharp   ixmlLexer.g4
antlr4 -v $version -encoding utf-8 -Dlanguage=CSharp   ixmlParser.g4


dotnet restore ixml.csproj
dotnet build ixml.csproj

exit 0
