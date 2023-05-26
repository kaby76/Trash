# Generated from trgen 0.20.14
set -e
if [ -f transformGrammar.py ]; then python3 transformGrammar.py ; fi
dotnet restore
dotnet build
exit 0
