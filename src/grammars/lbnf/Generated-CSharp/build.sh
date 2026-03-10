# Generated from trgen 0.23.43
set -e
if [ -f transformGrammar.py ]; then python3 transformGrammar.py ; fi

version=`dotnet trxml2 Other.csproj | fgrep 'PackageReference/@Version' | awk -F= '{print $2}'`

antlr4 -visitor -v $version -encoding utf-8 -Dlanguage=CSharp   lbnfLexer.g4
antlr4 -visitor -v $version -encoding utf-8 -Dlanguage=CSharp   lbnfParser.g4


dotnet restore lbnf.csproj
dotnet build lbnf.csproj

exit 0
