# Generated from trgen 0.23.23
set -e
if [ -f transformGrammar.py ]; then python3 transformGrammar.py ; fi

version=`dotnet trxml2 Other.csproj | fgrep 'PackageReference/@Version' | awk -F= '{print $2}'`

antlr4 -v $version -encoding utf-8 -Dlanguage=CSharp -visitor -listener   ANTLRv4Lexer.g4
antlr4 -v $version -encoding utf-8 -Dlanguage=CSharp -visitor -listener   ANTLRv4Parser.g4


dotnet restore antlr4.csproj
dotnet build antlr4.csproj

exit 0
