﻿using Antlr4.Runtime.Tree;

namespace Trash
{
    static class Util
    {
        public static IParseTree Root(this IParseTree tree)
        {
            for (; tree != null; tree = tree.Parent) if (tree.Parent == null) return tree;
            return null;
        }
    }
}
