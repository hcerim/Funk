using Xunit;
using static Funk.Operators;

namespace Funk.Tests
{
    public partial class OperatorsTests
    {
        [Fact]
        public void Create_Record_With_Two_Items_With_Factory_Functional()
        {
            UnitTest(
                () => ("John", "Doe"),
                p => record(p.Item1, p.Item2),
                r =>
                {
                    Assert.Equal("John", r.Item1);
                    Assert.Equal("Doe", r.Item2);
                }
            );
        }
    }
}
