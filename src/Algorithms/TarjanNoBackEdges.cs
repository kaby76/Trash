using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Algorithms
{
    public class TarjanNoBackEdges<T, E> : IEnumerable<T>
        where E : IEdge<T>
    {
        private readonly IGraph<T, E> _graph;
        private int index = 0; // number of nodes
        private readonly Stack<T> S = new Stack<T>();
        private readonly Dictionary<T, int> Index = new Dictionary<T, int>();
        private readonly Dictionary<T, int> LowLink = new Dictionary<T, int>();
        private readonly Dictionary<E, EdgeClassifier.Classification> classify = new Dictionary<E, EdgeClassifier.Classification>();
        private readonly IEnumerable<T> _work;

        public TarjanNoBackEdges(IGraph<T, E> graph, IEnumerable<T> subset_vertices)
        {
            _graph = graph;
            _work = subset_vertices;
            foreach (T v in _work)
            {
                if (graph.Predecessors(v).Any())
                {
                    continue;
                }

                EdgeClassifier.Classify(graph, v, ref classify);
            }
            foreach (T v in _graph.Vertices)
            {
                Index[v] = -1;
                LowLink[v] = -1;
            }
        }

        public TarjanNoBackEdges(IGraph<T, E> graph)
        {
            _graph = graph;
            _work = _graph.Vertices;
            foreach (T v in _work)
            {
                if (graph.Predecessors(v).Any())
                {
                    continue;
                }

                EdgeClassifier.Classify(graph, v, ref classify);
            }
            foreach (T v in _work)
            {
                Index[v] = -1;
                LowLink[v] = -1;
            }
        }

        private IEnumerable<T> StrongConnect(T v)
        {
            // Set the depth index for v to the smallest unused index
            Index[v] = index;
            LowLink[v] = index;

            index++;
            S.Push(v);

            // Consider successors of v
            foreach (E e in _graph.SuccessorEdges(v))
            {
                if (classify[e] == EdgeClassifier.Classification.Back)
                {
                    continue;
                }

                T w = e.To;
                if (Index[w] < 0)
                {
                    // Successor w has not yet been visited; recurse on it
                    foreach (T x in StrongConnect(w))
                    {
                        yield return x;
                    }

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

                T w;
                do
                {
                    w = S.Pop();
                    //Console.Write(w + " ");
                    yield return w;
                } while (!w.Equals(v));

                //Console.WriteLine();
            }
        }

        public IEnumerable<T> GetEnumerable()
        {
            foreach (T v in _work)
            {
                if (_graph.Predecessors(v).Any())
                {
                    continue;
                }

                if (Index[v] < 0)
                {
                    foreach (T w in StrongConnect(v))
                    {
                        yield return w;
                    }
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (T v in _work)
            {
                if (_graph.Predecessors(v).Any())
                {
                    continue;
                }

                if (Index[v] < 0)
                {
                    foreach (T w in StrongConnect(v))
                    {
                        yield return w;
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (T v in _work)
            {
                if (_graph.Predecessors(v).Any())
                {
                    continue;
                }

                if (Index[v] < 0)
                {
                    foreach (T w in StrongConnect(v))
                    {
                        yield return w;
                    }
                }
            }
        }
    }
}
