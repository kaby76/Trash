# Generated from trgen 0.23.23
set -e
if [ -f transformGrammar.py ]; then python3 transformGrammar.py ; fi

version=`dotnet trxml2 Other.csproj | fgrep 'PackageReference/@Version' | awk -F= '{print $2}'`

antlr4 -v $version -encoding utf-8 -Dlanguage=CSharp -visitor -listener   W3CebnfLexer.g4
antlr4 -v $version -encoding utf-8 -Dlanguage=CSharp -visitor -listener   W3CebnfParser.g4


dotnet restore w3cebnf.csproj
dotnet build w3cebnf.csproj

exit 0
