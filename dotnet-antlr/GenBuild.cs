using System;
using System.IO;
using System.Linq;
using System.Text;

namespace dotnet_antlr
{
    class GenBuild
    {
        public static void AddBuildFile(Program p)
        {
            StringBuilder sb = new StringBuilder();
            if (p.target == Program.TargetType.CSharp)
            {
                sb.AppendLine(@"<!-- Template generated code from Antlr4BuildTasks.dotnet-antlr v " + Program.version + @" -->
<Project Sdk=""Microsoft.NET.Sdk"" >
  <PropertyGroup>
    <TargetFramework>net5.0</TargetFramework>
    <OutputType>Exe</OutputType>
  </PropertyGroup>
  ");

                if (!p.antlr4cs)
                {
                    sb.AppendLine("<ItemGroup>");
                    if (p.tool_grammar_files != null && p.tool_grammar_files.Any())
                    {
                        foreach (var grammar in p.tool_grammar_files)
                        {
                            if (p.@namespace == null)
                                sb.AppendLine("<Antlr4 Include=\"" + Path.GetFileName(grammar) + "\" />");
                            else
                            {
                                sb.AppendLine("<Antlr4 Include=\"" + Path.GetFileName(grammar) + "\">");
                                sb.AppendLine("<Package>" + p.@namespace + "</Package>");
                                sb.AppendLine("</Antlr4>");
                            }
                        }
                    }
                    else
                    {
                        sb.AppendLine(@"<Antlr4 Include=""Arithmetic.g4"" />");
                    }
                    sb.AppendLine("</ItemGroup>");
                }

                if (!p.antlr4cs)
                {
                    sb.AppendLine(@"
  <ItemGroup>
    <PackageReference Include=""Antlr4.Runtime.Standard"" Version =""4.9.1"" />
    <PackageReference Include=""Antlr4BuildTasks"" Version = ""8.13"" PrivateAssets=""all"" />
  </ItemGroup>");
                }
                else
                {
                    sb.AppendLine(@"
  <ItemGroup>
    <PackageReference Include=""Antlr4"" Version=""4.6.6"">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include=""Antlr4.Runtime"" Version=""4.6.6"" />
  </ItemGroup>");
                }
                sb.AppendLine(@"
  <PropertyGroup>
    <RestoreProjectStyle>PackageReference</RestoreProjectStyle>
  </PropertyGroup>
  <PropertyGroup Condition=""'$(Configuration)|$(Platform)'=='Debug|AnyCPU'"" >
    <NoWarn>1701;1702;3021</NoWarn>
  </PropertyGroup>

  <PropertyGroup>
    <!--
      need the CData since this blob is just going to
      be embedded in a mini batch file by studio/msbuild
    -->
    <MyTester><![CDATA[");
                if (p.env_type == Program.EnvType.Windows)
                {
                    sb.AppendLine(@"
set ERR=0
for %%G in (..\examples\*) do (
  setlocal EnableDelayedExpansion
  set FILE=%%G
  set X1=%%~xG
  set X2=%%~nG
  set X3=%%~pG
  if !X1! neq .errors (
    echo !FILE!
    cat !FILE! | bin\Debug\net5.0\Test.exe
    if not exist !FILE!.errors (
      if ERRORLEVEL 1 set ERR=1
    ) else (
      echo Expected.
    )
  )
)
EXIT %ERR%
");
                }
                else
                {
                    sb.AppendLine(@"
err=0
for g in ../examples/*
do
  file=$g
  x1=""${g##*.}""
  if [ ""$x1"" != ""errors"" ]
  then
    echo $file
    cat $file | bin/Debug/net5.0/Test
    status=""$?""
    if [ -f ""$file"".errors ]
    then
      if [ ""$stat"" = ""0"" ]
      then
        echo Expected parse fail.
        err=1
      else
        echo Expected.
      fi
    else
      if [ ""$status"" != ""0"" ]
      then
        err=1
      fi
    fi
  fi
done
exit $err
");
                }
                sb.AppendLine(@"]]></MyTester>
</PropertyGroup>

  <Target Name=""Test"" >
    <Message Text=""testing"" />
    <Exec Command=""$(MyTester)"" >
       <Output TaskParameter=""ExitCode"" PropertyName =""ErrorCode"" />
    </Exec>
    <Message Importance=""high"" Text=""$(ErrorCode)""/>
  </Target>

</Project>");
                var fn = p.outputDirectory + "Test.csproj";
                System.IO.File.WriteAllText(fn, Program.Localize(p.line_translation, sb.ToString()));
            }
            else if (p.target == Program.TargetType.Java)
            {
                sb.AppendLine(@"
# Generated code from Antlr4BuildTasks.dotnet-antlr v " + Program.version + @"
# Makefile for " + String.Join(", ", p.tool_grammar_files) + @"

JAR = ~/Downloads/antlr-4.9.1-complete.jar
CLASSPATH = $(JAR)" + (p.line_translation == Program.LineTranslationType.CRLF ? "\\;" : ":") + @".

.SUFFIXES: .g4 .java .class

.java.class:
	javac -cp $(CLASSPATH) $*.java

ANTLRGRAMMARS ?= $(wildcard *.g4)

GENERATED = " + String.Join(" ", p.generated_files) + @"

SOURCES = $(GENERATED) \
    " + (p.@namespace != null ? p.@namespace.Replace('.', '/') + '/' : "") + @"Program.java \
    " + (p.@namespace != null ? p.@namespace.Replace('.', '/') + '/' : "") + @"ErrorListener.java

default: classes

classes: $(GENERATED) $(SOURCES:.java=.class)

clean:
	rm -f " + (p.@namespace != null ? p.@namespace.Replace('.', '/') + '/' : "") + @"*.class
	rm -f " + (p.@namespace != null ? p.@namespace.Replace('.', '/') + '/' : "") + @"*.interp
	rm -f " + (p.@namespace != null ? p.@namespace.Replace('.', '/') + '/' : "") + @"*.tokens
	rm -f $(GENERATED)

run:
	java -classpath $(CLASSPATH) " + (p.@namespace != null ? p.@namespace + "." : "") + @"Program $(RUNARGS)

" + p.lexer_generated_file_name + " : " + p.lexer_grammar_file_name + @"
	java -jar $(JAR) " + (p.@namespace != null ? " -package " + p.@namespace : "") + @" $<

" + p.parser_generated_file_name + " : " + p.parser_grammar_file_name + @"
	java -jar $(JAR) " + (p.@namespace != null ? " -package " + p.@namespace : "") + @" $<
");
                var fn = p.outputDirectory + "makefile";
                System.IO.File.WriteAllText(fn, Program.Localize(p.line_translation, sb.ToString()));
            }
            else if (p.target == Program.TargetType.JavaScript)
            {
                sb.AppendLine(@"
# Generated code from Antlr4BuildTasks.dotnet-antlr v " + Program.version + @"
# Makefile for " + String.Join(", ", p.tool_grammar_files) + @"

JAR = ~/Downloads/antlr4-4.9.2-SNAPSHOT-complete.jar
RT = ~/Downloads/antlr4-4.9.2-SNAPSHOT-runtime-js.zip

CLASSPATH = $(JAR)" + (p.line_translation == Program.LineTranslationType.CRLF ? "\\;" : ":") + @".

.SUFFIXES: .g4 .js

ANTLRGRAMMARS ?= $(wildcard *.g4)

GENERATED = " + String.Join(" ", p.generated_files) + @"

SOURCES = $(GENERATED) \
    " + (p.@namespace != null ? p.@namespace.Replace('.', '/') + '/' : "") + @"index.js

default: classes

classes: $(SOURCES)
	npm install
	cd node_modules/antlr4; unzip -q -o $(RT)

clean:
	rm -rf node_modules
	rm -f package-lock.json
	rm -f " + (p.@namespace != null ? p.@namespace.Replace('.', '/') + '/' : "") + @"*.interp
	rm -f " + (p.@namespace != null ? p.@namespace.Replace('.', '/') + '/' : "") + @"*.tokens
	rm -f $(GENERATED)

run:
	node index.js $(RUNARGS)

" + p.lexer_generated_file_name + " : " + p.lexer_grammar_file_name + @"
	java -jar $(JAR) -Dlanguage=JavaScript " + (p.@namespace != null ? " -package " + p.@namespace : "") + @" $<

" + p.parser_generated_file_name + " : " + p.parser_grammar_file_name + @"
	java -jar $(JAR) -Dlanguage=JavaScript " + (p.@namespace != null ? " -package " + p.@namespace : "") + @" $<
");
                var fn = p.outputDirectory + "makefile";
                System.IO.File.WriteAllText(fn, Program.Localize(p.line_translation, sb.ToString()));
                sb = new StringBuilder();
                sb.AppendLine(@"
{
  ""name"": ""i"",
  ""version"": ""1.0.0"",
  ""description"": """",
  ""main"": ""index.js"",
  ""scripts"": {
    ""test"": ""echo \""Error: no test specified\"" && exit 1""
  },
  ""author"": """",
  ""license"": ""ISC"",
  ""dependencies"": {
    ""antlr4"": ""^4.9.1"",
    ""fs-extra"": ""^9.1.0"",
    ""typescript-string-operations"": ""^1.4.1""
  },
  ""type"": ""module""
}
");
                fn = p.outputDirectory + "package.json";
                System.IO.File.WriteAllText(fn, Program.Localize(p.line_translation, sb.ToString()));
            }
            else if (p.target == Program.TargetType.Python3)
            {
                sb.AppendLine(@"
# Generated code from Antlr4BuildTasks.dotnet-antlr v " + Program.version + @"
# Makefile for " + String.Join(", ", p.tool_grammar_files) + @"

JAR = ~/Downloads/antlr-4.9.1-complete.jar
CLASSPATH = $(JAR)" + (p.line_translation == Program.LineTranslationType.CRLF ? "\\;" : ":") + @".

.SUFFIXES: .g4 .py

ANTLRGRAMMARS ?= $(wildcard *.g4)

GENERATED = " + String.Join(" ", p.generated_files) + @"

SOURCES = $(GENERATED) \
    " + (p.@namespace != null ? p.@namespace.Replace('.', '/') + '/' : "") + @"Program.py

default: classes

classes: $(SOURCES)
	pip install antlr4-python3-runtime
	pip install readchar

clean:
	rm -f *.tokens *.interp
	rm -f " + (p.@namespace != null ? p.@namespace.Replace('.', '/') + '/' : "") + @"*.interp
	rm -f " + (p.@namespace != null ? p.@namespace.Replace('.', '/') + '/' : "") + @"*.tokens
	rm -f $(GENERATED)

run:
	python Program.py $(RUNARGS)

" + p.lexer_generated_file_name + " : " + p.lexer_grammar_file_name + @"
	java -jar $(JAR) -Dlanguage=Python3 " + (p.@namespace != null ? " -package " + p.@namespace : "") + @" $<

" + p.parser_generated_file_name + " : " + p.parser_grammar_file_name + @"
	java -jar $(JAR) -Dlanguage=Python3 " + (p.@namespace != null ? " -package " + p.@namespace : "") + @" $<
");
                var fn = p.outputDirectory + "makefile";
                System.IO.File.WriteAllText(fn, Program.Localize(p.line_translation, sb.ToString()));
            }
            else if (p.target == Program.TargetType.Dart)
            {
                try { Directory.CreateDirectory(p.outputDirectory + "lib"); }
                catch (Exception) { throw; }

                sb = new StringBuilder();
                sb.AppendLine(@"
include: package:pedantic/analysis_options.yaml
analyzer:
");
                var fn = p.outputDirectory + "analysis_options.yaml";
                System.IO.File.WriteAllText(fn, Program.Localize(p.line_translation, sb.ToString()));

                sb = new StringBuilder();
                sb.AppendLine(@"
name: cli
description: A sample command-line application.
# version: 1.0.0
# homepage: https://www.example.com

environment:
  sdk: '>=2.8.1 <3.0.0'

#dependencies:
#  path: ^1.7.0
dependencies:
  antlr4: 4.9.1

dev_dependencies:
  pedantic: ^1.9.0
  test: ^1.14.4

");
                fn = p.outputDirectory + "pubspec.yaml";
                System.IO.File.WriteAllText(fn, Program.Localize(p.line_translation, sb.ToString()));
                
                sb = new StringBuilder();
                sb.AppendLine(@"
# Generated code from Antlr4BuildTasks.dotnet-antlr v " + Program.version + @"
# Makefile for " + String.Join(", ", p.tool_grammar_files) + @"
JAR = " + p.antlr_tool_path + @"
CLASSPATH = $(JAR)" + (p.line_translation == Program.LineTranslationType.CRLF ? "\\;" : ":") + @".
.SUFFIXES: .g4 .dart
ANTLRGRAMMARS ?= $(wildcard *.g4)
GENERATED = " + String.Join(" ", p.generated_files) + @"
SOURCES = $(GENERATED) \
    bin/" + (p.@namespace != null ? p.@namespace.Replace('.', '/') + '/' : "") + @"cli.dart
default: classes
classes: $(SOURCES)
	dart pub get
clean:
	rm -f lib/*.tokens lib/*.interp
	rm -f $(GENERATED)
	rm -f pubspec.lock
run:
	dart run bin/cli.dart $(RUNARGS)
" + p.lexer_generated_file_name + " : " + p.lexer_grammar_file_name + @"
	java -jar $(JAR) -Dlanguage=Dart -o lib " + (p.@namespace != null ? "-package " + p.@namespace : "") + @" $<
" + p.parser_generated_file_name + " : " + p.parser_grammar_file_name + @"
	java -jar $(JAR) -Dlanguage=Dart -o lib " + (p.@namespace != null ? "-package " + p.@namespace : "") + @" $<
");
                fn = p.outputDirectory + "makefile";
                System.IO.File.WriteAllText(fn, Program.Localize(p.line_translation, sb.ToString()));
            }
        }
    }
}
