# Generated from trgen 0.21.2
set -e
if [ -f transformGrammar.py ]; then python3 transformGrammar.py ; fi
dotnet restore antlr4.csproj
dotnet build antlr4.csproj
exit 0
