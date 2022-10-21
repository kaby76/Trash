// Template generated code from trgen <version>

import java.io.FileNotFoundException;
import java.io.IOException;
import org.antlr.v4.runtime.*;
import org.antlr.v4.runtime.tree.ParseTree;
import java.time.Instant;
import java.time.Duration;
import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;

public class Program {

    static int error_code = 0;
    static boolean show_tree = false;
    static boolean show_tokens = false;
    static java.nio.charset.Charset charset = null;

    public static void main(String[] args) throws  FileNotFoundException, IOException
    {
        List\<Boolean> is_fns = new ArrayList\<Boolean>();
        List\<String> inputs = new ArrayList\<String>();
        for (int i = 0; i \< args.length; ++i)
        {
            if (args[i].equals("-tokens"))
            {
                show_tokens = true;
                continue;
            }
            else if (args[i].equals("-tree"))
            {
                show_tree = true;
                continue;
            }
            else if (args[i].equals("-input")) {
		inputs.add(args[++i]);
		is_fns.add(false);
	    }
            else if (args[i].equals("-encoding"))
            {
                charset = java.nio.charset.Charset.forName(args[++i]);
            }
	    else {
		inputs.add(args[i]);
		is_fns.add(true);
	    }
        }
        CharStream str = null;
        if (inputs.size() == 0)
        {
            ParseStdin();
        }
        else
        {
            Instant start = Instant.now();
            for (int f = 0; f \< inputs.size(); ++f)
            {
                if (is_fns.get(f))
                    ParseFilename(inputs.get(f));
                else
                    ParseString(inputs.get(f));
            }
            Instant finish = Instant.now();
            long timeElapsed = Duration.between(start, finish).toMillis();
            System.err.println("Total Time: " + (timeElapsed * 1.0) / 1000.0);
        }
        java.lang.System.exit(error_code);
    }

    static void ParseStdin()throws IOException {
        CharStream str = CharStreams.fromStream(System.in);
        DoParse(str);
    }

    static void ParseString(String input) throws IOException {
        var str = CharStreams.fromString(input);
        DoParse(str);
    }

    static void ParseFilename(String file_name) throws IOException
    {
        CharStream str = null;
        if (charset == null)
            str = CharStreams.fromFileName(file_name);
        else
            str = CharStreams.fromFileName(file_name, charset);
        DoParse(str);
    }

    static void DoParse(CharStream str) {
        <lexer_name> lexer = new <lexer_name>(str);
        if (show_tokens)
        {
            StringBuilder new_s = new StringBuilder();
            for (int i = 0; ; ++i)
            {
                var ro_token = lexer.nextToken();
                var token = (CommonToken)ro_token;
                token.setTokenIndex(i);
                new_s.append(token.toString());
                new_s.append(System.getProperty("line.separator"));
                if (token.getType() == IntStream.EOF)
                    break;
            }
            System.out.println(new_s.toString());
            lexer.reset();
        }
        var tokens = new CommonTokenStream(lexer);
        <parser_name> parser = new <parser_name>(tokens);
        ErrorListener lexer_listener = new ErrorListener();
        ErrorListener listener = new ErrorListener();
        parser.removeErrorListeners();
        lexer.removeErrorListeners();
        parser.addErrorListener(listener);
        lexer.addErrorListener(lexer_listener);
        Instant start = Instant.now();
        ParseTree tree = parser.<start_symbol>();
        Instant finish = Instant.now();
        long timeElapsed = Duration.between(start, finish).toMillis();
        System.err.println("Time: " + (timeElapsed * 1.0) / 1000.0);
        if (listener.had_error || lexer_listener.had_error)
        {
            System.err.println("Parse failed.");
            error_code = 1;
        }
        else
            System.err.println("Parse succeeded.");
        if (show_tree)
        {
            System.out.println(tree.toStringTree(parser));
        }
    }
}
