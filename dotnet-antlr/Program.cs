using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace dotnet_antlr
{
    class Program
	{
		class Options
		{
			[Option('g', "grammar-files", Required = false, HelpText = "A list of semi-colon separated grammar files.")]
			public string GrammarFiles { get; set; }

            [Option('n', "namespace", Required = false, HelpText = "The namespace for all generated files.")]
            public string DefaultNamespace { get; set; }

			[Option('p', "package", Required = false, HelpText = "PackageReference's to include, in name/version pairs.")]
			public string Packages { get; set; }

            [Option('s', "start-rule", Required = false, HelpText = "Start rule name.")]
            public string StartRule { get; set; }

            [Option('o', "output-directory", Required = false, HelpText = "The output directory for the project.")]
            public string OutputDirectory { get; set; }
        }

        static void Main(string[] args)
		{
			try
			{
				MainInternal(args);
			}
			catch (Exception e)
			{
				Console.Error.WriteLine(e);
			}
		}

        static int MainInternal(string[] args)
        {
            var result = Parser.Default.ParseArguments<Options>(args);
            List<string> grammarFiles = new List<string>();
            string @namespace = null;
            Dictionary<string, string> packages = new Dictionary<string, string>();
            string startRule = null;
            string outputDirectory = null;
            result.WithParsed(o =>
            {
                if (o.GrammarFiles != null) grammarFiles = o.GrammarFiles?.Split(",").ToList();
                else
                {
                    var g = new Domemtech.Globbing.Glob();
                    var files = g.Contents("./*.g4");
                    foreach (var f in files)
                    {
                        if (f is System.IO.FileInfo fi)
                        {
                            grammarFiles.Add(fi.Name);
                        }
                    }
                }
                if (o.DefaultNamespace != null) @namespace = o.DefaultNamespace;
                else
                {
                    string current = Directory.GetCurrentDirectory();
                    string ns = Path.GetFileName(current);
                    @namespace = ns;
                }
                if (o.StartRule != null) startRule = o.StartRule;
                else
                {
                    if (o.GrammarFiles == null && grammarFiles.Count == 0)
                    {
                        startRule = "file";
                    }
                    else startRule = null;
                }
                if (o.OutputDirectory != null) outputDirectory = o.OutputDirectory;
                else
                {
                    outputDirectory = "Generated/";
                }
            });

            if ((grammarFiles != null && grammarFiles.Count > 0) && startRule == null)
            {
                throw new Exception("You must specify a start rule.");
            }

            var path = Environment.CurrentDirectory;
            path = path + Path.DirectorySeparatorChar + outputDirectory;
            outputDirectory = System.IO.Path.GetFullPath(path);
            try
            {
                // Create a directory containing a C# project with grammars.
                Directory.CreateDirectory(outputDirectory);
            }
            catch (Exception e)
            {
                throw e;
            }

            AddCsproj(grammarFiles, outputDirectory);

            // Add grammars
            AddGrammars(grammarFiles, outputDirectory);

            // Add Program.cs
            AddMain(grammarFiles, @namespace, startRule, outputDirectory);

            // Add ErrorListener.cs
            AddErrorListener(@namespace, outputDirectory);

            // Add TreeOutput.cs
            AddTreeOutput(@namespace, outputDirectory);

            return 0;
        }

        private static void AddGrammars(List<string> grammarFiles, string outputDirectory)
        {
            if (grammarFiles != null && grammarFiles.Any())
            {
                // Copy all files to generated directory.
                foreach (var g in grammarFiles)
                {
                    var i = System.IO.File.ReadAllText(g);
                    var n = System.IO.Path.GetFileName(g);
                    var fn = outputDirectory + n;
                    System.IO.File.WriteAllText(fn, i);
                }
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(@"
grammar arithmetic;

file : expression (SEMI expression)* EOF;

expression
   :  expression POW expression
   |  expression (TIMES | DIV) expression
   |  expression (PLUS | MINUS) expression
   |  LPAREN expression RPAREN
   |  (PLUS | MINUS)* atom
   ;

atom
   : scientific
   | variable
   ;

scientific
   : SCIENTIFIC_NUMBER
   ;

variable
   : VARIABLE
   ;


VARIABLE
   : VALID_ID_START VALID_ID_CHAR*
   ;


fragment VALID_ID_START
   : ('a' .. 'z') | ('A' .. 'Z') | '_'
   ;


fragment VALID_ID_CHAR
   : VALID_ID_START | ('0' .. '9')
   ;

//The NUMBER part gets its potential sign from ""(PLUS | MINUS) * atom"" in the expression rule
SCIENTIFIC_NUMBER
   : NUMBER(E SIGN ? UNSIGNED_INTEGER) ?
   ;

fragment NUMBER
   : ('0'..'9') + ('.'('0'..'9') +) ?
   ;

fragment UNSIGNED_INTEGER
   : ('0'..'9') +
   ;


fragment E
   : 'E' | 'e'
   ;


fragment SIGN
   : ('+' | '-')
   ;


LPAREN
   : '('
   ;


RPAREN
   : ')'
   ;


PLUS
   : '+'
   ;


MINUS
   : '-'
   ;


TIMES
   : '*'
   ;


DIV
   : '/'
   ;


GT
   : '>'
   ;


LT
   : '<'
   ;


EQ
   : '='
   ;


POINT
   : '.'
   ;


POW
   : '^'
   ;

SEMI
   : ';'
   ;

WS
   :[ \r\n\t] + ->skip
   ; ");
                var fn = outputDirectory + "arithmetic.g4";
                System.IO.File.WriteAllText(fn, sb.ToString());
            }
        }

        private static void AddCsproj(List<string> grammarFiles, string outputDirectory)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"
<Project Sdk=""Microsoft.NET.Sdk"" >
	<PropertyGroup>
    <TargetFrameworks>net5.0;net471;net480;netcoreapp3.1</TargetFrameworks>
        <OutputType>Exe</OutputType>
	</PropertyGroup>
	<ItemGroup>");

            if (grammarFiles != null && grammarFiles.Any())
            {
                foreach (var grammar in grammarFiles)
                {
                    sb.AppendLine("<Antlr4 Include=\"" + Path.GetFileName(grammar) + "\" />");
                }
            }
            else
            {
                sb.AppendLine(@"<Antlr4 Include=""arithmetic.g4"" />");
            }

            sb.AppendLine(@"</ItemGroup>
	<ItemGroup>
		<PackageReference Include=""Antlr4.Runtime.Standard"" Version =""4.9.0"" />
		<PackageReference Include=""Antlr4BuildTasks"" Version = ""8.10"" />
		<PackageReference Include= ""AntlrTreeEditing"" Version = ""1.9"" />
	</ItemGroup>
	<PropertyGroup>
		<RestoreProjectStyle>PackageReference</RestoreProjectStyle>
	</PropertyGroup>
	<PropertyGroup Condition=""'$(Configuration)|$(Platform)'=='Debug|AnyCPU'"" >
		<NoWarn>1701;1702;3021</NoWarn>
	</PropertyGroup>
</Project>");
            var fn = outputDirectory + "Test.csproj";
            System.IO.File.WriteAllText(fn, sb.ToString());
        }

        private static void AddTreeOutput(string @namespace, string outputDirectory)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
namespace " + @namespace + @"
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Misc;
    using Antlr4.Runtime.Tree;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class TreeOutput
    {
        private static int changed = 0;
        private static bool first_time = true;

        public static StringBuilder OutputTree(IParseTree tree, Lexer lexer, Parser parser, CommonTokenStream stream)
        {
            changed = 0;
            first_time = true;
            var sb = new StringBuilder();
            ParenthesizedAST(tree, sb, lexer, parser, stream);
            return sb;
        }

        private static void ParenthesizedAST(IParseTree tree, StringBuilder sb, Lexer lexer, Parser parser, CommonTokenStream stream, int level = 0)
        {
            if (tree as TerminalNodeImpl != null)
            {
                TerminalNodeImpl tok = tree as TerminalNodeImpl;
                Interval interval = tok.SourceInterval;
                IList<IToken> inter = null;
                if (tok.Symbol.TokenIndex >= 0)
                    inter = stream?.GetHiddenTokensToLeft(tok.Symbol.TokenIndex);
                if (inter != null)
                    foreach (var t in inter)
                    {
                        var ty = tok.Symbol.Type;
                        var name = lexer.Vocabulary.GetSymbolicName(ty);
                        StartLine(sb, level);
                        sb.AppendLine(""("" + name + "" text = "" + PerformEscapes(t.Text) + "" "" + lexer.ChannelNames[t.Channel]);
                    }
            {
                var ty = tok.Symbol.Type;
                var name = lexer.Vocabulary.GetSymbolicName(ty);
                StartLine(sb, level);
                sb.AppendLine(""( "" + name + "" i ="" + tree.SourceInterval.a
                    + "" txt ="" + PerformEscapes(tree.GetText())
                    + "" tt ="" + tok.Symbol.Type
                    + "" "" + lexer.ChannelNames[tok.Symbol.Channel]);
            }
        }
            else
            {
                var x = tree as RuleContext;
        var ri = x.RuleIndex;
        var name = parser.RuleNames[ri];
        StartLine(sb, level);
        sb.Append(""( "" + name);
                sb.AppendLine();
            }
            for (int i = 0; i<tree.ChildCount; ++i)
            {
                var c = tree.GetChild(i);
    ParenthesizedAST(c, sb, lexer, parser, stream, level + 1);
}
if (level == 0)
{
    for (int k = 0; k < 1 + changed - level; ++k) sb.Append("") "");
    sb.AppendLine();
    changed = 0;
}
        }

        private static void StartLine(StringBuilder sb, int level = 0)
{
    if (changed - level >= 0)
    {
        if (!first_time)
        {
            for (int j = 0; j < level; ++j) sb.Append(""  "");
            for (int k = 0; k < 1 + changed - level; ++k) sb.Append("") "");
            sb.AppendLine();
        }
        changed = 0;
        first_time = false;
    }
    changed = level;
    for (int j = 0; j < level; ++j) sb.Append(""  "");
}

private static string ToLiteral(string input)
{
    using (var writer = new StringWriter())
    {
        var literal = input;
        literal = literal.Replace(""\\"", ""\\\\"");
        return literal;
    }
}

public static string PerformEscapes(string s)
{
    StringBuilder new_s = new StringBuilder();
    new_s.Append(ToLiteral(s));
    return new_s.ToString();
}
    }
}
");
            string fn = outputDirectory + "TreeOutput.cs";
            System.IO.File.WriteAllText(fn, sb.ToString());
        }

        private static void AddErrorListener(string @namespace, string outputDirectory)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
namespace " + @namespace + @"
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Misc;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class ErrorListener<S> : ConsoleErrorListener<S>
    {
        public bool had_error;

        public override void SyntaxError(TextWriter output, IRecognizer recognizer, S offendingSymbol, int line,
            int col, string msg, RecognitionException e)
        {
            had_error = true;
            base.SyntaxError(output, recognizer, offendingSymbol, line, col, msg, e);
        }
    }
}
");
            string fn = outputDirectory + "ErrorListener.cs";
            System.IO.File.WriteAllText(fn, sb.ToString());
        }

        private static void AddMain(List<string> grammarFiles, string @namespace, string startRule, string outputDirectory)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
