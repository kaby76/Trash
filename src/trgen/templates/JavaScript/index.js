// Template generated code from trgen <version>

import antlr4 from 'antlr4';
<tool_grammar_tuples: {x | import <x.GrammarAutomName> from './<x.GeneratedFileName>';
} >
import strops from 'typescript-string-operations';
import fs from 'fs-extra';
import pkg from 'timer-node';
const { Timer, Time, TimerOptions } = pkg;

function getChar() {
    let buffer = Buffer.alloc(1);
    var xx = fs.readSync(0, buffer, 0, 1);
    if (xx === 0) {
        return '';
    }
    return buffer.toString('utf8');
}

var num_errors = 0;

class MyErrorListener extends antlr4.error.ErrorListener {
    syntaxError(recognizer, offendingSymbol, line, column, msg, err) {
        num_errors++;
        console.error(`${offendingSymbol} line ${line}, col ${column}: ${msg}`);
    }
}

function ParseStdin() {
    var sb = new strops.StringBuilder();
    var ch;
    while ((ch = getChar()) != '') {
        sb.Append(ch);
    }
    var input = sb.ToString();
    var str = antlr4.CharStreams.fromString(input);
    DoParse(str);
}

function ParseString(input) {
    var str = antlr4.CharStreams.fromString(input);
    DoParse(str);
}

function ParseFilename(input) {
    var str = antlr4.CharStreams.fromPathSync(input, encoding);
    DoParse(str);
}

var show_tokens = false;
var show_tree = false;
var inputs = [];
var is_fns = [];
var error_code = 0;
var encoding = 'utf8';

function main() {
    for (let i = 2; i \< process.argv.length; ++i)
    {
        switch (process.argv[i]) {
            case '-tokens':
                show_tokens = true;
                break;
            case '-tree':
                show_tree = true;
                break;
            case '-encoding':
                encoding = process.argv[++i];
                break;
            case '-input':
                inputs.push(process.argv[++i]);
                is_fns.push(false);
                break;
            default:
                inputs.push(process.argv[i]);
                is_fns.push(true);
                break;
        }
    }
    if (inputs.length == 0) {
        ParseStdin();
    }
    else {
        const timer = new Timer({ label: 'test-timer' });
        timer.start();
        for (var f = 0; f \< inputs.length; ++f)
        {
            if (is_fns[f])
                ParseFilename(inputs[f]);
            else
                ParseString(inputs[f]);
        }
        timer.stop();
        console.error(timer.format('Total Time: %mm %ss %msms'));
    }
    process.exitCode = error_code;
}

function DoParse(str) {
    const lexer = new <lexer_name>(str);
    lexer.strictMode = false;
    const tokens = new antlr4.CommonTokenStream(lexer);
    const parser = new <parser_name>(tokens);
    lexer.removeErrorListeners();
    parser.removeErrorListeners();
    parser.addErrorListener(new MyErrorListener());
    lexer.addErrorListener(new MyErrorListener());
    if (show_tokens) {
        for (var i = 0; ; ++i) {
            var ro_token = lexer.nextToken();
            var token = ro_token;
            token.TokenIndex = i;
            console.log(token.toString());
            if (token.type === antlr4.Token.EOF)
                break;
        }
        lexer.reset();
    }
    const timer = new Timer({ label: 'test-timer2' });
    timer.start();
    const tree = parser.< start_symbol > ();
    timer.stop();
    if (show_tree) {
        console.log(tree.toStringTree(parser.ruleNames));
    }
    if (num_errors > 0) {
        console.error('Parse failed.');
        error_code = 1;
    }
    else {
        console.error('Parse succeeded.');
    }
    console.error(timer.format('Time: %mm %ss %msms'));
}


main()
