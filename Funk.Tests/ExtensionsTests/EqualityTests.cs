using System.Collections.Generic;
using Xunit;

namespace Funk.Tests
{
    public class EqualityTests : Test
    {
        [Fact]
        public void Enumerable_Any_Equals()
        {
            UnitTest(
                _ => new List<string>{null, "Harun", "Funk"},
                l => l.SafeAnyEquals("Funk"),
                Assert.True
            );
        }

        [Fact]
        public void Enumerable_All_Equals()
        {
            UnitTest(
                _ => new List<string> { null, "Harun", "Funk" },
                l => l.SafeAllEquals("Funk"),
                Assert.False
            );
        }

        [Fact]
        public void Enumerable_All_Equals_Nullable()
        {
            UnitTest(
                _ => new List<int?> { null, 1, 2 },
                l => l.SafeAllEquals(2),
                Assert.False
            );
        }

        [Fact]
        public void Enumerable_All_Equals_Nullable_Reverse()
        {
            UnitTest(
                _ => new List<int?> { null, 1, 2 },
                l => 2.SafeEqualsToAll(l),
                Assert.False
            );
        }

        [Fact]
        public void Enumerable_All_Equals_Nullable_Reverse_Null()
        {
            UnitTest(
                _ => default(int?[]),
                l => 2.SafeEqualsToAll(l),
                Assert.True
            );
        }

        [Fact]
        public void Enumerable_Any_Equals_Nullable_Reverse()
        {
            UnitTest(
                _ => new List<int?> { null, 1, 2 },
                l => 2.SafeEqualsToAny(l),
                Assert.True
            );
        }
    }
}
