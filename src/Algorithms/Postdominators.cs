using System.Collections;
using System.Collections.Generic;

namespace Algorithms
{
    //Digraph dg = new Digraph(5);
    //dg.AddVertex(0);
    //dg.AddVertex(1);
    //dg.AddVertex(2);
    //dg.AddVertex(3);
    //dg.AddVertex(4);
    //dg.addEdge(new DirectedEdge<int>(0, 1));
    //dg.addEdge(new DirectedEdge<int>(1, 0));
    //dg.addEdge(new DirectedEdge<int>(3, 0));
    //dg.addEdge(new DirectedEdge<int>(4, 3));
    //dg.addEdge(new DirectedEdge<int>(2, 1));
    //dg.addEdge(new DirectedEdge<int>(4, 2));
    //// from https://www.cs.rice.edu/~keith/EMBED/dom.pdf
    //Dominators<int, DirectedEdge<int>> dom1 = new Dominators<int, DirectedEdge<int>>(dg, dg.Vertices, 4);
    //dom1.run();
    // Naive Iterative
    public class PostDominators<T, E> : IEnumerable<T>
        where E : IEdge<T>
    {
        public Dictionary<T, HashSet<T>> _doms;
        private readonly IGraph<T, E> _graph;
        private readonly IEnumerable<T> _work;
        private readonly T _exit;

        public PostDominators(IGraph<T, E> graph, IEnumerable<T> subset_vertices, T exit)
        {
            _graph = graph;
            _work = subset_vertices;
            _exit = exit;
            _doms = new Dictionary<T, HashSet<T>>();
        }

        private HashSet<T> All()
        {
            HashSet<T> d = new HashSet<T>();
            foreach (T u in _work)
            {
                d.Add(u);
            }
            return d;
        }

        private HashSet<T> One(T v)
        {
            HashSet<T> d = new HashSet<T>
            {
                v
            };
            return d;
        }

        public void Run()
        {
            foreach (T t in _work)
            {
                _doms[t] = t.Equals(_exit) ? One(_exit) : All();
            }

            bool changed = true;
            while (changed)
            {
                changed = false;
                foreach (T v in _work)
                {
                    if (v.Equals(_exit))
                    {
                        continue;
                    }

                    HashSet<T> new_idom = All();
                    foreach (T q in _graph.Successors(v))
                    {
                        new_idom.IntersectWith(_doms[q]);
                    }

                    new_idom.Add(v);
                    if (!_doms[v].SetEquals(new_idom))
                    {
                        _doms[v] = new_idom;
                        changed = true;
                    }
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
