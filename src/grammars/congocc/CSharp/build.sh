#!/usr/bin/bash
set -e
if [ -f Test.csproj ]; then mv Test.csproj congocc.csproj; fi
if [ -f transformGrammar.py ]; then python3 transformGrammar.py; fi
antlr4 -v 4.13.1 -encoding utf-8 -Dlanguage=CSharp CongoCC.g4
dotnet build congocc.csproj
