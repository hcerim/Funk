using Xunit;
using static Funk.Prelude;

namespace Funk.Tests
{
    public partial class PreludeTests
    {
        [Fact]
        public void Create_Maybe()
        {
            UnitTest(_ => maybe("Funk"),
                m => m.UnsafeGet(),
                s => Assert.Equal("Funk", s)
            );
        }
    }
}
