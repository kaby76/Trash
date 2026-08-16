# Generated from trgen 2.3.0

if (Test-Path -Path Test.csproj -PathType Leaf) {
    Rename-Item -Path Test.csproj -NewName antlr3.csproj
}

if (Test-Path -Path transformGrammar.py -PathType Leaf) {
    $(& python3 transformGrammar.py ) 2>&1 | Write-Host
}

$version = "4.13.1"

$(& antlr4 -v $version ANTLRv3Lexer.g4 -encoding utf-8 -Dlanguage=CSharp  ; $compile_exit_code = $LASTEXITCODE) | Write-Host
if($compile_exit_code -ne 0){
    exit $compile_exit_code
}
$(& antlr4 -v $version ANTLRv3Parser.g4 -encoding utf-8 -Dlanguage=CSharp  ; $compile_exit_code = $LASTEXITCODE) | Write-Host
if($compile_exit_code -ne 0){
    exit $compile_exit_code
}


$(& dotnet build antlr3.csproj; $compile_exit_code = $LASTEXITCODE) | Write-Host
exit $compile_exit_code
