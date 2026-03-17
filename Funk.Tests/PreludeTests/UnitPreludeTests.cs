using Xunit;

namespace Funk.Tests
{
    public partial class PreludeTests
    {
        [Fact]
        public void Empty_Returns_Unit_Value()
        {
            UnitTest(
                _ => empty,
                u => u,
                u => Assert.IsType<Unit>(u)
            );
        }

        [Fact]
        public void Empty_Equals_Unit_Value()
        {
            UnitTest(
                _ => empty,
                u => u.SafeEquals(Unit.Value),
                Assert.True
            );
        }
    }
}
