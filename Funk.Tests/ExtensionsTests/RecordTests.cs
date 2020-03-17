using Xunit;

namespace Funk.Tests
{
    public partial class RecordTests
    {
        [Fact]
        public void Create_Record_With_1_Item_From_Tuple_With_Factory_Extension()
        {
            UnitTest(
                () => "John",
                p => p.ToRecord(),
                r =>
                {
                    Assert.Equal("John", r.Item1);
                }
            );
        }

        [Fact]
        public void Create_Record_With_2_Items_From_Tuple_With_Factory_Extension()
        {
            UnitTest(
                () => ("John", 30),
                p => p.ToRecord(),
                r =>
                {
                    Assert.Equal("John", r.Item1);
                    Assert.Equal(30, r.Item2);
                }
            );
        }

        [Fact]
        public void Create_Record_With_3_Items_From_Tuple_With_Factory_Extension()
        {
            UnitTest(
                () => ("John", "Doe", 30),
                p => p.ToRecord(),
                r =>
                {
                    Assert.Equal("John", r.Item1);
                    Assert.Equal("Doe", r.Item2);
                    Assert.Equal(30, r.Item3);
                }
            );
        }
    }
}
