# Generated from trgen 2.3.0

if (Test-Path -Path Test.csproj -PathType Leaf) {
    Rename-Item -Path Test.csproj -NewName lbnf.csproj
}

if (Test-Path -Path transformGrammar.py -PathType Leaf) {
    $(& python3 transformGrammar.py ) 2>&1 | Write-Host
}

$version = "4.13.1"

$(& antlr4 -v $version lbnfLexer.g4 -encoding utf-8 -Dlanguage=CSharp  ; $compile_exit_code = $LASTEXITCODE) | Write-Host
if($compile_exit_code -ne 0){
    exit $compile_exit_code
}
$(& antlr4 -v $version lbnfParser.g4 -encoding utf-8 -Dlanguage=CSharp  ; $compile_exit_code = $LASTEXITCODE) | Write-Host
if($compile_exit_code -ne 0){
    exit $compile_exit_code
}


$(& dotnet build lbnf.csproj; $compile_exit_code = $LASTEXITCODE) | Write-Host
exit $compile_exit_code
