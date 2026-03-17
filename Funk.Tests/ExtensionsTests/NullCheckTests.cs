using System.Collections.Generic;
using Xunit;

namespace Funk.Tests
{
    public class NullCheckTests : Test
    {
        [Fact]
        public void IsNull_On_Null_Returns_True()
        {
            UnitTest(string (_) => null,
                s => s.IsNull(),
                result => Assert.True(result)
            );
        }

        [Fact]
        public void IsNull_On_NonNull_Returns_False()
        {
            UnitTest(
                _ => "Funk",
                s => s.IsNull(),
                result => Assert.False(result)
            );
        }

        [Fact]
        public void IsNotNull_On_Null_Returns_False()
        {
            UnitTest(string (_) => null,
                s => s.IsNotNull(),
                result => Assert.False(result)
            );
        }

        [Fact]
        public void IsNotNull_On_NonNull_Returns_True()
        {
            UnitTest(
                _ => "Funk",
                s => s.IsNotNull(),
                result => Assert.True(result)
            );
        }

        [Fact]
        public void Initialize_On_Null_Creates_New_Instance()
        {
            UnitTest(List<int> (_) => null,
                l => l.Initialize(),
                result =>
                {
                    Assert.NotNull(result);
                    Assert.Empty(result);
                }
            );
        }

        [Fact]
        public void Initialize_On_NonNull_Returns_Existing()
        {
            UnitTest(
                _ => new List<int> { 1, 2, 3 },
                l => l.Initialize(),
                result =>
                {
                    Assert.NotNull(result);
                    Assert.Equal(3, result.Count);
                }
            );
        }
    }
}
