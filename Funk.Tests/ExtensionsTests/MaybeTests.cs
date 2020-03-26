using Xunit;
using static Funk.Prelude;

namespace Funk.Tests
{
    public partial class MaybeTests
    {
        [Fact]
        public void Get_Maybe_Or_Else()
        {
            UnitTest(
                _ => maybe(default(string)),
                m => m.GetOrElse(_ => "Funk"),
                s => Assert.Equal("Funk", s)
            );
        }

        [Fact]
        public void Get_Maybe_Or_Null()
        {
            UnitTest(
                _ => maybe(default(object)),
                m => m.GetOrNull(),
                Assert.Null
            );
        }

        [Fact]
        public void Get_Maybe_Or()
        {
            UnitTest(
                _ => maybe(default(string)),
                m => m.Or(_ => maybe("Harun")),
                s => Assert.Equal("Harun", s.UnsafeGet())
            );
        }

        [Fact]
        public void Get_Maybe_With_Nullable()
        {
            UnitTest(
                _ =>
                {
                    int? number = 1;
                    return maybe(number);
                },
                m => m.Map().Or(_ => maybe(2)),
                s => Assert.Equal(1, s.UnsafeGet())
            );
        }

        [Fact]
        public void Get_Maybe_With_Nullable_Empty()
        {
            UnitTest(
                _ => maybe((int?) null),
                m => m.Or(_ => maybe(default(int?))).Or(_ => maybe((int?)1)),
                s => Assert.Equal(1, s.GetOrElse(_ => (int?)2).GetValueOrDefault())
            );
        }
        [Fact]
        public void Get_Maybe_With_Nullable_Empty_With_Default()
        {
            UnitTest(
                _ => maybe((int?)null),
                m => m.Map().Or(_ => maybe(default(int?)).Map()).Or(_ => maybe(default(int?)).Map()),
                s => Assert.Equal(0, s.GetOrDefault())
            );
        }
    }
}
