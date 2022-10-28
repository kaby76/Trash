// Template generated code from trgen <version>

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

<if(has_name_space)>namespace <name_space>
{<endif>

public class Program
{
    public static Parser Parser { get; set; }
    public static Lexer Lexer { get; set; }
    public static ITokenStream TokenStream { get; set; }
    public static IParseTree Tree { get; set; }
    public static string StartSymbol { get; set; } = "<start_symbol>";
    public static string Input { get; set; }

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
        StringBuilder sb = new StringBuilder();
        int ch;
        while ((ch = System.Console.Read()) != -1)
        {
            sb.Append((char)ch);
        }
        input = sb.ToString();
        str = new Antlr4.Runtime.AntlrInputStream(
            new MemoryStream(Encoding.UTF8.GetBytes(input ?? "")));
        DoParse(str);
    }

    static void ParseString(string input)
    {
        System.Console.Error.WriteLine("Input: " + input);
        ICharStream str = null;
        str = new Antlr4.Runtime.AntlrInputStream(
            new MemoryStream(Encoding.UTF8.GetBytes(input ?? "")));
        DoParse(str);
    }

    static void ParseFilename(string input)
    {
        System.Console.Error.WriteLine("File: " + input);
        ICharStream str = null;
        FileStream fs = new FileStream(input, FileMode.Open);
        str = new Antlr4.Runtime.AntlrInputStream(fs);
        DoParse(str);
    }

    static void DoParse(ICharStream str)
    {
<if (case_insensitive_type)>
        str = new Antlr4.Runtime.CaseChangingCharStream(str, "<case_insensitive_type>" == "Upper");
< endif >
        var lexer = new Test.<lexer_name>(str);
        if (show_tokens)
        {
            StringBuilder new_s = new StringBuilder();
            for (int i = 0; ; ++i)
            {
                var ro_token = lexer.NextToken();
                var token = (CommonToken)ro_token;
                token.TokenIndex = i;
                new_s.AppendLine(token.ToString());
                if (token.Type == Antlr4.Runtime.TokenConstants.Eof)
                    break;
            }
            System.Console.Error.WriteLine(new_s.ToString());
            lexer.Reset();
        }
        var tokens = new CommonTokenStream(lexer);
        var parser = new Test.<parser_name>(tokens);
        DateTime before = DateTime.Now;
        var tree = parser.<start_symbol>();
        DateTime after = DateTime.Now;
        if (parser.NumberOfSyntaxErrors > 0)
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
    }
}

<if(has_name_space)>}<endif>
