namespace LanguageServer
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using Workspaces;

    public class Util
    {
        public static TerminalNodeImpl Find(int index, Document document)
        {
            ParsingResults pd = ParsingResultsFactory.Create(document);
            var workspace = document.Workspace;
            if (pd.ParseTree == null)
            {
                new Module().Compile(workspace);
            }

            foreach (IParseTree node in DFSVisitor.DFS(pd.ParseTree as ParserRuleContext))
            {
                if (node as TerminalNodeImpl == null)
                {
                    continue;
                }

                TerminalNodeImpl leaf = node as TerminalNodeImpl;
                if (leaf.Symbol.StartIndex <= index && index <= leaf.Symbol.StopIndex)
                {
                    return leaf;
                }
            }
            return null;
        }
    }
}
