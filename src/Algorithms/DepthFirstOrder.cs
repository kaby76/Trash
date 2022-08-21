using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Algorithms
{
    /******************************************************************************
     *  Compilation:  javac DepthFirstOrder.java
     *  Execution:    java DepthFirstOrder digraph.txt
     *  Dependencies: Digraph.java Queue.java Stack.java StdOut.java
     *                EdgeWeightedDigraph.java DirectedEdge.java
     *  Data files:   https://algs4.cs.princeton.edu/42digraph/tinyDAG.txt
     *                https://algs4.cs.princeton.edu/42digraph/tinyDG.txt
     *
     *  Compute preorder and postorder for a digraph or edge-weighted digraph.
     *  Runs in O(E + V) time.
     *
     *  % java DepthFirstOrder tinyDAG.txt
     *     v  pre post
     *  --------------
     *     0    0    8
     *     1    3    2
     *     2    9   10
     *     3   10    9
     *     4    2    0
     *     5    1    1
     *     6    4    7
     *     7   11   11
     *     8   12   12
     *     9    5    6
     *    10    8    5
     *    11    6    4
     *    12    7    3
     *  Preorder:  0 5 4 1 6 9 11 12 10 2 3 7 8 
     *  Postorder: 4 5 1 12 11 10 9 6 0 3 2 7 8 
     *  Reverse postorder: 8 7 2 3 0 6 9 10 11 12 1 5 4 
     *
     ******************************************************************************/

    /**
     *  The {@code DepthFirstOrder} class represents a data type for 
     *  determining depth-first search ordering of the vertices in a digraph
     *  or edge-weighted digraph, including preorder, postorder, and reverse postorder.
     *  <p>
     *  This implementation uses depth-first search.
     *  The constructor takes time proportional to <em>V</em> + <em>E</em>
     *  (in the worst case),
     *  where <em>V</em> is the number of vertices and <em>E</em> is the number of edges.
     *  Afterwards, the <em>preorder</em>, <em>postorder</em>, and <em>reverse postorder</em>
     *  operation takes take time proportional to <em>V</em>.
     *  <p>
     *  For additional documentation,
     *  see <a href="https://algs4.cs.princeton.edu/42digraph">Section 4.2</a> of
     *  <i>Algorithms, 4th Edition</i> by Robert Sedgewick and Kevin Wayne.
     *
     *  @author Robert Sedgewick
     *  @author Kevin Wayne
     */
    public class DepthFirstOrder<T, E> : IEnumerable<T>
        where E : IEdge<T>
    {
        private readonly Dictionary<T, bool> marked; // marked[v] = has v been marked in dfs?
        private readonly Dictionary<T, int> pre; // pre[v]    = preorder  number of v
        private readonly Dictionary<T, int> post; // post[v]   = postorder number of v
        private readonly Queue<T> preorder; // vertices in preorder
        private readonly Queue<T> postorder; // vertices in postorder
        private int preCounter; // counter or preorder numbering
        private int postCounter; // counter for postorder numbering

        /**
         * Determines a depth-first order for the digraph {@code G}.
         * @param G the digraph
         */
        public DepthFirstOrder(IGraph<T, E> graph, IEnumerable<T> subset_vertices)
        {
            pre = new Dictionary<T, int>();
            foreach (T v in graph.Vertices)
            {
                pre[v] = 0;
            }

            post = new Dictionary<T, int>();
            foreach (T v in graph.Vertices)
            {
                post[v] = 0;
            }

            postorder = new Queue<T>();
            preorder = new Queue<T>();
            marked = new Dictionary<T, bool>();
            foreach (T v in graph.Vertices)
            {
                marked[v] = false;
            }

            foreach (T v in subset_vertices)
            {
                if (!marked[v])
                {
                    Dfs(graph, v);
                }
            }
        }

        /**
         * Determines a depth-first order for the edge-weighted digraph {@code G}.
         * @param G the edge-weighted digraph
         */
        public DepthFirstOrder(IGraph<T, E> graph)
        {
            pre = new Dictionary<T, int>();
            foreach (T v in graph.Vertices)
            {
                pre[v] = 0;
            }

            post = new Dictionary<T, int>();
            foreach (T v in graph.Vertices)
            {
                post[v] = 0;
            }

            postorder = new Queue<T>();
            preorder = new Queue<T>();
            marked = new Dictionary<T, bool>();
            foreach (T v in graph.Vertices)
            {
                marked[v] = false;
            }

            foreach (T v in graph.Vertices)
            {
                if (!marked[v])
                {
                    Dfs(graph, v);
                }
            }
        }

        // run DFS in digraph G from vertex v and compute preorder/postorder
        private void Dfs(IGraph<T, E> G, T v)
        {
            marked[v] = true;
            pre[v] = preCounter++;
            preorder.Enqueue(v);
            foreach (T w in G.Successors(v))
            {
                if (!marked[w])
                {
                    Dfs(G, w);
                }
            }
            postorder.Enqueue(v);
            post[v] = postCounter++;
        }

        //// run DFS in edge-weighted digraph G from vertex v and compute preorder/postorder
        //private void dfs(EdgeWeightedDigraph G, int v)
        //{
        //    marked[v] = true;
        //    pre[v] = preCounter++;
        //    preorder.Enqueue(v);
        //    foreach (DirectedEdge<int> e in G.adj(v))
        //    {
        //        int w = e.To;
        //        if (!marked[w])
        //        {
        //            dfs(G, w);
        //        }
        //    }
        //    postorder.Enqueue(v);
        //    post[v] = postCounter++;
        //}

        /**
         * Returns the preorder number of vertex {@code v}.
         * @param  v the vertex
         * @return the preorder number of vertex {@code v}
         * @throws IllegalArgumentException unless {@code 0 <= v < V}
         */
        public int Pre(T v)
        {
            ValidateVertex(v);
            return pre[v];
        }

        /**
         * Returns the postorder number of vertex {@code v}.
         * @param  v the vertex
         * @return the postorder number of vertex {@code v}
         * @throws IllegalArgumentException unless {@code 0 <= v < V}
         */
        public int Post(T v)
        {
            ValidateVertex(v);
            return post[v];
        }

        /**
         * Returns the vertices in postorder.
         * @return the vertices in postorder, as an iterable of vertices
         */
        public IEnumerable<T> Post()
        {
            return postorder;
        }

        /**
         * Returns the vertices in preorder.
         * @return the vertices in preorder, as an iterable of vertices
         */
        public IEnumerable<T> Pre()
        {
            return preorder;
        }

        /**
         * Returns the vertices in reverse postorder.
         * @return the vertices in reverse postorder, as an iterable of vertices
         */
        public IEnumerable<T> ReversePost()
        {
            Stack<T> reverse = new Stack<T>();
            foreach (T v in postorder)
            {
                reverse.Push(v);
            }

            return reverse;
        }


        // check that pre() and post() are consistent with pre(v) and post(v)
        private bool Check()
        {
            // check that post(v) is consistent with post()
            int r = 0;
            foreach (T v in Post())
            {
                if (Post(v) != r)
                {
                    System.Console.WriteLine("post(v) and post() inconsistent");
                    return false;
                }
                r++;
            }

            // check that pre(v) is consistent with pre()
            r = 0;
            foreach (T v in Pre())
            {
                if (Pre(v) != r)
                {
                    System.Console.WriteLine("pre(v) and pre() inconsistent");
                    return false;
                }
                r++;
            }

            return true;
        }

        // throw an IllegalArgumentException unless {@code 0 <= v < V}
        private void ValidateVertex(T v)
        {
            if (!marked.ContainsKey(v))
                throw new Exception("Vertex " + v + " is not in marked list.");
        }

        /**
         * Unit tests the {@code DepthFirstOrder} data type.
         *
         * @param args the command-line arguments
         */
        public static void Test()
        {
            string tiny = $@"
13
22
 4  2
 2  3
 3  2
 6  0
 0  1
 2  0
11 12
12  9
 9 10
 9 11
 7  9
10 12
11  4
 4  3
 3  5
 6  8
 8  6
 5  4
 0  5
 6  4
 6  9
 7  6
";
            Digraph<IntWrapper> G = new Digraph<IntWrapper>(tiny, (string s) => new IntWrapper(int.Parse(s)));
            System.Console.Error.WriteLine(G);

            DepthFirstOrder<IntWrapper, DirectedEdge<IntWrapper>> dfs = new DepthFirstOrder<IntWrapper, DirectedEdge<IntWrapper>>(G);
            System.Console.WriteLine("   v  pre post");
            System.Console.WriteLine("--------------");
            foreach (IntWrapper v in G.Vertices)
            {
                System.Console.Write("{0} {1} {2}\n", v, dfs.Pre(v), dfs.Post(v));
            }

            System.Console.Write("Preorder:  ");
            foreach (IntWrapper v in dfs.Pre())
            {
                System.Console.Write(v + " ");
            }
            System.Console.WriteLine();

            System.Console.Write("Postorder: ");
            foreach (IntWrapper v in dfs.Post())
            {
                System.Console.Write(v + " ");
            }
            System.Console.WriteLine();

            System.Console.Write("Reverse postorder: ");
            foreach (IntWrapper v in dfs.ReversePost())
            {
                System.Console.Write(v + " ");
            }
            System.Console.WriteLine();
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (T v in Pre())
            {
                yield return v;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (T v in Pre())
            {
                yield return v;
            }
        }
    }
}
