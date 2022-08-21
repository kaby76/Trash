//using System;
//using System.Collections.Generic;
//using System.Globalization;

//namespace NWayDiff
//{
//    class Program
//    {
//        static void Main()
//        {
//            List<List<string>> exprs = new List<List<string>>()
//            {
//                new List<string>() { "'import'", "typeName", "';'" },
//                new List<string>() { "'import'", "'static'", "typeName", "'.'", "identifier", "';'" },
//                new List<string>() { "'import'", "'static'", "typeName", "'.'", "'*'", "';'" },
//                new List<string>() { "'import'", "packageOrTypeName", "'.'", "'*'", "';'" },
//            };
//            var difdef = new Difdef<string>(exprs.Count);
//            for (int x = 0; x < exprs.Count; ++x)
//            {
//                difdef.set_up_sequece(x, exprs[x]);
//            }
//            var diff = difdef.merge();

//            {
//                System.Console.Write("importDeclaration :");
//                bool first = true;
//                for (int x = 0; x < exprs.Count; ++x)
//                {
//                    if (! first)
//                    {
//                        System.Console.Write(" |");
//                    }
//                    for (int y = 0; y < exprs[x].Count; ++y)
//                    {
//                        System.Console.Write(" " + exprs[x][y]);
//                    }
//                }
//                System.Console.WriteLine();
//            }

//            System.Console.WriteLine("=>");
//            System.Console.Write("importDeclaration :");
//            int i = 0;
//            while (i < diff.lines.Count)
//            {
//                // Find anchor.
//                int j = i;
//                for (; j < diff.lines.Count; ++j)
//                {
//                    if (diff.lines[j].mask != diff.mask)
//                        break;
//                }
//                j = j - 1;
//                // Bracket all between i and j inclusive. These all have a common mask.
//                for (int k = i; k <= j; ++k)
//                    System.Console.Write(" " + diff.lines[k].text);

//                // Look for next common mask.
//                int l = j + 1;
//                for (; l < diff.lines.Count; ++l)
//                {
//                    if (diff.lines[l].mask == diff.mask)
//                        break;
//                }

//                if (l == diff.lines.Count)
//                    break;

//                // Now add parentheses and "|" for everything in between j+1 and l-1
//                System.Console.Write(" (");
//                bool first = true;
//                for (int f = 0; f < diff.dimension; ++f)
//                {
//                    if (!first)
//                        System.Console.Write(" |");
//                    for (int k = j + 1; k < l; ++k)
//                    {
//                        if ((diff.lines[k].mask & (1 << f)) != 0)
//                        {
//                            first = false;
//                            System.Console.Write(" " + diff.lines[k].text);
//                        }
//                    }
//                }
//                System.Console.Write(" )");
//                i = l;
//            }
//            System.Console.WriteLine();
//        }
//    }
//}
