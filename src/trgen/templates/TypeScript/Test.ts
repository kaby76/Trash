// Generated from trgen 0.19.3

import { CharStream, CommonTokenStream }  from 'antlr4';
<tool_grammar_tuples: {x | import <x.GrammarAutomName> from './<x.GrammarAutomName>';
} >
import { StringBuilder, emptyString, joinString, formatString, isNullOrWhiteSpace } from 'typescript-string-operations';

const input = "1+2"
const chars = new CharStream(input); // replace this with a FileStream as required
const lexer = new ArithmeticLexer(chars);
const tokens = new CommonTokenStream(lexer);
const parser = new ArithmeticParser(tokens);
const tree = parser.file_();
console.log(tree.toStringTree(null,parser));