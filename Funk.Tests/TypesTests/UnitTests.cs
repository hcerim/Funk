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
        public void Unit_Transform()
        {
            UnitTest(
                _ => Unit.Value,
                u => u.TransformTo("John"),
                s => Assert.Equal("John", s)
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
                u => u.Equals(new Unit()),
                Assert.True
            );
        }

        [Fact]
        public void Unit_ToString()
        {
            UnitTest(
                _ => Unit.Value,
                u => u.ToString(),
                Assert.Empty
            );
        }
    }
}
