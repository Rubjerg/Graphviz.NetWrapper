using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubjerg.Graphviz
{
    public static class GraphComparer
    {
        public static bool CheckTopologicallyEquals(Graph A, Graph B, Action<string> logger)
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
                result &= CheckNode(A, B, A.GetNode(nodename), B.GetNode(nodename), logger);
            }

            logger("");
            logger($"A and B are {(result ? "" : "NOT")} topologically equivalent");
            return result;
        }

        private static bool CheckNode(Graph A, Graph B, Node nA, Node nB, Action<string> logger)
        {
            return InnerCheckNode(A, B, nA, nB, logger, "B") & InnerCheckNode(B, A, nA, nB, logger, "A");
        }

        private static bool InnerCheckNode(Graph A, Graph B, Node nA, Node nB, Action<string> logger, string nameOfGraphOfNodeB)
        {
            bool result = true;
            foreach (var eA in nA.EdgesOut(A))
            {
                var expected_endpoint = eA.OppositeEndpoint(nA);
                bool diff = false;
                if (!nB.EdgesOut(B).Any(eB => CheckEdgeName(eA, eB)))
                {
                    logger($@"In graph {nameOfGraphOfNodeB} the node '{nB.GetName()}' does not have an outgoing edge with name '{eA.GetName()}'");
                    result = false;
                    diff = true;
                }
                if (!nB.EdgesOut(B).Any(eB => CheckEdgeEndpoints(eA, eB)))
                {
                    logger($@"In graph {nameOfGraphOfNodeB} the node '{nB.GetName()}' does not have an outgoing edge with head '{expected_endpoint.GetName()}'");
                    result = false;
                    diff = true;
                }
                if (!diff && !nB.EdgesOut(B).Any(eB => CheckEdge(eA, eB)))
                {
                    logger($@"In graph {nameOfGraphOfNodeB} the node '{nB.GetName()}' does not have an outgoing edge with **both** name '{eA.GetName()}' and head '{expected_endpoint.GetName()}'");
                    result = false;
                }
            }
            return result;
        }

        public static bool CheckEdge(Edge eA, Edge eB)
        {
            return CheckEdgeName(eA, eB) && CheckEdgeEndpoints(eA, eB);
        }

        public static bool CheckEdgeName(Edge eA, Edge eB)
        {
            return eA.GetName() == eB.GetName();
        }

        public static bool CheckEdgeEndpoints(Edge eA, Edge eB)
        {
            return eA.Head().GetName() == eB.Head().GetName() && eA.Tail().GetName() == eB.Tail().GetName();
        }

    }
}
