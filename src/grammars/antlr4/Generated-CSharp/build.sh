# Generated from trgen 0.23.12
set -e
if [ -f transformGrammar.py ]; then python3 transformGrammar.py ; fi

version=`dotnet trxml2 Other.csproj | fgrep 'PackageReference/@Version' | awk -F= '{print $2}'`

antlr4 -v $version -encoding utf-8 -Dlanguage=CSharp    ANTLRv4Lexer.g4
antlr4 -v $version -encoding utf-8 -Dlanguage=CSharp -visitor  ANTLRv4Parser.g4


dotnet restore antlr4.csproj
dotnet build antlr4.csproj

exit 0
