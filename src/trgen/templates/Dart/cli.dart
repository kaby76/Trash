// Template generated code from trgen <version>

import 'package:antlr4/antlr4.dart';
<tool_grammar_tuples:{x | import '<x.GeneratedFileName>';
} >
import 'dart:io';
import 'dart:convert';

var show_tree = false;
var show_tokens = false;
var inputs = List\<String>.empty(growable: true);
var is_fns = List\<bool>.empty(growable: true);
var error_code = 0;

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
        else if (args[i] == "-input")
        {
            inputs.add(args[++i]);
            is_fns.add(false);
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
                await ParseFilename(inputs[f]);
            else
                await ParseString(inputs[f]);
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
    await DoParse(str);
}

Future\<void> ParseString(String input) async
{
    var str = await InputStream.fromString(input);
    await DoParse(str);
}

Future\<void> ParseFilename(String file_name) async
{
    var str = await InputStream.fromPath(file_name);
    await DoParse(str);
}

Future\<void> DoParse(CharStream str) async
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
    Stopwatch s = new Stopwatch();
    s.start();
    var tree = parser.<start_symbol>();
    s.stop();
    var et = s.elapsedMilliseconds / 1000.0;
    stderr.writeln("Time: " + et.toString());
    if (parser.numberOfSyntaxErrors > 0)
    {
        stderr.writeln("Parse failed.");
        error_code = 1;
    }
    else
    {
        stderr.writeln("Parse succeeded.");
    }
    if (show_tree)
    {
        print(tree.toStringTree(parser: parser));
    }
}
