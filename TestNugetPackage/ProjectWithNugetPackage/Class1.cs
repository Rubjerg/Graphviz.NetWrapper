using Rubjerg.Graphviz;

namespace ProjectWithNugetPackage
{
    public class ClassWithGraphvizDependency
    {
        public static void GraphConstruction()
        {
            RootGraph root = RootGraph.CreateNew("Some Unique Identifier", GraphType.Directed);

            Node nodeA = root.GetOrAddNode("A");
            Node nodeB = root.GetOrAddNode("B");
            Node nodeC = root.GetOrAddNode("C");
            Node nodeD = root.GetOrAddNode("D");

            Edge edgeAB = root.GetOrAddEdge(nodeA, nodeB, "Some edge name");
            Edge edgeBC = root.GetOrAddEdge(nodeB, nodeC, "Some edge name");
            Edge anotherEdgeBC = root.GetOrAddEdge(nodeB, nodeC, "Another edge name");

            SubGraph cluster = root.GetOrAddSubgraph("cluster_1");
            cluster.AddExisting(nodeB);
            cluster.AddExisting(nodeC);

            Node.IntroduceAttribute(root, "my attribute", "defaultvalue");
            nodeA.SetAttribute("my attribute", "othervalue");

            edgeAB.SafeSetAttribute("color", "red", "black");
            edgeBC.SafeSetAttribute("arrowsize", "2.0", "1.0");

            root.ComputeLayout();
        }
    }
}
