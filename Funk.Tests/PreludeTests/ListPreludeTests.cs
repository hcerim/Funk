using System.Collections.Generic;
using Xunit;

namespace Funk.Tests
{
    public partial class PreludeTests
    {
        [Fact]
        public void List_With_Items_Creates_Immutable_List()
        {
            UnitTest(
                _ => list(1, 2, 3),
                l => l,
                l =>
                {
                    Assert.Equal(3, l.Count);
                    Assert.Equal(1, l[0]);
                    Assert.Equal(2, l[1]);
                    Assert.Equal(3, l[2]);
                }
            );
        }

        [Fact]
        public void List_Empty_Creates_Empty_Immutable_List()
        {
            UnitTest(
                _ => list<int>(),
                l => l,
                l => Assert.Empty(l)
            );
        }

        [Fact]
        public void List_With_Maybes_Flattens_NonEmpty()
        {
            UnitTest(
                _ => list(may("a"), may(default(string)), may("b")),
                l => l,
                l =>
                {
                    Assert.Equal(2, l.Count);
                    Assert.Equal("a", l[0]);
                    Assert.Equal("b", l[1]);
                }
            );
        }

        [Fact]
        public void List_From_Enumerable_Creates_Immutable_List()
        {
            UnitTest(IEnumerable<string> (_) => new[] { "x", "y", "z" },
                e => list(e),
                l =>
                {
                    Assert.Equal(3, l.Count);
                    Assert.Equal("x", l[0]);
                    Assert.Equal("y", l[1]);
                    Assert.Equal("z", l[2]);
                }
            );
        }

        [Fact]
        public void Range_Creates_Sequential_Integers()
        {
            UnitTest(
                _ => range(1, 5),
                l => l,
                l =>
                {
                    Assert.Equal(5, l.Count);
                    Assert.Equal(1, l[0]);
                    Assert.Equal(5, l[4]);
                }
            );
        }

        [Fact]
        public void Repeat_Creates_Repeated_Elements()
        {
            UnitTest(
                _ => repeat("Funk", 3),
                l => l,
                l =>
                {
                    Assert.Equal(3, l.Count);
                    Assert.All(l, item => Assert.Equal("Funk", item));
                }
            );
        }
    }
}
