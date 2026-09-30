# Generated from trgen 2.3.0
set -e

if [[ -f Test.csproj ]]
then
    mv Test.csproj g4x.csproj
fi

if [ -f transformGrammar.py ]; then python3 transformGrammar.py ; fi

version=4.13.1

antlr4 -v $version -encoding utf-8 -Dlanguage=CSharp   G4XLexer.g4
antlr4 -v $version -encoding utf-8 -Dlanguage=CSharp -visitor  G4XParser.g4


dotnet restore g4x.csproj
dotnet build g4x.csproj

exit 0
