
cwd=`pwd | sed 's%/c%c:%' | sed 's%/%\\\\%g'`
dotnet nuget add source $cwd\\trgen\\bin\\Debug\\ --name "nuget-trgen"
dotnet nuget add source $cwd\\trjson\\bin\\Debug\\ --name "nuget-trjson"
dotnet nuget add source $cwd\\trparse\\bin\\Debug\\ --name "nuget-trparse"
dotnet nuget add source $cwd\\trprint\\bin\\Debug\\ --name "nuget-trprint"
dotnet nuget add source $cwd\\trst\\bin\\Debug\\ --name "nuget-trst"
dotnet nuget add source $cwd\\trtext\\bin\\Debug\\ --name "nuget-trtext"
dotnet nuget add source $cwd\\trtokens\\bin\\Debug\\ --name "nuget-trtokens"
dotnet nuget add source $cwd\\trtree\\bin\\Debug\\ --name "nuget-trtree"
dotnet nuget add source $cwd\\trxml\\bin\\Debug\\ --name "nuget-trxml"
