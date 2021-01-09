using Xunit;

namespace Funk.Tests
{
    public class UnitTests : Test
    {
        [Fact]
        public void Get_Unit_Value()
        {
            UnitTest(
                _ => Unit.Value, 
                u => u, 
                u => Assert.IsType<Unit>(u)
            );
        }

        [Fact]
        public void Unit_Map()
        {
            UnitTest(
                _ => Unit.Value,
                u => u.Match(_ => "John Doe"),
                s => Assert.Equal("John Doe", s)
            );
        }

        [Fact]
        public void Compare_Unit()
        {
            UnitTest(
                _ => Unit.Value,
                u => u.SafeEquals(new Unit()),
                Assert.True
            );
        }
    }
}
