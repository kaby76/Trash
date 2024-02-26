namespace org.eclipse.wst.xml.xpath2.processor.@internal
{
    using ast;
    using System;
    using System.IO;
    using System.Text;

    public class OutputXPathExpression
    {
        private int changed = 0;
        private bool first_time = true;

        public StringBuilder OutputTree(XPathNode tree)
        {
            var sb = new StringBuilder();
            changed = 0;
            first_time = true;
            ParenthesizedAST(tree, sb, 0);
            return sb;
        }

        public void ParenthesizedAST(XPathNode tree, StringBuilder sb, int level = 0)
        {
            if (tree == null) return;
            {
                var fixed_name = tree.GetType().ToString();
                var ind = fixed_name.LastIndexOf('.');
                fixed_name = fixed_name.Substring(ind + 1);
                StartLine(sb, tree, level);
                sb.Append("( " + fixed_name);
                sb.Append(" " + tree.QuickInfo());
                sb.AppendLine();
            }
            foreach (var c in tree.GetAllChildren())
            {
                ParenthesizedAST(c, sb, level + 1);
            }
            if (level == 0)
            {
                for (int k = 0; k < 1 + changed - level; ++k) sb.Append(") ");
                sb.AppendLine();
                changed = 0;
            }
        }

        private void StartLine(StringBuilder sb, XPathNode tree, int level = 0)
        {
            if (changed - level >= 0)
            {
                if (!first_time)
                {
                    for (int j = 0; j < level; ++j) sb.Append("  ");
                    for (int k = 0; k < 1 + changed - level; ++k) sb.Append(") ");
                    sb.AppendLine();
                }
                changed = 0;
                first_time = false;
            }
            changed = level;
            for (int j = 0; j < level; ++j) sb.Append("  ");
        }

        private string ToLiteral(string input)
        {
            using (var writer = new StringWriter())
            {
                var literal = input;
                literal = literal.Replace("\\", "\\\\");
                literal = literal.Replace("\b", "\\b");
                literal = literal.Replace("\n", "\\n");
                literal = literal.Replace("\t", "\\t");
                literal = literal.Replace("\r", "\\r");
                literal = literal.Replace("\f", "\\f");
                literal = literal.Replace("\"", "\\\"");
                literal = literal.Replace(string.Format("\" +{0}\t\"", Environment.NewLine), "");
                return literal;
            }
        }
    }
}
