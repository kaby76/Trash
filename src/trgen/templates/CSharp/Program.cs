// Template generated code from trgen <version>

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

public class Program
{
    public static <parser_name> Parser { get; set; }
    public static ErrorListener\<IToken> ParserErrorListener { get; set; }
    public static Lexer Lexer { get; set; }
    public static ErrorListener\<int> LexerErrorListener { get; set; }
    public static ITokenStream TokenStream { get; set; }
    public static IParseTree Tree { get; set; }
    public static string StartSymbol { get; set; } = "<start_symbol>";
    public static string Input { get; set; }
    public static void SetupParse2(string input, bool quiet = false)
    {
        ICharStream str = new AntlrInputStream(input);
        <if (case_insensitive_type)>
                str = new Antlr4.Runtime.CaseChangingCharStream(str, "<case_insensitive_type>" == "Upper");
        <endif>
                var lexer = new <lexer_name>(str);
        Lexer = lexer;
        var tokens = new CommonTokenStream(lexer);
        TokenStream = tokens;
        var parser = new <parser_name>(tokens);
        Parser = parser;
        var listener_lexer = new ErrorListener\<int>(quiet);
        LexerErrorListener = listener_lexer;
        var listener_parser = new ErrorListener\<IToken>(quiet);
        ParserErrorListener = listener_parser;
        lexer.RemoveErrorListeners();
        parser.RemoveErrorListeners();
        lexer.AddErrorListener(listener_lexer);
        parser.AddErrorListener(listener_parser);
    }

    public static IParseTree Parse2()
    {
        var tree = Parser.<start_symbol>();
        Input = Lexer.InputStream.ToString();
        TokenStream = Parser.TokenStream;
        Tree = tree;
        return tree;
    }

    public static bool AnyErrors()
    {
        return ParserErrorListener.had_error || LexerErrorListener.had_error;
    }

    public static IParseTree Parse(string input)
    {
        ICharStream str = new AntlrInputStream(input);
    <if (case_insensitive_type)>
            str = new Antlr4.Runtime.CaseChangingCharStream(str, "<case_insensitive_type>" == "Upper");
    < endif >
        var lexer = new < lexer_name > (str);
        Lexer = lexer;
        var tokens = new CommonTokenStream(lexer);
        TokenStream = tokens;
        var parser = new <parser_name>(tokens);
        Parser = parser;
        var listener_lexer = new ErrorListener\<int>();
        var listener_parser = new ErrorListener\<IToken>();
        lexer.RemoveErrorListeners();
        parser.RemoveErrorListeners();
        lexer.AddErrorListener(listener_lexer);
        parser.AddErrorListener(listener_parser);
        var tree = parser.<start_symbol>();
        Input = lexer.InputStream.ToString();
        TokenStream = parser.TokenStream;
        Tree = tree;
        return tree;
    }

    static bool show_profile = false;
    static bool show_tree = false;
    static bool show_tokens = false;
    static bool old = false;
    static bool two_byte = false;
    static int exit_code = 0;
    static Encoding encoding = null;

    static void Main(string[] args)
    {
        List\<bool> is_fns = new List\<bool>();
        List\<string> inputs = new List\<string>();
        for (int i = 0; i \< args.Length; ++i)
        {
            if (args[i].Equals("-profile"))
            {
                show_profile = true;
                continue;
            }
            else if (args[i].Equals("-tokens"))
            {
                show_tokens = true;
                continue;
            }
            else if (args[i].Equals("-two-byte"))
            {
                two_byte = true;
                continue;
            }
            else if (args[i].Equals("-old"))
            {
                old = true;
                continue;
            }
            else if (args[i].Equals("-tree"))
            {
                show_tree = true;
                continue;
            }
            else if (args[i].Equals("-input"))
            {
                inputs.Add(args[++i]);
                is_fns.Add(false);
            }
            else if (args[i].Equals("-encoding"))
            {
                ++i;
                encoding = Encoding.GetEncoding(
                    args[i],
                    new EncoderReplacementFallback("(unknown)"),
                    new DecoderReplacementFallback("(error)"));
                if (encoding == null)
                    throw new Exception(@"Unknown encoding. Must be an Internet Assigned Numbers Authority (IANA) code page name. https://www.iana.org/assignments/character-sets/character-sets.xhtml");
            }
            else
            {
                 inputs.Add(args[i]);
                 is_fns.Add(true);
            }
        }
        if (inputs.Count() == 0)
        {
            ParseStdin();
        }
        else
        {
            DateTime before = DateTime.Now;
            for (int f = 0; f \< inputs.Count(); ++f)
            {
                if (is_fns[f])
                    ParseFilename(inputs[f]);
                else
                    ParseString(inputs[f]);
            }
            DateTime after = DateTime.Now;
            System.Console.Error.WriteLine("Total Time: " + (after - before));
        }
        Environment.ExitCode = exit_code;
    }

    static void ParseStdin()
    {
        ICharStream str = null;
        str = CharStreams.fromStream(System.Console.OpenStandardInput());
        DoParse(str);
    }

    static void ParseString(string input)
    {
        System.Console.Error.WriteLine("Input: " + input);
        ICharStream str = null;
        str = CharStreams.fromString(input);
        DoParse(str);
    }

    static void ParseFilename(string input)
    {
        System.Console.Error.WriteLine("File: " + input);
        ICharStream str = null;
        if (two_byte)
            str = new TwoByteCharStream(input);
        else if (old)
        {
            FileStream fs = new FileStream(input, FileMode.Open);
            str = new Antlr4.Runtime.AntlrInputStream(fs);
        }
        else if (encoding == null)
            str = CharStreams.fromPath(input);
        else
            str = CharStreams.fromPath(input, encoding);
        DoParse(str);
    }

    static void DoParse(ICharStream str)
    {
        var lexer = new <lexer_name>(str);
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
            lexer.Reset();
        }
        var tokens = new CommonTokenStream(lexer);
        var parser = new <parser_name>(tokens);
        var listener_lexer = new ErrorListener\<int>();
        var listener_parser = new ErrorListener\<IToken>();
        lexer.RemoveErrorListeners();
        parser.RemoveErrorListeners();
        lexer.AddErrorListener(listener_lexer);
        parser.AddErrorListener(listener_parser);
        DateTime before = DateTime.Now;
        if (show_profile)
        {
                parser.Profile = true;
        }
        var tree = parser.<start_symbol>();
        DateTime after = DateTime.Now;
        if (listener_lexer.had_error || listener_parser.had_error)
        {
            System.Console.Error.WriteLine("Parse failed.");
            exit_code = 1;
        }
        else
        {
            System.Console.Error.WriteLine("Parse succeeded.");
        }
		System.Console.Error.WriteLine("Time: " + (after - before));
        if (show_tree)
        {
            System.Console.Out.WriteLine(tree.ToStringTree(parser));
        }
        if (show_profile)
        {
            System.Console.Out.WriteLine(String.Join(",\n\r", parser.ParseInfo.getDecisionInfo().Select(d => d.ToString())));
        }
    }
}
