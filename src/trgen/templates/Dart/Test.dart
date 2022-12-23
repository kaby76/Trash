// Template generated code from trgen <version>

import 'package:antlr4/antlr4.dart';
<tool_grammar_tuples:{x | import '<x.GeneratedFileName>';
} >
import 'dart:io';
import 'dart:convert';

var show_tree = false;
var show_tokens = false;
var show_trace = false;
var inputs = List\<String>.empty(growable: true);
var is_fns = List\<bool>.empty(growable: true);
var error_code = 0;
var string_instance = 0;
var prefix = "";

void main(List\<String> args) async {
    for (int i = 0; i \< args.length; ++i)
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
        else if (args[i] == "-prefix")
        {
            prefix = args[++i] + " ";
        }
        else if (args[i] == "-input")
        {
            inputs.add(args[++i]);
            is_fns.add(false);
        }
        else if (args[i] == "-trace")
        {
            show_trace = true;
            continue;
        }
        else
        {
            inputs.add(args[i]);
            is_fns.add(true);
        }
    }
    if (inputs.length == 0)
    {
        await ParseStdin();
    }
    else
    {
        Stopwatch s = new Stopwatch();
        s.start();
        for (int f = 0; f \< inputs.length; ++f)
        {
            if (is_fns[f])
                await ParseFilename(inputs[f], f);
            else
                await ParseString(inputs[f], f);
        }
        s.stop();
        var et = s.elapsedMilliseconds / 1000.0;
        stderr.writeln("Total Time: " + et.toString());
    }
    exit(error_code);
}

Future\<void> ParseStdin() async
{
    final List\<int> bytes = \<int>[];
    int byte = stdin.readByteSync();
    while (byte >= 0) {
        bytes.add(byte);
        byte = stdin.readByteSync();
    }
    var input = utf8.decode(bytes);
    var str = await InputStream.fromString(input);
    await DoParse(str, "stdin", 0);
}

Future\<void> ParseString(String input, int row_number) async
{
    var str = await InputStream.fromString(input);
    await DoParse(str, "string" + string_instance.toString(), row_number);
    string_instance++;
}

Future\<void> ParseFilename(String input, int row_number) async
{
    var str = await InputStream.fromPath(input);
    await DoParse(str, input, row_number);
}

Future\<void> DoParse(CharStream str, String input_name, int row_number) async
{
    <tool_grammar_tuples:{x|<x.GrammarAutomName>.checkVersion();
    }>
    var lexer = <lexer_name>(str);
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
    var parser = <parser_name>(tokens);
//    var listener_lexer = ErrorListener();
//    var listener_parser = ErrorListener();
//    lexer.AddErrorListener(listener_lexer);
//    parser.AddErrorListener(listener_parser);
    if (show_trace)
    {
        parser.setTrace(true);
        // Missing ATN trace.
    }
    Stopwatch s = new Stopwatch();
    s.start();
    var tree = parser.<start_symbol>();
    s.stop();
    var et = s.elapsedMilliseconds / 1000.0;
    var result = "";
    if (parser.numberOfSyntaxErrors > 0)
    {
        result = "fail";
        error_code = 1;
    }
    else
    {
        result = "success";
    }
    if (show_tree)
    {
        print(tree.toStringTree(parser: parser));
    }
    stderr.writeln(prefix + "Dart " + row_number.toString() + " " + input_name + " " + result + " " + et.toString());
}
