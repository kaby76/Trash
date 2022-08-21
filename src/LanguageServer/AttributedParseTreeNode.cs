namespace LanguageServer
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class AttributedParseTreeNode : AntlrTreeEditing.AntlrDOM.ObserverParserRuleContext
    {
        public AttributedParseTreeNode()
            : base()
        {
        }

        public AttributedParseTreeNode(ParserRuleContext parent, int invokingState)
            : base(parent, invokingState)
        {
        }
    }
}
