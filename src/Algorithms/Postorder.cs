using System.Collections.Generic;
using Utils;

namespace Algorithms
{
    // Algorithms adapted from "A NEW NON-RECURSIVE ALGORITHM FOR
    // BINARY SEARCH TREE TRAVERSAL", Akram Al-Rawi, Azzedine Lansari, Faouzi Bouslama
    // N.B.: There is no "in-order" traversal defined for a general graph,
    // it must be a binary tree.
    public class Postorder
    {
        public static System.Collections.Generic.IEnumerable<T> Sort<T, E>
            (IGraph<T, E> graph, IEnumerable<T> source)
            where E : IEdge<T>
        {
            Dictionary<T, bool> Visited = new Dictionary<T, bool>();
            StackQueue<T> Stack = new StackQueue<T>();

            foreach (T v in graph.Vertices)
            {
                Visited.Add(v, false);
            }

            foreach (T v in source)
            {
                Stack.Push(v);
            }

            while (Stack.Count != 0)
            {
                T u = Stack.Pop();
                if (Visited[u])
                {
                    yield return u;
                }
                else
                {
                    Visited[u] = true;
                    Stack.Push(u);
                    foreach (T v in graph.ReverseSuccessors(u))
                    {
                        if (!Visited[v] && !Stack.Contains(v))
                        {
                            Stack.Push(v);
                        }
                    }
                }
            }
        }

        public static void Test()
        {
            {
                string input = @"
14
15
1  5
2  4
2  3
3  9
4  5
5  7
5  6
6  8
7 8
8 2
8 9
9 11
9 10
10 11
12 13
";

                Digraph<IntWrapper> graph = new Digraph<IntWrapper>(input, (string s) => new IntWrapper(int.Parse(s)));
                IEnumerable<IntWrapper> sort = Postorder.Sort(graph, new List<IntWrapper>() { new IntWrapper(1) });
                foreach (IntWrapper n in sort)
                {
                    System.Console.Error.WriteLine(n);
                }
            }
            {
                string input = @"
6
5
1  2
2  4
2  5
1  3
3  6
";

                Digraph<IntWrapper> graph = new Digraph<IntWrapper>(input, (string s) => new IntWrapper(int.Parse(s)));
                IEnumerable<IntWrapper> sort = Postorder.Sort(graph, new List<IntWrapper>() { new IntWrapper(1) });
                foreach (IntWrapper n in sort)
                {
                    System.Console.Error.WriteLine(n);
                }
            }
        }
    }
}
