using NUnit.Framework;

namespace Rubjerg.Graphviz.Test;

[TestFixture()]
[Category("Slow")]
public class CGraphStressTests : CGraphIntegrationTests
{
    protected override int SizeMultiplier => 100;
}
