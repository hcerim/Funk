using Xunit;
using static Funk.Prelude;

namespace Funk.Tests
{
    public partial class MaybeTests
    {
        [Fact]
        public void Get_Maybe_Or_Else()
        {
            UnitTest(_ => maybe(default(string)),
                m => m.GetOrElse(_ => "Funk"),
                s => Assert.Equal("Funk", s)
            );
        }

        [Fact]
        public void Get_Maybe_Or_Null()
        {
            UnitTest(_ => maybe(default(object)),
                m => m.GetOrNull(),
                Assert.Null
            );
        }

        [Fact]
        public void Get_Maybe_Or()
        {
            UnitTest(_ => maybe(default(string)),
                m => m.Or(_ => maybe("Harun")),
                s => Assert.Equal("Harun", s.UnsafeGet())
            );
        }
    }
}
