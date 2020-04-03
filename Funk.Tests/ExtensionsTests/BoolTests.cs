using Xunit;

namespace Funk.Tests
{
    public class BoolTests : Test
    {
        [Fact]
        public void Bool_To_Maybe()
        {
            UnitTest(
                _ => true,
                b => b.AsTrue(),
                m =>
                {
                    Assert.True(m.NotEmpty);
                    Assert.True(m.UnsafeGet());
                }
            );
        }

        [Fact]
        public void Bool_Nullable_Empty_To_Maybe()
        {
            UnitTest(
                _ => default(bool?),
                b => b.AsTrue(),
                m => Assert.True(m.IsEmpty)
            );
        }

        [Fact]
        public void Bool_Nullable_To_Maybe()
        {
            UnitTest(
                _ => (bool?)true,
                b => b.AsTrue(),
                m =>
                {
                    Assert.True(m.NotEmpty);
                    Assert.True(m.UnsafeGet());
                }
            );
        }
    }
}
