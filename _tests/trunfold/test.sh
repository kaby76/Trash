#!/bin/bash

rm -rf Generated/
trfold
dotnet restore Generated/Test.csproj
dotnet build Generated/Test.csproj
trparse -i "1+2" | trst > output
diff output Gold/
rm -rf Generated/
