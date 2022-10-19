// Template generated code from trgen <version>

package main
import (
    "fmt"
    "os"
    "io"
    "time"
    "github.com/antlr/antlr4/runtime/Go/antlr/v4"
    "example.com/myparser/<package_name>"
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
    antlr.ConsoleErrorListenerINSTANCE.ReportAmbiguity(recognizer, dfa, startIndex, stopIndex, exact, ambigAlts, configs)
}

func (l *CustomErrorListener) ReportAttemptingFullContext(recognizer antlr.Parser, dfa *antlr.DFA, startIndex, stopIndex int, conflictingAlts *antlr.BitSet, configs antlr.ATNConfigSet) {
    antlr.ConsoleErrorListenerINSTANCE.ReportAttemptingFullContext(recognizer, dfa, startIndex, stopIndex, conflictingAlts, configs)
}

func (l *CustomErrorListener) ReportContextSensitivity(recognizer antlr.Parser, dfa *antlr.DFA, startIndex, stopIndex, prediction int, configs antlr.ATNConfigSet) {
    antlr.ConsoleErrorListenerINSTANCE.ReportContextSensitivity(recognizer, dfa, startIndex, stopIndex, prediction, configs)
}

var inputs = make([]string, 0)
var is_fns = make([]bool, 0)
var error_code int = 0
var show_tree = false
var show_tokens = false

func main() {
    for i := 1; i \< len(os.Args); i = i + 1 {
        if os.Args[i] == "-tokens" {
            show_tokens = true
            continue
        } else if os.Args[i] == "-tree" {
            show_tree = true
            continue
        } else if os.Args[i] == "-input" {
            i = i + 1
            inputs = append(inputs, os.Args[i])
            is_fns = append(is_fns, false)
        } else {
            inputs = append(inputs, os.Args[i])
            is_fns = append(is_fns, true)
        }
    }
    if len(inputs) == 0 {
        ParseStdin()
    } else {
        start := time.Now()
        for i := 0; i \< len(inputs); i = i + 1 {
            if is_fns[i] {
                ParseFilename(inputs[i])
            } else {
                ParseString(inputs[i])
            }
        }
        elapsed := time.Since(start)
        fmt.Printf("Total Time: %.3f s", elapsed.Seconds())
        fmt.Println()
    }
    if error_code != 0 {
        os.Exit(1)
    } else {
        os.Exit(0)
    }
}

func ParseStdin() {
    var b []byte = make([]byte, 1)
    var st = ""
    for {
        _, err := os.Stdin.Read(b)
        if err == io.EOF {
            break
        }
        st = st + string(b)
    }
    var str antlr.CharStream = nil
    str = antlr.NewInputStream(st)
    DoParse(str)
}

func ParseString(input string) {
    str := antlr.NewInputStream(input)
    DoParse(str)
}

func ParseFilename(file_name string) {
    var str antlr.CharStream = nil
    str, _ = antlr.NewFileStream(file_name)        
    DoParse(str)
}

func DoParse(str antlr.CharStream) {
    var lexer = <go_lexer_name>(str);
    if show_tokens {
        j := 0
        for {
            t := lexer.NextToken()
            fmt.Print(j)
            fmt.Print(" ")
            // missing fmt.Println(t.String())
            fmt.Println(t.GetText())
            if t.GetTokenType() == antlr.TokenEOF {
                break
            }
            j = j + 1
        }
        // missing lexer.Reset()
    }
    // Requires additional 0??
    var tokens = antlr.NewCommonTokenStream(lexer, 0)
    var parser = <go_parser_name>(tokens)

    lexerErrors := &CustomErrorListener{}
    lexer.RemoveErrorListeners()
    lexer.AddErrorListener(lexerErrors)

    parserErrors := &CustomErrorListener{}
    parser.RemoveErrorListeners()
    parser.AddErrorListener(parserErrors)

    // mutated name--not lowercase.
    start := time.Now()
    var tree = parser.<cap_start_symbol>()
    elapsed := time.Since(start)
    fmt.Printf("Time: %.3f s", elapsed.Seconds())
    fmt.Println()
    if parserErrors.errors > 0 || lexerErrors.errors > 0 {
        fmt.Println("Parse failed.");
    } else {
        fmt.Println("Parse succeeded.")
    }
    if show_tree {
        ss := tree.ToStringTree(parser.RuleNames, parser)
        fmt.Println(ss)
    }
    if parserErrors.errors > 0 || lexerErrors.errors > 0 {
        error_code = 1
    }
}
