using Xunit;
using static Funk.Prelude;

namespace Funk.Tests
{
    public partial class PreludeTests
    {
        [Fact]
        public void Create_Maybe()
        {
            UnitTest(
                _ => may("Funk"),
                m => m.UnsafeGet(),
                s => Assert.Equal("Funk", s)
            );
        }

        [Fact]
        public void Create_Maybe_Empty_Nullable_Throws()
        {
            UnitTest(
                _ => may(default(int?)),
                m => act(() => m.UnsafeGet()),
                a => Assert.Throws<EmptyValueException>(a)
            );
        }
    }
}
