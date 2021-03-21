// Template generated code from Antlr4BuildTasks.dotnet-antlr v <version>

#include \<iostream>
#include \<string>
#include \<chrono>
#include \<atomic>
#include "ANTLRInputStream.h"
#include "ErrorListener.h"
<tool_grammar_tuples:{x | #include "<x.GeneratedIncludeFileName>"
} >

std::string formatDuration(uint64_t duration) {
	std::stringstream oss;

	oss \<\< std::setfill('0')
			\<\< (duration % 60000000000) / 60000000
			\<\< ":"
			\<\< std::setw(2)
			\<\< (duration % 60000000) / 1000000
			\<\< "."
			\<\< std::setw(3)
			\<\< (duration % 1000000) / 1000
			\<\< "."
			\<\< std::setw(3)
			\<\< duration % 1000;

	return oss.str();
}

int TryParse(std::vector\<std::string>& args)
{
    bool show_tree = false;
    bool show_tokens = false;
    std::string * file_name = nullptr;
    std::string * input = nullptr;
    for (int i = 0; i \< args.size(); ++i)
    {
        if (args[i] == "-tokens")
        {
            show_tokens = true;
            continue;
        }
        else if (args[i] == "-tree")
        {
            show_tree = true;
            continue;
        }
        else if (args[i] == "-input")
            input = & args[++i];
        else if (args[i] == "-file")
            file_name = & args[++i];
    }
    antlr4::CharStream* str = nullptr;
    if (input == nullptr && file_name == nullptr)
    {
        str = new antlr4::ANTLRInputStream(std::cin);
    } else if (input != nullptr)
    {
      //  str = CharStreams.fromString(input);
    } else if (file_name != nullptr)
    {
        str = new antlr4::ANTLRInputStream(*file_name);
    }
    antlr4::Lexer * lexer = new <lexer_name>(str);
//    if (show_tokens)
//    {
//        StringBuilder new_s = new StringBuilder();
//        for (int i = 0; ; ++i)
//        {
//            var ro_token = lexer.NextToken();
//            var token = (CommonToken)ro_token;
//            token.TokenIndex = i;
//            new_s.AppendLine(token.ToString());
//            if (token.Type == Antlr4.Runtime.TokenConstants.EOF)
//                break;
//        }
//        System.Console.Error.WriteLine(new_s.ToString());
//        lexer.Reset();
//    }
    auto tokens = new antlr4::CommonTokenStream(lexer);
    auto * parser = new <parser_name>(tokens);
    auto listener_lexer = new ErrorListener();
    auto listener_parser = new ErrorListener();
    lexer->addErrorListener(listener_lexer);
    parser->addErrorListener(listener_parser);
    auto before = std::chrono::steady_clock::now();
    auto tree = parser-><start_symbol>();
    auto after = std::chrono::steady_clock::now();
    auto duration = std::chrono::duration_cast\<std::chrono::microseconds>(after - before);
    if (listener_parser->had_error || listener_lexer->had_error)
    {
        std::cout \<\< "Parse failed." \<\< std::endl;
    }
    else
    {
        std::cout \<\< "Parse succeeded." \<\< std::endl;
    }
    if (show_tree)
    {
//        System.Console.Error.WriteLine(tree.ToStringTree(parser));
    }
    std::cout \<\< "Time: " \<\< formatDuration(duration.count()) \<\< std::endl;
}

int main(int argc, const char * argv[])
{
    std::vector \<std::string> args;
    for (int i = 1; i \< argc; ++i)
    {
        args.push_back(argv[i]);
    }   
    TryParse(args);
}

