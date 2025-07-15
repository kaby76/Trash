# Generated from trgen 0.23.23
set -e
if [ -f transformGrammar.py ]; then python3 transformGrammar.py ; fi

version=`dotnet trxml2 Other.csproj | fgrep 'PackageReference/@Version' | awk -F= '{print $2}'`

antlr4 -v $version -encoding utf-8 -Dlanguage=CSharp -visitor -listener   PestLexer.g4
antlr4 -v $version -encoding utf-8 -Dlanguage=CSharp -visitor -listener   PestParser.g4


dotnet restore pest.csproj
dotnet build pest.csproj

exit 0
