# Generated from trgen 2.3.0
set -e

if [[ -f Test.csproj ]]
then
    mv Test.csproj xtext.csproj
fi

if [ -f transformGrammar.py ]; then python3 transformGrammar.py ; fi

version=4.13.1

antlr4 -v $version -encoding utf-8 -Dlanguage=CSharp   xtext.g4
antlr4 -v $version -encoding utf-8 -Dlanguage=CSharp   xtext.g4


dotnet restore xtext.csproj
dotnet build xtext.csproj

exit 0
