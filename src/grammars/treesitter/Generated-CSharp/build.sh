# Generated from trgen 0.23.12
set -e
if [ -f transformGrammar.py ]; then python3 transformGrammar.py ; fi

version=`dotnet trxml2 Other.csproj | fgrep 'PackageReference/@Version' | awk -F= '{print $2}'`

antlr4 -v $version -encoding utf-8 -Dlanguage=CSharp   JavaScriptLexer.g4
antlr4 -v $version -encoding utf-8 -Dlanguage=CSharp   JavaScriptParser.g4


dotnet restore treesitter.csproj
dotnet build treesitter.csproj

exit 0
