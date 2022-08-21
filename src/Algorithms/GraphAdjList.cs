using Algorithms.Utils;
using System.Collections.Generic;
using System.Text;

namespace Algorithms
{
    public class GraphAdjList<NODE, EDGE> : IGraph<NODE, EDGE>
        where EDGE : IEdge<NODE>
    {
        public Dictionary<NODE, NODE> VertexSpace = new Dictionary<NODE, NODE>();
        public MultiMap<NODE, EDGE> ForwardEdgeSpace = new MultiMap<NODE, EDGE>();
        public MultiMap<NODE, EDGE> ReverseEdgeSpace = new MultiMap<NODE, EDGE>();
        private HashSet<NODE> StartSpace = new HashSet<NODE>();
        private HashSet<NODE> EndSpace = new HashSet<NODE>();

        private class VertexEnumerator : IEnumerable<NODE>
        {
            private readonly Dictionary<NODE, NODE> VertexSpace;

            public VertexEnumerator(Dictionary<NODE, NODE> vs)
            {
                VertexSpace = vs;
            }

            public IEnumerator<NODE> GetEnumerator()
            {
                foreach (NODE key in VertexSpace.Keys)
                {
                    yield return key;
                }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public IEnumerable<NODE> Vertices => new VertexEnumerator(VertexSpace);

        public class EdgeEnumerator : IEnumerable<EDGE>
        {
            private readonly MultiMap<NODE, EDGE> EdgeSpace;

            public EdgeEnumerator(MultiMap<NODE, EDGE> es)
            {
                EdgeSpace = es;
            }

            public IEnumerator<EDGE> GetEnumerator()
            {
                foreach (KeyValuePair<NODE, List<EDGE>> t in EdgeSpace)
                {
                    List<EDGE> l = t.Value;
                    foreach (EDGE e in l)
                    {
                        yield return e;
                    }
                }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public IEnumerable<EDGE> Edges => new EdgeEnumerator(ForwardEdgeSpace);

        public IEnumerable<NODE> StartVertices { get { return StartSpace; } }
        public IEnumerable<NODE> EndVertices { get { return EndSpace; } }

        public virtual NODE AddVertex(NODE v)
        {
            if (VertexSpace.ContainsKey(v))
            {
                return VertexSpace[v];
            }
            VertexSpace[v] = v;
            return v;
        }

        public virtual NODE AddStart(NODE v)
        {
            if (StartSpace.Contains(v))
            {
                return v;
            }
            StartSpace.Add(v);
            return v;
        }

        public virtual NODE AddEnd(NODE v)
        {
            if (EndSpace.Contains(v))
            {
                return v;
            }
            EndSpace.Add(v);
            return v;
        }

        public virtual void DeleteVertex(NODE v)
        {
        }

        public virtual EDGE AddEdge(EDGE e)
        {
            _ = AddVertex(e.From);
            _ = AddVertex(e.To);
            ForwardEdgeSpace.Add(e.From, e);
            ReverseEdgeSpace.Add(e.To, e);
            return e;
        }

        public virtual void DeleteEdge(EDGE e)
        {
            NODE vf = e.From;
            NODE vt = e.To;
            bool done = ForwardEdgeSpace.TryGetValue(vf, out List<EDGE> list);
            if (done)
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    if (list[i].From.Equals(vf) && list[i].To.Equals(vt))
                    {
                        list.RemoveAt(i);
                        break;
                    }
                }
            }

            done = ReverseEdgeSpace.TryGetValue(vf, out _);
            if (done)
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    if (list[i].From.Equals(vt) && list[i].To.Equals(vf))
                    {
                        list.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public GraphAdjList()
        {
        }

        private class PredecessorEnumerator : IEnumerable<NODE>
        {
            private readonly GraphAdjList<NODE, EDGE> graph;
            private readonly NODE node;

            public PredecessorEnumerator(GraphAdjList<NODE, EDGE> g, NODE n)
            {
                graph = g;
                node = n;
            }

            public IEnumerator<NODE> GetEnumerator()
            {
                if (graph.ReverseEdgeSpace.TryGetValue(node, out List<EDGE> list))
                {
                    foreach (EDGE e in list)
                    {
                        yield return e.From;
                    }
                }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public IEnumerable<NODE> Predecessors(NODE n)
        {
            return new PredecessorEnumerator(this, n);
        }

        private class PredecessorEdgeEnumerator : IEnumerable<EDGE>
        {
            private readonly GraphAdjList<NODE, EDGE> graph;
            private readonly NODE node;

            public PredecessorEdgeEnumerator(GraphAdjList<NODE, EDGE> g, NODE n)
            {
                graph = g;
                node = n;
            }

            public IEnumerator<EDGE> GetEnumerator()
            {
                if (graph.ReverseEdgeSpace.TryGetValue(node, out List<EDGE> list))
                {
                    foreach (EDGE e in list)
                    {
                        yield return e;
                    }
                }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public IEnumerable<EDGE> PredecessorEdges(NODE n)
        {
            return new PredecessorEdgeEnumerator(this, n);
        }

        private class ReversePredecessorEnumerator : IEnumerable<NODE>
        {
            private readonly GraphAdjList<NODE, EDGE> graph;
            private readonly NODE node;

            public ReversePredecessorEnumerator(GraphAdjList<NODE, EDGE> g, NODE n)
            {
                graph = g;
                node = n;
            }

            public IEnumerator<NODE> GetEnumerator()
            {
                if (graph.ReverseEdgeSpace.TryGetValue(node, out List<EDGE> list))
                {
                    list.Reverse();
                    foreach (EDGE e in list)
                    {
                        yield return e.From;
                    }
                }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public IEnumerable<NODE> ReversePredecessors(NODE n)
        {
            return new ReversePredecessorEnumerator(this, n);
        }

        public IEnumerable<NODE> PredecessorNodes(NODE n)
        {
            return new PredecessorEnumerator(this, n);
        }

        private class SuccessorEnumerator : IEnumerable<NODE>
        {
            private readonly GraphAdjList<NODE, EDGE> graph;
            private readonly NODE node;

            public SuccessorEnumerator(GraphAdjList<NODE, EDGE> g, NODE n)
            {
                graph = g;
                node = n;
            }

            public IEnumerator<NODE> GetEnumerator()
            {
                if (graph.ForwardEdgeSpace.TryGetValue(node, out List<EDGE> list))
                {
                    foreach (EDGE e in list)
                    {
                        yield return e.To;
                    }
                }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public IEnumerable<NODE> Successors(NODE n)
        {
            return new SuccessorEnumerator(this, n);
        }

        public IEnumerable<NODE> SuccessorNodes(NODE n)
        {
            return new SuccessorEnumerator(this, n);
        }

        private class SuccessorEdgeEnumerator : IEnumerable<EDGE>
        {
            private readonly GraphAdjList<NODE, EDGE> graph;
            private readonly NODE node;

            public SuccessorEdgeEnumerator(GraphAdjList<NODE, EDGE> g, NODE n)
            {
                graph = g;
                node = n;
            }

            public IEnumerator<EDGE> GetEnumerator()
            {
                if (graph.ForwardEdgeSpace.TryGetValue(node, out List<EDGE> list))
                {
                    foreach (EDGE e in list)
                    {
                        yield return e;
                    }
                }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public IEnumerable<EDGE> SuccessorEdges(NODE n)
        {
            return new SuccessorEdgeEnumerator(this, n);
        }

        public class ReverseSuccessorEnumerator : IEnumerable<NODE>
        {
            private readonly GraphAdjList<NODE, EDGE> graph;
            private readonly NODE node;

            public ReverseSuccessorEnumerator(GraphAdjList<NODE, EDGE> g, NODE n)
            {
                graph = g;
                node = n;
            }

            public IEnumerator<NODE> GetEnumerator()
            {
                if (graph.ForwardEdgeSpace.TryGetValue(node, out List<EDGE> list))
                {
                    list.Reverse();
                    foreach (EDGE e in list)
                    {
                        yield return e.To;
                    }
                }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public IEnumerable<NODE> ReverseSuccessors(NODE n)
        {
            return new ReverseSuccessorEnumerator(this, n);
        }

        public bool IsLeaf(NODE name)
        {
            if (ForwardEdgeSpace.TryGetValue(name, out _))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public virtual string ToDotString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("digraph G {");
            sb.AppendLine("rankdir=\"LR\";");
            foreach (var s in StartVertices) sb.AppendLine(s + " [ pos=left ];");
            foreach (var e in Edges)
            {
                sb.AppendLine(e.ToString());
            }
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
