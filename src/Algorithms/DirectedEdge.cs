using System;

namespace Algorithms
{
    /******************************************************************************
     *  Compilation:  javac DirectedEdge.java
     *  Execution:    java DirectedEdge
     *  Dependencies: StdOut.java
     *
     *  Immutable weighted directed edge.
     *
     ******************************************************************************/
    /**
     *  The {@code DirectedEdge} class represents a weighted edge in an 
     *  {@link EdgeWeightedDigraph}. Each edge consists of two integers
     *  (naming the two vertices) and a real-value weight. The data type
     *  provides methods for accessing the two endpoints of the directed edge and
     *  the weight.
     *  <p>
     *  For additional documentation, see <a href="https://algs4.cs.princeton.edu/44sp">Section 4.4</a> of
     *  <i>Algorithms, 4th Edition</i> by Robert Sedgewick and Kevin Wayne.
     *
     *  @author Robert Sedgewick
     *  @author Kevin Wayne
     */

    public class DirectedEdge<NODE> : IEdge<NODE>
    {
        private NODE _from;
        private NODE _to;

        /**
         * Initializes a directed edge from vertex {@code v} to vertex {@code w} with
         * the given {@code weight}.
         * @param v the tail vertex
         * @param w the head vertex
         * @param weight the weight of the directed edge
         */
        public DirectedEdge()
        {
        }

        /**
         * Returns the tail vertex of the directed edge.
         * @return the tail vertex of the directed edge
         */
        public NODE From
        {
            get => _from;
            set => _from = value;
        }


        /**
         * Returns the head vertex of the directed edge.
         * @return the head vertex of the directed edge
         */
        public NODE To
        {
            get => _to;
            set => _to = value;
        }

        /**
         * Returns a string representation of the directed edge.
         * @return a string representation of the directed edge
         */
        public int CompareTo(IEdge<NODE> other)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return _from + "->" + _to;
        }

        /**
         * Unit tests the {@code DirectedEdge} data type.
         *
         * @param args the command-line arguments
         */
        public static void Test()
        {
            DirectedEdge<int> e = new DirectedEdge<int>() { From = 12, To = 34 };
            System.Console.Error.WriteLine(e);
        }
    }
}
