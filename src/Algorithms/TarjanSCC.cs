using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Algorithms
{
    public class TarjanSCC<T, E>
        where E : IEdge<T>
    {
        private readonly IGraph<T, E> _graph;
        private int index = 0; // number of nodes
        private readonly Stack<T> S = new Stack<T>();
        private readonly Dictionary<T, int> Index = new Dictionary<T, int>();
        private readonly Dictionary<T, int> LowLink = new Dictionary<T, int>();
        private readonly IEnumerable<T> _work;
        private readonly Dictionary<T, IEnumerable<T>> sccs = new Dictionary<T, IEnumerable<T>>();

        public TarjanSCC(IGraph<T, E> graph)
        {
            _graph = graph;
            _work = _graph.Vertices;
            foreach (T v in _work)
            {
                Index[v] = -1;
                LowLink[v] = -1;
            }
        }

        public IDictionary<T, IEnumerable<T>> Compute()
        {
            index = 0;
            foreach (var v in _graph.Vertices)
            {
                if (Index[v] < 0)
                {
                    StrongConnect(v);
                }
            }
            return sccs;
        }

        private void StrongConnect(T v)
        {
            // Set the depth index for v to the smallest unused index
            Index[v] = index;
            LowLink[v] = index;

            index++;
            S.Push(v);

            // Consider successors of v
            foreach (E e in _graph.SuccessorEdges(v))
            {
                T w = e.To;
                if (Index[w] < 0)
                {
                    // Successor w has not yet been visited; recurse on it
                    StrongConnect(w);
                    LowLink[v] = Math.Min(LowLink[v], LowLink[w]);
                }
                else if (S.Contains(w))
                {
                    // Successor w is in stack S and hence in the current SCC
                    LowLink[v] = Math.Min(LowLink[v], Index[w]);
                }
            }

            // If v is a root node, pop the stack and generate an SCC
            if (LowLink[v] == Index[v])
            {
                //Console.Write("SCC: ");
                List<T> scc = new List<T>();
                T w;
                do
                {
                    w = S.Pop();
                    //Console.Write(w + " ");
                    scc.Add(w);
                    sccs[w] = scc;
                } while (!w.Equals(v));
                //Console.WriteLine();
            }
        }
    }
}
