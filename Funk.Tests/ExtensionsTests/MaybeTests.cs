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
                _ => may(default(string)),
                m => m.GetOrElse(_ => "Funk"),
                s => Assert.Equal("Funk", s)
            );
        }

        [Fact]
        public void Get_Maybe_Or_Null()
        {
            UnitTest(
                _ => may(default(object)),
                m => m.GetOrNull(),
                Assert.Null
            );
        }

        [Fact]
        public void Get_Maybe_Or()
        {
            UnitTest(
                _ => may(default(string)),
                m => m.Or(_ => may("Harun")),
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
                    return may(number);
                },
                m => m.Or(_ => may(2)),
                s => Assert.Equal(1, s.UnsafeGet())
            );
        }

        [Fact]
        public void Get_Maybe_With_Nullable_Empty()
        {
            UnitTest(
                _ => may((int?) null),
                m => m.Or(_ => may(default(int?))).Or(_ => may((int?)1)),
                s => Assert.Equal(1, s.GetOrElse(_ => 2))
            );
        }
        [Fact]
        public void Get_Maybe_With_Nullable_Empty_With_Default()
        {
            UnitTest(
                _ => may((int?)null),
                m => m.Or(_ => may(default(int?))).Or(_ => may(default(int?))),
                s => Assert.True(s.IsEmpty)
            );
        }
    }
}
