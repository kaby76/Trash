using Antlr4.Runtime.Misc;
using System.Collections.Generic;

namespace EditableAntlrTree
{
    public interface IMyParseTreeNode
    {
        List<IMyParseTreeNode> Children { get; }
    }
}
