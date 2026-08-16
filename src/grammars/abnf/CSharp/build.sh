# Generated from trgen 2.3.0
set -e

if [[ -f Test.csproj ]]
then
    mv Test.csproj abnf.csproj
fi

if [ -f transformGrammar.py ]; then python3 transformGrammar.py ; fi

version=4.13.1

antlr4 -visitor -v $version -encoding utf-8 -Dlanguage=CSharp   Abnf.g4

dotnet restore abnf.csproj
dotnet build abnf.csproj

exit 0
