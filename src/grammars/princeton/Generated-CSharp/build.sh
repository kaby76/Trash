# Generated from trgen 0.23.23
set -e
if [ -f transformGrammar.py ]; then python3 transformGrammar.py ; fi

version=`dotnet trxml2 Other.csproj | fgrep 'PackageReference/@Version' | awk -F= '{print $2}'`

antlr4 -v $version -encoding utf-8 -Dlanguage=CSharp -visitor -listener   Princeton.g4
antlr4 -v $version -encoding utf-8 -Dlanguage=CSharp -visitor -listener   Princeton.g4


dotnet restore princeton.csproj
dotnet build princeton.csproj

exit 0
