namespace dotnet_antlr
{
    using CommandLine;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    class Program
    {
        static string version = "1.2";

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

            [Option('t', "target", Required = false, HelpText = "The target language for the project.")]
            public string Target { get; set; }
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
            string target = null;
            result.WithParsed(o =>
            {
                if (o.DefaultNamespace != null) @namespace = o.DefaultNamespace;
                else
                {
                    string current = Directory.GetCurrentDirectory();
                    string ns = Path.GetFileName(current);
                    @namespace = ns;
                }
                if (o.Target != null && o.Target == "Java")
                {
                    target = o.Target;
                    // Java is probably the most nasty POS when it comes to namespaces.
                    // **The directory must be the same as the namespace, no matter what,
                    // because there is no form of calling java.exe with ANY classpath that
                    // will result if finding the classes in the namespace. YOU MUST HAVE
                    // THE NAMESPACE BE THE SAME AS THE DIRECTORY NAME!
                    @namespace = "Generated";
                }
                else
                {
                    target = "C#";
                }
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

            // Add .csproj or pom.xml.
            AddBuildFile(target, grammarFiles, outputDirectory);

            // Add grammars
            AddGrammars(grammarFiles, outputDirectory);

            // Add driver program.
            AddMain(target, grammarFiles, @namespace, startRule, outputDirectory);

            // Add ErrorListener code.
            AddErrorListener(target, @namespace, outputDirectory);

            // Add TreeOutput code.
            AddTreeOutput(target, @namespace, outputDirectory);

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
// Template generated code from Antlr4BuildTasks.dotnet-antlr v " + version + @"

grammar Arithmetic;

file : expression (SEMI expression)* EOF;
expression : expression POW expression | expression (TIMES | DIV) expression | expression (PLUS | MINUS) expression | LPAREN expression RPAREN | (PLUS | MINUS)* atom ;
atom : scientific | variable ;
scientific : SCIENTIFIC_NUMBER ;
variable : VARIABLE ;

VARIABLE : VALID_ID_START VALID_ID_CHAR* ;
SCIENTIFIC_NUMBER : NUMBER (E SIGN? UNSIGNED_INTEGER)? ;
LPAREN : '(' ;
RPAREN : ')' ;
PLUS : '+' ;
MINUS : '-' ;
TIMES : '*' ;
DIV : '/' ;
GT : '>' ;
LT : '<' ;
EQ : '=' ;
POINT : '.' ;
POW : '^' ;
SEMI : ';' ;
WS : [ \r\n\t] + -> channel(HIDDEN) ;

fragment VALID_ID_START : ('a' .. 'z') | ('A' .. 'Z') | '_' ;
fragment VALID_ID_CHAR : VALID_ID_START | ('0' .. '9') ;
fragment NUMBER : ('0' .. '9') + ('.' ('0' .. '9') +)? ;
fragment UNSIGNED_INTEGER : ('0' .. '9')+ ;
fragment E : 'E' | 'e' ;
fragment SIGN : ('+' | '-') ;
");
                var fn = outputDirectory + "arithmetic.g4";
                System.IO.File.WriteAllText(fn, sb.ToString());
            }
        }

        private static void AddBuildFile(string target, List<string> grammarFiles, string outputDirectory)
        {
            StringBuilder sb = new StringBuilder();
            if (target == "C#")
            {
                sb.AppendLine(@"<!-- Template generated code from Antlr4BuildTasks.dotnet-antlr v " + version + @" -->
<Project Sdk=""Microsoft.NET.Sdk"" >
  <PropertyGroup>
    <TargetFrameworks>net5.0</TargetFrameworks>
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
    <PackageReference Include=""Antlr4BuildTasks"" Version = ""8.10"" PrivateAssets=""all"" />
    <PackageReference Include=""AntlrTreeEditing"" Version = ""1.9"" />
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
        }

        private static void AddTreeOutput(string target, string @namespace, string outputDirectory)
        {
            StringBuilder sb = new StringBuilder();
            if (target == "C#")
            {
                sb.Append(@"
// Template generated code from Antlr4BuildTasks.dotnet-antlr v " + version + @"
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
            else if (target == "Java")
            {
                sb.Append(@"
// Template generated code from Antlr4BuildTasks.dotnet-antlr v " + version + @"
package " + @namespace + @";

import java.util.*;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.util.Arrays;
import java.util.HashMap;
import org.antlr.v4.gui.TreeViewer;
import org.antlr.v4.runtime.ANTLRInputStream;
import org.antlr.v4.runtime.Lexer;
import org.antlr.v4.runtime.CharStream;
import org.antlr.v4.runtime.CharStreams;
import org.antlr.v4.runtime.CodePointCharStream;
import org.antlr.v4.runtime.CommonTokenStream;
import org.antlr.v4.runtime.tree.ParseTree;
import org.antlr.v4.runtime.tree.TerminalNodeImpl;
import org.antlr.v4.runtime.*;
import java.util.regex.Pattern;
import java.io.File;
import java.lang.reflect.*;

public class TreeOutput
{
    private static int changed = 0;
    private static boolean first_time = true;

    public static StringBuilder OutputTree(ParseTree tree, CommonTokenStream stream)
    {
        var sb = new StringBuilder();
        ParenthesizedAST(tree, sb, stream, 0);
        return sb;
    }

    private static void ParenthesizedAST(ParseTree tree, StringBuilder sb, CommonTokenStream stream, int level)
    {
        // Antlr always names a non-terminal with first letter lowercase,
        // but renames it when creating the type in C#. So, remove the prefix,
        // lowercase the first letter, and remove the trailing ""Context"" part of
        // the name. Saves big time on output!
        if (tree instanceof TerminalNodeImpl)
        {
            TerminalNodeImpl tok = (TerminalNodeImpl)tree;
            var interval = tok.getSourceInterval();
            var inter = stream.getHiddenTokensToLeft(tok.symbol.getTokenIndex());
            if (inter != null)
                for (var t : inter)
                {
                    StartLine(sb, tree, stream, level);
                    sb.append(""( HIDDEN text="" + PerformEscapes(t.getText()));
                    sb.append(System.lineSeparator());
                }
                StartLine(sb, tree, stream, level);
                Lexer xxx = (Lexer)stream.getTokenSource();
                String[] yyy = xxx.getChannelNames();
                sb.append(""( "" + yyy[tok.getSymbol().getChannel()]
                    + "" i ="" + tree.getSourceInterval().a
                    + "" txt ="" + PerformEscapes(tree.getText())
                    + "" tt ="" + tok.getSymbol().getType());
                    sb.append(System.lineSeparator());
            }
            else
            {
                var fixed_name = tree.getClass().getName().toString();
                fixed_name = fixed_name.replaceAll("" ^[^$]*[$]"", """");
                fixed_name = fixed_name.substring(0, fixed_name.length() - ""Context"".length());
                fixed_name = Character.toString(fixed_name.charAt(0)).toLowerCase()
                        + fixed_name.substring(1);
                StartLine(sb, tree, stream, level);
                sb.append(""( "" + fixed_name);
                sb.append(System.lineSeparator());
            }
            for (int i = 0; i < tree.getChildCount(); ++i)
            {
                var c = tree.getChild(i);
                ParenthesizedAST(c, sb, stream, level + 1);
            }
            if (level == 0)
            {
                for (int k = 0; k < 1 + changed - level; ++k) sb.append("") "");
                sb.append(System.lineSeparator());
                changed = 0;
            }
        }

        private static void StartLine(StringBuilder sb, ParseTree tree, CommonTokenStream stream, int level)
        {
            if (changed - level >= 0)
            {
                if (!first_time)
                {
                    for (int j = 0; j < level; ++j) sb.append(""  "");
                    for (int k = 0; k < 1 + changed - level; ++k) sb.append("") "");
                    sb.append(System.lineSeparator());
                }
                changed = 0;
                first_time = false;
            }
            changed = level;
            for (int j = 0; j < level; ++j) sb.append(""  "");
        }

        private static String ToLiteral(String input)
        {
            var literal = input;
            literal = literal.replace(""\\"", ""\\\\"");
            literal = input.replace(""\b"", ""\\b"");
            literal = literal.replace(""\n"", ""\\n"");
            literal = literal.replace(""\t"", ""\\t"");
            literal = literal.replace(""\r"", ""\\r"");
            literal = literal.replace(""\f"", ""\\f"");
            literal = literal.replace(""\"""", ""\\\"""");
            literal = literal.replace(String.format(""\"" +{0}\t\"""", ""\n""), """");
            return literal;
        }

        public static String PerformEscapes(String s)
        {
            StringBuilder new_s = new StringBuilder();
            new_s.append(ToLiteral(s));
            return new_s.toString();
        }
    }
");

                string fn = outputDirectory + "TreeOutput.java";
                System.IO.File.WriteAllText(fn, sb.ToString());
            }
        }

        private static void AddErrorListener(string target, string @namespace, string outputDirectory)
        {
            StringBuilder sb = new StringBuilder();
            if (target == "C#")
            {
                sb.Append(@"
// Template generated code from Antlr4BuildTasks.dotnet-antlr v " + version + @"
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
            else if (target == "Java")
            {
                sb.Append(@"
// Template generated code from Antlr4BuildTasks.dotnet-antlr v " + version + @"
package " + @namespace + @";

import org.antlr.v4.runtime.*;

public class ErrorListener extends ConsoleErrorListener
{
    public boolean had_error = false;
    
    @Override
    public void syntaxError(Recognizer<?, ?> recognizer,
        Object offendingSymbol,
        int line,
        int charPositionInLine,
        String msg,
        RecognitionException e)
    {
        had_error = true;
        super.syntaxError(recognizer, offendingSymbol, line, charPositionInLine, msg, e);
    }
}
");
                string fn = outputDirectory + "ErrorListener.java";
                System.IO.File.WriteAllText(fn, sb.ToString());
            }
        }

        private static void AddMain(string target, List<string> grammarFiles, string @namespace, string startRule, string outputDirectory)
        {
            StringBuilder sb = new StringBuilder();
            var lexer_name = "";
            var parser_name = "";
            // lexer and parser are set if the grammar is partitioned.
            // rest is set if there are grammar is combined.
            var lexer = grammarFiles?.Where(d => d.EndsWith("Lexer.g4")).ToList();
            var parser = grammarFiles?.Where(d => d.EndsWith("Parser.g4")).ToList();
            var rest = grammarFiles?.Where(d => !d.EndsWith("Parser.g4") && !d.EndsWith("Lexer.g4")).ToList();
            if ((rest == null || rest.Count == 0) && lexer == null && parser == null)
            {
                // I have no clue what your grammars are.
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
            if (target == "C#")
            {
                sb.Append(@"
// Template generated code from Antlr4BuildTasks.dotnet-antlr v " + version + @"
namespace " + @namespace + @"
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using System.Text;
    using System.Runtime.CompilerServices;
    public class Program
    {
        static void Main(string[] args)
        {
            bool show_tree = false;
            bool show_tokens = false;
            string input = null;
            for (int i = 0; i < args.Length; ++i)
            {
                if (args[i].Equals(""-tokens""))
                {
                    show_tokens = true;
                    continue;
                }
                else if (args[i].Equals(""-tree""))
                {
                    show_tree = true;
                    continue;
                }
                else if (args[i].Equals(""-input""))
                    input = args[i];
            }
            if (input == null)
            {
                StringBuilder sb = new StringBuilder();
                int ch;
                while ((ch = System.Console.Read()) != -1)
                {
                    sb.Append((char)ch);
                }
                input = sb.ToString();
            }
            var str = new AntlrInputStream(input);
            var lexer = new " + lexer_name + @"(str);
            if (show_tokens)
            {
                StringBuilder new_s = new StringBuilder();
                for (int i = 0; ; ++i)
                {
                    var ro_token = lexer.NextToken();
                    var token = (CommonToken)ro_token;
                    token.TokenIndex = i;
                    new_s.AppendLine(token.ToString());
                    if (token.Type == Antlr4.Runtime.TokenConstants.EOF)
                        break;
                }
                System.Console.Error.WriteLine(new_s.ToString());
            }
            lexer.Reset();
            var tokens = new CommonTokenStream(lexer);
            var parser = new " + parser_name + @"(tokens);
            var listener_lexer = new ErrorListener<int>();
            var listener_parser = new ErrorListener<IToken>();
            lexer.AddErrorListener(listener_lexer);
            parser.AddErrorListener(listener_parser);
            var tree = parser." + startRule + @"();
            if (listener_lexer.had_error || listener_parser.had_error)
            {
                System.Console.Error.WriteLine(""parse failed."");
            }
            else
            {
                System.Console.Error.WriteLine(""parse succeeded."");
            }
            if (show_tree)
            {
                System.Console.Error.WriteLine(tree.ToStringTree());
            }
            System.Environment.Exit(listener_lexer.had_error || listener_parser.had_error ? 1 : 0);
        }
    }
}");
                // Test to find an appropriate file name to place this into.
                string fn = outputDirectory + "Program.cs";
                System.IO.File.WriteAllText(fn, sb.ToString());
            }
            else if (target == "Java")
            {
                sb.Append(@"
// Template generated code from Antlr4BuildTasks.dotnet-antlr v " + version + @"
package " + @namespace + @";

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.util.Arrays;
import java.util.Date;
import java.util.HashMap;
import org.antlr.v4.runtime.ANTLRInputStream;
import org.antlr.v4.runtime.CharStream;
import org.antlr.v4.runtime.CharStreams;
import org.antlr.v4.runtime.CodePointCharStream;
import org.antlr.v4.runtime.CommonToken;
import org.antlr.v4.runtime.CommonTokenStream;
import org.antlr.v4.runtime.IntStream;
import org.antlr.v4.runtime.tree.ParseTree;

public class Program {
    public static void main(String[] args) throws  FileNotFoundException, IOException
    {
        boolean show_tree = false;
        boolean show_tokens = false;
        String input = null;
        for (int i = 0; i < args.length; ++i)
        {
            if (args[i].equals(""-tokens""))
            {
                show_tokens = true;
                continue;
            }
            else if (args[i].equals(""-tree""))
            {
                show_tree = true;
                continue;
            }
            else if (args[i].equals(""-input""))
                input = args[i];
        }
        if (input == null)
        {
            StringBuilder sb = new StringBuilder();
            int ch;
            while ((ch = System.in.read()) != -1)
            {
                sb.append((char)ch);
            }
            input = sb.toString();
        }
        CommonTokenStream tokens = null;
        ANTLRInputStream str = new ANTLRInputStream(input);
        " + lexer_name + " lexer = new " + lexer_name + @"(str);
        if (show_tokens)
        {
            StringBuilder new_s = new StringBuilder();
            for (int i = 0; ; ++i)
            {
                var ro_token = lexer.nextToken();
                var token = (CommonToken)ro_token;
                token.setTokenIndex(i);
                new_s.append(token.toString());
                new_s.append(System.getProperty(""line.separator""));
                if (token.getType() == IntStream.EOF)
                    break;
            }
            System.out.println(new_s.toString());
        }
        lexer.reset();
        tokens = new CommonTokenStream(lexer);
        " + parser_name + " parser = new " + parser_name + @"(tokens);
        ErrorListener lexer_listener = new ErrorListener();
        ErrorListener listener = new ErrorListener();
        parser.removeParseListeners();
        parser.addErrorListener(listener);
        lexer.addErrorListener(lexer_listener);
        ParseTree tree = parser." + startRule + @"();
        if (listener.had_error || lexer_listener.had_error)
            System.out.println(""error in parse."");
        else
            System.out.println(""parse completed."");
        if (show_tree)
        {
            System.out.println(tree.toStringTree());
        }
        java.lang.System.exit(listener.had_error || lexer_listener.had_error ? 1 : 0);
    }
}
");

                // Test to find an appropriate file name to place this into.
                string fn = outputDirectory + "Program.java";
                System.IO.File.WriteAllText(fn, sb.ToString());
            }
        }
    }
}