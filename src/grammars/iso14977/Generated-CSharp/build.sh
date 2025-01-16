# Generated from trgen 0.23.12
set -e
if [ -f transformGrammar.py ]; then python3 transformGrammar.py ; fi

version=`dotnet trxml2 Other.csproj | fgrep 'PackageReference/@Version' | awk -F= '{print $2}'`

antlr4 -v $version -encoding utf-8 -Dlanguage=CSharp   Iso14977Lexer.g4
antlr4 -v $version -encoding utf-8 -Dlanguage=CSharp   Iso14977Parser.g4


dotnet restore iso14977.csproj
dotnet build iso14977.csproj

exit 0
