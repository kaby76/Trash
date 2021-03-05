using System;
using System.IO;
using System.Text;

namespace dotnet_antlr
{
    public class GenMain
    {
        public static void AddMain(Program p)
        {
            StringBuilder sb = new StringBuilder();
            if (p.target == Program.TargetType.CSharp)
            {
                sb.AppendLine(@"// Template generated code from Antlr4BuildTasks.dotnet-antlr v " + Program.version);
                if (p.@namespace != null) sb.AppendLine("namespace " + p.@namespace + @"
{");
                sb.Append(@"
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

public class Program
{
    public static Parser Parser { get; set; }
    public static Lexer Lexer { get; set; }
    public static ITokenStream TokenStream { get; set; }
    public static IParseTree Tree { get; set; }
    public static IParseTree Parse(string input)
    {
        var str = new AntlrInputStream(input);
        var lexer = new ");
                sb.Append(p.lexer_name);
                sb.Append(@"(str);
        Lexer = lexer;
        var tokens = new CommonTokenStream(lexer);
        TokenStream = tokens;
        var parser = new ");
                sb.Append(p.parser_name);
                sb.Append(@"(tokens);
        Parser = parser;
        var tree = parser." + p.startRule);
                sb.AppendLine(@"();
        Tree = tree;
        return tree;
    }

    static void Main(string[] args)
    {
        bool show_tree = false;
        bool show_tokens = false;
        string file_name = null;
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
                input = args[++i];
            else if (args[i].Equals(""-file""))
                file_name = args[++i];
        }
        ICharStream str = null;
        if (input == null && file_name == null)
        {
            StringBuilder sb = new StringBuilder();
            int ch;
            while ((ch = System.Console.Read()) != -1)
            {
                sb.Append((char)ch);
            }
            input = sb.ToString();
            ");
                if (!p.antlr4cs)
                    sb.Append(
            @"str = CharStreams.fromString(input);");
                else
                    sb.Append(
            @"str = new Antlr4.Runtime.AntlrInputStream(
                    new MemoryStream(Encoding.UTF8.GetBytes(input ?? """")));");
                sb.Append(@"
        } else if (input != null)
        {
            ");
                if (!p.antlr4cs)
                    sb.Append(
            @"str = CharStreams.fromString(input);");
                else
                    sb.Append(
            @"str = new Antlr4.Runtime.AntlrInputStream(
                    new MemoryStream(Encoding.UTF8.GetBytes(input ?? """")));");
                sb.Append(@"
        } else if (file_name != null)
        {
            ");
                if (!p.antlr4cs)
                    sb.Append(
            @"str = CharStreams.fromPath(file_name);");
                else
                    sb.Append(
            @"FileStream fs = new FileStream(file_name, FileMode.Open);
                str = new Antlr4.Runtime.AntlrInputStream(fs);");
                sb.Append(@"
        }
        ");
                if (p.case_fold != null)
                {
                    sb.Append(@"str = new CaseChangingCharStream(str, "
                        + ((bool)p.case_fold ? "true" : "false") + @");
        ");
                }
                sb.Append(@"var lexer = new " + p.lexer_name + @"(str);
        if (show_tokens)
        {
            StringBuilder new_s = new StringBuilder();
            for (int i = 0; ; ++i)
            {
                var ro_token = lexer.NextToken();
                var token = (CommonToken)ro_token;
                token.TokenIndex = i;
                new_s.AppendLine(token.ToString());
                if (token.Type == Antlr4.Runtime.TokenConstants." + (p.antlr4cs ? "Eof" : "EOF") + @")
                    break;
            }
            System.Console.Error.WriteLine(new_s.ToString());
            lexer.Reset();
        }
        var tokens = new CommonTokenStream(lexer);
        var parser = new " + p.parser_name + @"(tokens);
        ");
                if (!p.antlr4cs)
                {
                    sb.Append(@"var listener_lexer = new ErrorListener<int>();
        var listener_parser = new ErrorListener<IToken>();
        lexer.AddErrorListener(listener_lexer);
        parser.AddErrorListener(listener_parser);");
                    if (p.profiling)
                    {
                        sb.Append(@"
        parser.Profile = true;");
                    }
                    sb.Append(@"
        var tree = parser." + p.startRule + @"();
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
            System.Console.Error.WriteLine(tree.ToStringTree(parser));
        }");
                    if (p.profiling)
                    {
                        sb.Append(@"System.Console.Out.WriteLine(String.Join("", "", parser.ParseInfo.getDecisionInfo().Select(d => d.ToString())));
        ");
                    }
                    sb.Append(@"
        System.Environment.Exit(listener_lexer.had_error || listener_parser.had_error ? 1 : 0);");
                }
                else
                {
                    if (p.profiling)
                    {
                        sb.Append(@"
        parser.Profile = true;");
                    }
                    sb.Append(@"
        var tree = parser." + p.startRule + @"();
        if (parser.NumberOfSyntaxErrors != 0)
        {
            System.Console.Error.WriteLine(""parse failed."");
        }
        else
        {
            System.Console.Error.WriteLine(""parse succeeded."");
        }
        if (show_tree)
        {
            System.Console.Error.WriteLine(tree.ToStringTree(parser));
        }
");
                    if (p.profiling)
                    {
                        sb.Append(@"System.Console.Out.WriteLine(String.Join("", "", parser.ParseInfo.getDecisionInfo().Select(d => d.ToString())));
        ");
                    }
                    sb.Append(@"
        System.Environment.Exit(parser.NumberOfSyntaxErrors != 0 ? 1 : 0);");
                }
                sb.Append(@"
    }
}
");
                if (p.@namespace != null) sb.AppendLine("}");
                // Test to find an appropriate file name to place this into.
                string fn = p.outputDirectory + "Program.cs";
                System.IO.File.WriteAllText(fn, Program.Localize(p.line_translation, sb.ToString()));
            }
            else if (p.target == Program.TargetType.Java)
            {
                sb.AppendLine(@"// Template generated code from Antlr4BuildTasks.dotnet-antlr v " + Program.version);
                if (p.@namespace != null) sb.AppendLine("package " + p.@namespace + @";");
                sb.Append(@"

import java.io.FileNotFoundException;
import java.io.IOException;
import org.antlr.v4.runtime.*;
import org.antlr.v4.runtime.tree.ParseTree;

public class Program {
    public static void main(String[] args) throws  FileNotFoundException, IOException
    {
        boolean show_tree = false;
        boolean show_tokens = false;
        String file_name = null;
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
                input = args[++i];
            else if (args[i].equals(""-file""))
                file_name = args[++i];
        }
        CharStream str = null;
        if (input == null && file_name == null)
        {
            str = CharStreams.fromStream(System.in);
        } else if (input != null)
        {
            str = CharStreams.fromString(input);
        } else if (file_name != null)
        {
            str = CharStreams.fromFileName(file_name);
        }
        " + p.lexer_name + " lexer = new " + p.lexer_name + @"(str);
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
            lexer.reset();
        }
        var tokens = new CommonTokenStream(lexer);
        " + p.parser_name + " parser = new " + p.parser_name + @"(tokens);
        ErrorListener lexer_listener = new ErrorListener();
        ErrorListener listener = new ErrorListener();
        parser.removeParseListeners();
        parser.addErrorListener(listener);
        lexer.addErrorListener(lexer_listener);");
                if (p.profiling)
                {
                    sb.Append(@"
        parser.setProfile(true);");
                }
                sb.Append(@"
        ParseTree tree = parser." + p.startRule + @"();
        if (listener.had_error || lexer_listener.had_error)
            System.out.println(""error in parse."");
        else
            System.out.println(""parse completed."");
        if (show_tree)
        {
            System.out.println(tree.toStringTree(parser));
        }
        ");
                if (p.profiling)
                {
                    sb.Append(@"System.out.print(string.join("", "", parser.getParseInfo().getDecisionInfo())));
        ");
                }
                sb.Append(@"java.lang.System.exit(listener.had_error || lexer_listener.had_error ? 1 : 0);
    }
}
");

                // Java code has to go into the directory corresponding to the namespace.
                // Test to find an appropriate file name to place this into.
                var q = p.@namespace != null ? p.@namespace.Replace(".", "/") + "/" : "";
                string fn = p.outputDirectory + q + "Program.java";
                System.IO.File.WriteAllText(fn, Program.Localize(p.line_translation, sb.ToString()));
            }
            else if (p.target == Program.TargetType.JavaScript)
            {
                sb.AppendLine(@"// Template generated code from Antlr4BuildTasks.dotnet-antlr v " + Program.version);
                sb.Append(@"
import { createRequire } from 'module';
const require = createRequire(import.meta.url);
const antlr4 = require('antlr4');
import " + p.lexer_name + @" from './" + p.lexer_name + @".js';
import " + p.parser_name + @" from './" + p.parser_name + @".js';
const strops = require('typescript-string-operations');
let fs = require('fs-extra')

function getChar() {
	let buffer = Buffer.alloc(1);
	var xx = fs.readSync(0, buffer, 0, 1);
	if (xx === 0) {
		return '';
	}
    return buffer.toString('utf8');
}

class MyErrorListener extends antlr4.error.ErrorListener {
	syntaxError(recognizer, offendingSymbol, line, column, msg, err) {
		num_errors++;
		console.error(`${offendingSymbol} line ${line}, col ${column}: ${msg}`);
	}
}

var show_tokens = false;
var show_tree = false;
var input = null;
var file_name = null;
for (let i = 2; i < process.argv.length; ++i)
{
    switch (process.argv[i]) {
        case '-tokens':
            var show_tokens = true;
            break;
        case '-tree':
            var show_tree = true;
            break;
        case '-input':
            var input = process.argv[++i];
            break;
        case '-file':
            var file_name = process.argv[++i];
            break;
        default:
            console.log('unknown '.concat(process.argv[i]));
    }
}
var str = null;
if (input == null && file_name == null)
{
    var sb = new strops.StringBuilder();
    var ch;
    while ((ch = getChar()) != '')
    {
        sb.Append(ch);
    }
    var input = sb.ToString();
    str = antlr4.CharStreams.fromString(input);
} else if (input != null)
{
    str = antlr4.CharStreams.fromString(input);
} else if (file_name != null)
{
    str = antlr4.CharStreams.fromPathSync(file_name, 'utf8');
}
var num_errors = 0;
const lexer = new " + p.lexer_name + @"(str);
lexer.strictMode = false;
const tokens = new antlr4.CommonTokenStream(lexer);
const parser = new " + p.parser_name + @"(tokens);
lexer.removeErrorListeners();
parser.removeErrorListeners();
parser.addErrorListener(new MyErrorListener());
lexer.addErrorListener(new MyErrorListener());
if (show_tokens)
{
    for (var i = 0; ; ++i)
    {
        var ro_token = lexer.nextToken();
        var token = ro_token;
        token.TokenIndex = i;
        console.log(token.toString());
        if (token.type === antlr4.Token.EOF)
            break;
    }
    lexer.reset();
}
const tree = parser." + p.startRule + @"();
if (show_tree)
{
    console.log(tree.toStringTree(parser.ruleNames));
}
if (num_errors > 0)
{
    console.log('error in parse.');
    process.exitCode = 1;
}
else
{
    console.log('parse completed.');
    process.exitCode = 0;
}
");

                // Test to find an appropriate file name to place this into.
                string fn = p.outputDirectory + "index.js";
                System.IO.File.WriteAllText(fn, Program.Localize(p.line_translation, sb.ToString()));
            }
            else if (p.target == Program.TargetType.Python3)
            {
                sb.AppendLine(@"# Template generated code from Antlr4BuildTasks.dotnet-antlr v " + Program.version);
                sb.Append(@"
import sys
from antlr4 import *
from antlr4.error.ErrorListener import ErrorListener
from readchar import readchar
from " + p.lexer_name + @" import " + p.lexer_name + @";
from " + p.parser_name + @" import " + p.parser_name + @";

def getChar():
    xx = readchar()
    if (xx == 0):
        return '';
    return xx

class MyErrorListener(ErrorListener):
    __slots__ = 'num_errors'

    def __init__(self):
        super().__init__()
        self.num_errors = 0

    def syntaxError(self, recognizer, offendingSymbol, line, column, msg, e):
        self.num_errors = self.num_errors + 1
        super().syntaxError(recognizer, offendingSymbol, line, column, msg, e)

def main(argv):
    show_tokens = False
    show_tree = False
    input = None
    file_name = None
    i = 1
    while i < len(argv):
        arg = argv[i]
        if arg in (""-tokens""):
            show_tokens = True
        elif arg in (""-tree""):
            show_tree = True
        elif arg in (""-input""):
            i = i + 1
            input = argv[i]
        elif arg in (""-file""):
            i = i + 1
            file_name = argv[i]
        else:
            print(""unknown"")
        i = i + 1

    if (input == None and file_name == None):
        sb = """"
        ch = getChar()
        while (ch != ''):
            sb = sb + ch
            ch = getChar()
        input = sb
        str = InputStream(input);
    elif (input != None):
        str = InputStream(input);
    elif (file_name != None):
        str = FileStream(file_name, 'utf8');

    lexer = ArithmeticLexer(str)
    lexer = " + p.lexer_name + @"(str);
    lexer.removeErrorListeners()
    l_listener = MyErrorListener()
    lexer.addErrorListener(l_listener)
    # lexer.strictMode = false
    tokens = CommonTokenStream(lexer)
    parser = " + p.parser_name + @"(tokens)
    parser.removeErrorListeners()
    p_listener = MyErrorListener()
    parser.addErrorListener(p_listener)
    if (show_tokens):
        i = 0
        while True:
            ro_token = lexer.nextToken()
            token = ro_token
            # token.TokenIndex = i
            i = i + 1
            print(token)
            if (token.type == -1):
                break
        lexer.reset()
    tree = parser.file_()
    if (show_tree):
        print(tree.toStringTree(recog=parser))
    if p_listener.num_errors > 0 or l_listener.num_errors > 0:
        print('error in parse.');
        sys.exit(1)
    else:
        print('parse completed.');
        sys.exit(0)

if __name__ == '__main__':
    main(sys.argv)
");

                // Test to find an appropriate file name to place this into.
                string fn = p.outputDirectory + "Program.py";
                System.IO.File.WriteAllText(fn, Program.Localize(p.line_translation, sb.ToString()));
            }
            else if (p.target == Program.TargetType.Dart)
            {
                sb.AppendLine(@"// Template generated code from Antlr4BuildTasks.dotnet-antlr v " + Program.version);
                sb.Append(@"
import 'package:antlr4/antlr4.dart';
import '" + p.parser_generated_file_name + @"';
import '" + p.lexer_generated_file_name + @"';
import 'dart:io';
import 'dart:convert';

void main(List<String> args) async {
    var show_tree = false;
    var show_tokens = false;
    var file_name = null;
    var input = null;
    var str = null;
    for (int i = 0; i < args.length; ++i)
    {
        if (args[i] == ""-tokens"")
        {
            show_tokens = true;
            continue;
        }
        else if (args[i] == ""-tree"")
        {
            show_tree = true;
            continue;
        }
        else if (args[i] == ""-input"")
            input = args[++i];
        else if (args[i] == ""-file"")
            file_name = args[++i];
    }
    " + p.parser_name + @".checkVersion();
    " + p.lexer_name + @".checkVersion();
    if (input == null && file_name == null)
    {
	    final List<int> bytes = <int>[];
	    int byte = stdin.readByteSync();
	    while (byte >= 0) {
		    bytes.add(byte);
		    byte = stdin.readByteSync();
	    }
	    input = utf8.decode(bytes);
        str = await InputStream.fromString(input);
    } else if (input != null)
    {
        str = await InputStream.fromString(input);
    } else if (file_name != null)
    {
        str = await InputStream.fromPath(file_name);        
    }
    var lexer = " + p.lexer_name + @"(str);
    if (show_tokens)
    {
        for (int i = 0; ; ++i)
        {
            var token = lexer.nextToken();
	    print(token.toString());
            if (token.type == -1)
                break;
        }
        lexer.reset();
    }
    var tokens = CommonTokenStream(lexer);
    var parser = " + p.parser_name + @"(tokens);
//    var listener_lexer = ErrorListener();
//    var listener_parser = ErrorListener();
//    lexer.AddErrorListener(listener_lexer);
//    parser.AddErrorListener(listener_parser);
    var tree = parser." + p.startRule + @"();
    if (parser.numberOfSyntaxErrors > 0)
    {
        print(""parse failed."");
    }
    else
    {
        print(""parse succeeded."");
    }
    if (show_tree)
    {
        print(tree.toStringTree(parser: parser));
    }
    exit(parser.numberOfSyntaxErrors > 0 ? 1 : 0);
}
");

                // Test to find an appropriate file name to place this into.
                string fn = p.outputDirectory + "cli.dart";
                System.IO.File.WriteAllText(fn, Program.Localize(p.line_translation, sb.ToString()));
            }
            else if (p.target == Program.TargetType.Go)
            {
                sb.AppendLine(@"// Template generated code from Antlr4BuildTasks.dotnet-antlr v " + Program.version);
                sb.Append(@"
package main
import (
	""fmt""
	""os""
    ""io""
    ""github.com/antlr/antlr4/runtime/Go/antlr""
    ""./parser""
)
type CustomErrorListener struct {
	errors int
}

func NewCustomErrorListener() *CustomErrorListener {
	return new(CustomErrorListener)
}

func (l *CustomErrorListener) SyntaxError(recognizer antlr.Recognizer, offendingSymbol interface{}, line, column int, msg string, e antlr.RecognitionException) {
	l.errors += 1
	antlr.ConsoleErrorListenerINSTANCE.SyntaxError(recognizer, offendingSymbol, line, column, msg, e)
}

func (l *CustomErrorListener) ReportAmbiguity(recognizer antlr.Parser, dfa *antlr.DFA, startIndex, stopIndex int, exact bool, ambigAlts *antlr.BitSet, configs antlr.ATNConfigSet) {
	l.errors += 1
}

func (l *CustomErrorListener) ReportAttemptingFullContext(recognizer antlr.Parser, dfa *antlr.DFA, startIndex, stopIndex int, conflictingAlts *antlr.BitSet, configs antlr.ATNConfigSet) {
	l.errors += 1
}

func (l *CustomErrorListener) ReportContextSensitivity(recognizer antlr.Parser, dfa *antlr.DFA, startIndex, stopIndex, prediction int, configs antlr.ATNConfigSet) {
	l.errors += 1
}


func main() {
    var show_tree = false
    var show_tokens = false
    var file_name = """"
    var input = """"
	var str antlr.CharStream = nil
	for i := 0; i < len(os.Args); i = i + 1 {
        if os.Args[i] == ""-tokens"" {
            show_tokens = true
            continue
        } else if os.Args[i] == ""-tree"" {
            show_tree = true
            continue
        } else if os.Args[i] == ""-input"" {
			i = i + 1
			input = os.Args[i]
        } else if os.Args[i] == ""-file"" {
			i = i + 1
			file_name = os.Args[i]
		}
    }
    if input == """" && file_name == """" {
        var b []byte = make([]byte, 1)
        var st = """"
	    for {
		    _, err := os.Stdin.Read(b)
            if err == io.EOF {
                break
            }
		    st = st + string(b)
        }
        str = antlr.NewInputStream(st)
    } else if input != """" {
        str = antlr.NewInputStream(input)
    } else if file_name != """" {
        str, _ = antlr.NewFileStream(file_name);        
    }
    var lexer = parser.New" + p.lexer_name + @"(str);
    if show_tokens {
		j := 0
	    for {
		    t := lexer.NextToken()
			// missing ToString() of all types.
			fmt.Print(j)
			fmt.Print("" "")
			//	    fmt.Print(t.String())
			fmt.Print("" "")
			fmt.Println(t.GetText())
			if t.GetTokenType() == antlr.TokenEOF {
				break
			}
			j = j + 1
	    }
        // missing
        // lexer.reset()
    }
	// Requires additional 0??
    var tokens = antlr.NewCommonTokenStream(lexer, 0)
    var parser = parser.New" + p.parser_name + @"(tokens)

	lexerErrors := &CustomErrorListener{}
	lexer.RemoveErrorListeners()
	lexer.AddErrorListener(lexerErrors)

	parserErrors := &CustomErrorListener{}
	parser.RemoveErrorListeners()
	parser.AddErrorListener(parserErrors)

	// mutated name--not lowercase.
    var tree = parser." + Cap(p.startRule) + @"()
	// missing
    if parserErrors.errors > 0 || lexerErrors.errors > 0 {
        fmt.Println(""parse failed."");
    } else {
        fmt.Println(""parse succeeded."")
    }
    if show_tree {
		ss := tree.ToStringTree(parser.RuleNames, parser)
		fmt.Println(ss)
    }
    if parserErrors.errors > 0 || lexerErrors.errors > 0 {
        os.Exit(1)
    } else {
        os.Exit(0)
    }
}
");

                // Test to find an appropriate file name to place this into.
                string fn = p.outputDirectory + "Program.go";
                System.IO.File.WriteAllText(fn, Program.Localize(p.line_translation, sb.ToString()));
            }
        }

        static string Cap(string str)
        {
            if (str.Length == 0)
                return str;
            else if (str.Length == 1)
                return char.ToUpper(str[0]).ToString();
            else
                return char.ToUpper(str[0]) + str.Substring(1);
        }
    }
}
