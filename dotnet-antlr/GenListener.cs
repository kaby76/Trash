using System.Text;

namespace dotnet_antlr
{
    class GenListener
    {
        public static void AddErrorListener(Program p)
        {
            StringBuilder sb = new StringBuilder();
            if (p.target == Program.TargetType.CSharp && !p.antlr4cs)
            {
                sb.AppendLine(@"// Template generated code from Antlr4BuildTasks.dotnet-antlr v " + Program.version);
                if (p.@namespace != null) sb.AppendLine("namespace " + p.@namespace + @"
{");
                sb.Append(@"
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ErrorListener<S> : ConsoleErrorListener<S>
{
    public bool had_error;

    public override void SyntaxError(TextWriter output, IRecognizer recognizer, S offendingSymbol, int line,
        int col, string msg, RecognitionException e)
    {
        had_error = true;
        base.SyntaxError(output, recognizer, offendingSymbol, line, col, msg, e);
    }
}
");
                if (p.@namespace != null) sb.AppendLine("}");
                string fn = p.outputDirectory + "ErrorListener.cs";
                System.IO.File.WriteAllText(fn, Program.Localize(p.line_translation, sb.ToString()));
            }
            else if (p.target == Program.TargetType.Java)
            {
                sb.AppendLine(@"// Template generated code from Antlr4BuildTasks.dotnet-antlr v " + Program.version);
                if (p.@namespace != null) sb.AppendLine("package " + p.@namespace + @";");
                sb.Append(@"
import org.antlr.v4.runtime.*;

public class ErrorListener extends ConsoleErrorListener
{
    public boolean had_error = false;
    
    @Override
    public void syntaxError(Recognizer<?, ?> recognizer,
        Object offendingSymbol,
        int line,
        int charPositionInLine,
        String msg,
        RecognitionException e)
    {
        had_error = true;
        super.syntaxError(recognizer, offendingSymbol, line, charPositionInLine, msg, e);
    }
}
");
                // Java code has to go into the directory corresponding to the namespace.
                // Test to find an appropriate file name to place this into.
                var q = p.@namespace != null ? p.@namespace.Replace(".", "/") + "/" : "";
                string fn = p.outputDirectory + q + "ErrorListener.java";
                System.IO.File.WriteAllText(fn, Program.Localize(p.line_translation, sb.ToString()));
            }
            else if (p.target == Program.TargetType.JavaScript)
            {
            }
        }
    }
}
