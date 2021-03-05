using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace dotnet_antlr
{
    class GenGrammars
    {
        public static void AddGrammars(Program p)
        {
            if (p.tool_src_grammar_files.Any() && p.tool_src_grammar_files.First() != "Arithmetic.g4")
            {
                if (p.target == Program.TargetType.Java)
                {
                    var cd = Environment.CurrentDirectory + "/";
                    var set = new HashSet<string>();
                    foreach (var path in p.tool_src_grammar_files)
                    {
                        // Construct proper starting directory based on namespace.
                        var f = path.Replace('\\', '/');
                        var c = cd.Replace('\\', '/');
                        var e = f.Replace(c, "");
                        var m = Path.GetFileName(f);
                        var n = (p.@namespace != null ? p.@namespace.Replace('.', '/') + '/' : "") + m;
                        p.CopyFile(path, p.outputDirectory.Replace('\\', '/') + n);
                    }
                    foreach (var path in p.additional_grammar_files)
                    {
                        // Construct proper starting directory based on namespace.
                        var f = path.Replace('\\', '/');
                        var c = cd.Replace('\\', '/');
                        var e = f.Replace(c, "");
                        var m = Path.GetFileName(f);
                        var n = (p.@namespace != null ? p.@namespace.Replace('.', '/') + '/' : "") + m;
                        p.CopyFile(path, p.outputDirectory.Replace('\\', '/') + n);
                    }
                }
                else
                {
                    foreach (var g in p.tool_src_grammar_files)
                    {
                        var i = System.IO.File.ReadAllText(g);
                        var n = System.IO.Path.GetFileName(g);
                        var fn = p.outputDirectory + n;
                        System.IO.File.WriteAllText(fn, Program.Localize(p.line_translation, i));
                    }
                    foreach (var g in p.additional_grammar_files)
                    {
                        var i = System.IO.File.ReadAllText(g);
                        var n = System.IO.Path.GetFileName(g);
                        var fn = p.outputDirectory + n;
                        System.IO.File.WriteAllText(fn, Program.Localize(p.line_translation, i));
                    }
                }
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(@"
// Template generated code from Antlr4BuildTasks.dotnet-antlr v " + Program.version + @"

grammar Arithmetic;

file_ : expression (SEMI expression)* EOF;
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
                var fn = p.outputDirectory + p.parser_grammar_file_name;
                System.IO.File.WriteAllText(fn, Program.Localize(p.line_translation, sb.ToString()));
            }
        }
    }
}
