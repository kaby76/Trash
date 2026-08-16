# Generated from trgen 2.3.0
set -e

if [[ -f Test.csproj ]]
then
    mv Test.csproj javacc.csproj
fi

if [ -f transformGrammar.py ]; then python3 transformGrammar.py ; fi

version=4.13.1

antlr4 -v $version -encoding utf-8 -Dlanguage=CSharp   Javacc.g4
antlr4 -v $version -encoding utf-8 -Dlanguage=CSharp   Javacc.g4


dotnet restore javacc.csproj
dotnet build javacc.csproj

exit 0
