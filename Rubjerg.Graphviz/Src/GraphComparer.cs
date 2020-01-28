using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubjerg.Graphviz
{
    public static class GraphComparer
    {
        public static bool CheckTopologicallyEquals(RootGraph A, RootGraph B, Action<string> logger)
        {
            logger($"Comparing graph A = '{A.GetName()}' with graph B = '{B.GetName()}'");
            logger("");

            bool result = true;
            var common_nodenames = new List<string>();
            foreach (var node in A.Nodes())
            {
                var othernode = B.GetNode(node.GetName());
                if (othernode == null)
                {
                    logger($"graph B does not contain node {node.GetName()}");
                    result = false;
                    continue;
                }
                common_nodenames.Add(node.GetName());
            }

            foreach (var node in B.Nodes())
            {
                var othernode = A.GetNode(node.GetName());
                if (othernode == null)
                {
                    logger($"graph A does not contain node {node.GetName()}");
                    result = false;
                }
            }

            foreach (var nodename in common_nodenames)
            {
                result &= CheckNode(A.GetNode(nodename), B.GetNode(nodename), logger);
            }

            logger("");
            logger($"A and B are {(result ? "" : "NOT")} topologically equivalent");
            return result;
        }

        private static bool CheckNode(Node A, Node B, Action<string> logger)
        {
            return InnerCheckNode(A, B, logger, "B") & InnerCheckNode(B, A, logger, "A");
        }

        private static bool InnerCheckNode(Node A, Node B, Action<string> logger, string nameOfGraphOfNodeB)
        {
            bool result = true;
            foreach (var a in A.EdgesOut())
            {
                var expected_endpoint = a.OppositeEndpoint(A);
                bool diff = false;
                if (!B.EdgesOut().Any(b => CheckEdgeName(a, b)))
                {
                    logger($@"In graph {nameOfGraphOfNodeB} the node '{B.GetName()}' does not have an outgoing edge with name '{a.GetName()}'");
                    result = false;
                    diff = true;
                }
                if (!B.EdgesOut().Any(b => CheckEdgeEndpoints(a, b)))
                {
                    logger($@"In graph {nameOfGraphOfNodeB} the node '{B.GetName()}' does not have an outgoing edge with head '{expected_endpoint.GetName()}'");
                    result = false;
                    diff = true;
                }
                if (!diff && !B.EdgesOut().Any(b => CheckEdge(a, b)))
                {
                    logger($@"In graph {nameOfGraphOfNodeB} the node '{B.GetName()}' does not have an outgoing edge with **both** name '{a.GetName()}' and head '{expected_endpoint.GetName()}'");
                    result = false;
                }
            }
            return result;
        }

        public static bool CheckEdge(Edge A, Edge B)
        {
            return CheckEdgeName(A, B) && CheckEdgeEndpoints(A, B);
        }

        public static bool CheckEdgeName(Edge A, Edge B)
        {
            return A.GetName() == B.GetName();
        }

        public static bool CheckEdgeEndpoints(Edge A, Edge B)
        {
            return A.Head().GetName() == B.Head().GetName() && A.Tail().GetName() == B.Tail().GetName();
        }

    }
}
