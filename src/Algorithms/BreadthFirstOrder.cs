namespace Algorithms
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class BreadthFirstOrder<T, E> : IEnumerable<T>
        where E : IEdge<T>
    {
        private readonly Dictionary<T, int> bfs;
        private readonly Queue<T> bfsorder;
        private int bfsCounter;

        public BreadthFirstOrder(IGraph<T, E> graph, IEnumerable<T> subset_vertices)
        {
            bfs = new Dictionary<T, int>();
            foreach (T v in graph.Vertices)
            {
                bfs[v] = 0;
            }

            bfsorder = new Queue<T>();
            foreach (T v in subset_vertices)
            {
                Order(graph, v);
            }

            foreach (T v in graph.Vertices)
            {
                if (!bfsorder.Contains(v))
                {
                    Order(graph, v);
                }
            }

            if (bfsorder.Count != graph.Vertices.Count())
            {
                throw new System.Exception("Internal error: Not all vertices of graph in BFS ordering.");
            }    
        }

        private void Order(IGraph<T, E> G, T s)
        {
            if (bfsorder.Contains(s))
            {
                return;
            }
            bfsorder.Enqueue(s);

            Queue<T> Q = new Queue<T>();
            Dictionary<T, int> color = new Dictionary<T, int>();
            foreach (T v in G.Vertices)
            {
                color[v] = 0;
            }

            Dictionary<T, int> d = new Dictionary<T, int>();
            foreach (T v in G.Vertices)
            {
                d[v] = 0;
            }

            Dictionary<T, T> pi = new Dictionary<T, T>();
            foreach (T v in G.Vertices)
            {
                pi[v] = default;
            }

            foreach (T u in G.Vertices)
            {
                if (!u.Equals(s))
                {
                    color[u] = 0;
                    d[u] = int.MaxValue;
                    pi[u] = default;
                }
            }
            color[s] = 1;
            d[s] = 0;
            pi[s] = default;
            Q.Enqueue(s);
            while (Q.Count > 0)
            {
                T u = Q.Dequeue();
                foreach (T v in G.Successors(u))
                {
                    if (color[v] == 0)
                    {
                        color[v] = 1;
                        if (!bfsorder.Contains(v))
                        {
                            bfs[v] = ++bfsCounter;
                            bfsorder.Enqueue(v);
                        }
                        d[v] = d[u] + 1;
                        pi[v] = u;
                        Q.Enqueue(v);
                    }
                }
                color[u] = 2;
            }
        }

        public IEnumerable<T> BFS()
        {
            return bfsorder;
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (T v in BFS())
            {
                yield return v;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (T v in BFS())
            {
                yield return v;
            }
        }
    }
}