namespace " + @namespace + @"
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using System.Text;
    using System.Runtime.CompilerServices;
    public class Program
    {
        public static Parser Parser { get; set; }
        public static Lexer Lexer { get; set; }
        public static ITokenStream TokenStream { get; set; }
        public static IParseTree Tree { get; set; }
        delegate string MyDelegate();
        static void Main(string[] args)
        {
            MyDelegate de = () =>
            {
                StringBuilder ssb = new StringBuilder();
                string line;
                while ((line = System.Console.ReadLine()) != null)
                {
                    ssb.AppendLine(line);
                }
                return ssb.ToString();
            };
            string input = args.Length > 0 ? args[0] : de();
            var str = new AntlrInputStream(input);
            var lexer = new ");
            var lexer_name = "";
            var parser_name = "";
            var lexer = grammarFiles?.Where(d => d.EndsWith("Lexer.g4")).ToList();
            var parser = grammarFiles?.Where(d => d.EndsWith("Parser.g4")).ToList();
            var rest = grammarFiles?.Where(d => !lexer.Contains(d) && !parser.Contains(d)).ToList();
            if ((rest == null && lexer == null && parser == null) || rest.Count == 0)
            {
                lexer_name = "arithmeticLexer";
                parser_name = "arithmeticParser";
            }
            else if (lexer.Count == 1)
            {
                lexer_name = Path.GetFileName(lexer.First().Replace(".g4", ""));
                parser_name = Path.GetFileName(parser.First().Replace(".g4", ""));
            }
            else if (lexer.Count == 0)
            {
                // Combined.
                if (rest.Count == 1)
                {
                    var combined_name = Path.GetFileName(rest.First()).Replace(".g4", "");
                    lexer_name = combined_name + "Lexer";
                    parser_name = combined_name + "Parser";
                }
            }

            sb.Append(lexer_name);
            sb.Append(@"(str);
            Lexer = lexer;
            var tokens = new CommonTokenStream(lexer);
            TokenStream = tokens;
            var parser = new ");
            sb.Append(parser_name);
            sb.Append(@"(tokens);
            Parser = parser;
            var listener_lexer = new ErrorListener<int>();
            var listener_parser = new ErrorListener<IToken>();
            lexer.AddErrorListener(listener_lexer);
            parser.AddErrorListener(listener_parser);
            var tree = parser.");
            sb.Append(startRule);

            sb.AppendLine(@"();
            if (listener_lexer.had_error || listener_parser.had_error)
                System.Console.Error.WriteLine(""error in parse."");
            else
                System.Console.Error.WriteLine(""parse completed."");
            Tree = tree;
        }
    }
}");
            // Test to find an appropriate file name to place this into.
            string fn = outputDirectory + "Program.cs";
            System.IO.File.WriteAllText(fn, sb.ToString());
        }
	}
}
