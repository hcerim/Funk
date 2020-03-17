using Xunit;
using static Funk.Prelude;

namespace Funk.Tests
{
    public partial class PreludeTests
    {
        [Fact]
        public void Create_Record_With_2_Items_With_Factory_Functional()
        {
            UnitTest(
                _ => ("John", "Doe"),
                p => record(p.Item1, p.Item2),
                r =>
                {
                    Assert.Equal("John", r.Item1);
                    Assert.Equal("Doe", r.Item2);
                }
            );
        }

        [Fact]
        public void Create_Record_With_5_Items_With_Factory_Functional()
        {
            UnitTest(
                _ => ("John", "Doe", 30, record("John"), record("Doe")),
                p => record(p.Item1, p.Item2, p.Item3, p.Item4, p.Item5),
                r =>
                {
                    Assert.Equal("John", r.Item1);
                    Assert.Equal("Doe", r.Item2);
                    Assert.Equal(30, r.Item3);
                    Assert.Equal("John", r.Item4.Item1);
                    Assert.Equal("Doe", r.Item5.Item1);
                }
            );
        }
    }
}
