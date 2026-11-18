if (Test-Path Test.csproj) {
    Move-Item Test.csproj congocc.csproj
}
if (Test-Path transformGrammar.py) {
    python3 transformGrammar.py
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}
antlr4 -v 4.13.1 -encoding utf-8 -Dlanguage=CSharp CongoCC.g4
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet build congocc.csproj
exit $LASTEXITCODE
