using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trash.Commands
{
    class CWc
    {
        public void Help()
        {
        }

        public void Execute(Repl repl, ReplParser.WcContext tree, bool piped)
        {
            var txt = repl.input_output_stack.Pop();
            char prevChar = (char)0;
            char LF = '\n';
            char CR = '\r';
            int lineCount = 0;
            bool pendingTermination = false;
            for (var i = 0; i < txt.Length; i++)
            {
                var currentChar = (char)txt[i];
                if (currentChar == 0) { continue; }
                if (currentChar == CR || currentChar == LF)
                {
                    if (prevChar == CR && currentChar == LF) { continue; }
                    lineCount++;
                    pendingTermination = false;
                }
                else
                {
                    if (!pendingTermination)
                    {
                        pendingTermination = true;
                    }
                }
                prevChar = currentChar;
            }
            if (pendingTermination) { lineCount++; }
            repl.input_output_stack.Push(lineCount.ToString());
        }
    }
}
