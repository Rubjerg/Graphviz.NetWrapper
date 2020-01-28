using NUnit.Framework;

namespace Rubjerg.Graphviz.Test
{
    [TestFixture()]
    public class ClusterCreation
    {
        [Test()]
        public void TestCreateClusterByEdgeName()
        {
            RootGraph root = Utils.CreateUniqueTestGraph();
            root.SafeSetAttribute("compound", "true", "true");
            Node wrapper = root.GetOrAddNode("wrapper");
            Node node1 = root.GetOrAddNode("node1");
            Node node2 = root.GetOrAddNode("node2");

            root.GetOrAddEdge(wrapper, node1, "has1");
            root.GetOrAddEdge(node1, node2, "has2");
            root.GetOrAddEdge(node2, wrapper, "has3");
            root.GetOrAddEdge(wrapper, node2, "has4");

            SubGraph cluster = wrapper.CreateOrUpdateClusterByEdgeName("has1");
            cluster.SafeSetAttribute("label", "test", "");

            // Deleting the wrapper node causes the lhead/ltail edges to be removed.
            //root.Delete(wrapper);

            // Test by manual visual inspection
            //root.DumpToDotFile("dump1.dot");
            //root.DumpToPngFile("dump1.png");
        }
    }
}
