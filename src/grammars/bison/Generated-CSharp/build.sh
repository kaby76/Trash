# Generated from trgen 0.21.0
set -e
if [ -f transformGrammar.py ]; then python3 transformGrammar.py ; fi
dotnet restore bison.csproj
dotnet build bison.csproj
exit 0
