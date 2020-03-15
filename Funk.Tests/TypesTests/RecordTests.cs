using System;
using Funk.Tests.Helpers;
using Xunit;
using static Funk.Operators;

namespace Funk.Tests
{
    public class RecordTests : Test
    {
        [Fact]
        public void Create_Record_With_One_Item_With_Factory_Create()
        {
            UnitTest(
                () => "John Doe",
                Record.Create,
                r => Assert.Equal("John Doe", r.FirstItem)
            );
        }

        [Fact]
        public void Create_Record_With_Two_Items_With_Constructor()
        {
            UnitTest(
                () => ("John","Doe"),
                p => new Record<string, string>(p.Item1, p.Item2), 
                r =>
                {
                    Assert.Equal("John", r.FirstItem);
                    Assert.Equal("Doe", r.SecondItem);
                }
            );
        }

        [Fact]
        public void Create_Record_With_Three_Items_From_Tuple_With_Factory_Create()
        {
            UnitTest(
                () => ("John", "Doe", 30),
                Record.Create,
                r =>
                {
                    Assert.Equal("John", r.FirstItem);
                    Assert.Equal("Doe", r.SecondItem);
                    Assert.Equal(30, r.ThirdItem);
                }
            );
        }

        [Fact]
        public void Create_Record_With_Two_Items_From_Tuple_With_Factory_Extension()
        {
            UnitTest(
                () => ("John", 30),
                p => p.ToRecord(),
                r =>
                {
                    Assert.Equal("John", r.FirstItem);
                    Assert.Equal(30, r.SecondItem);
                }
            );
        }

        [Fact]
        public void Map_Record_With_Two_Items_To_Record_Of_Three_Items()
        {
            UnitTest(
                () => Record.Create("John", "Doe"),
                r => r.Map((name, surname) => new Record<string, string, int>(name, surname, 30)),
                r =>
                {
                    Assert.Equal("John", r.FirstItem);
                    Assert.Equal("Doe", r.SecondItem);
                    Assert.Equal(30, r.ThirdItem);
                }
            );
        }

        [Fact]
        public void Execute_Operation_On_Record_With_Two_Items()
        {
            UnitTest(
                () => ("John", "Doe"),
                Record.Create,
                r =>
                {
                    var a = act(() => r.Do((name, surname) => throw new Exception($"{name} {surname}.")));
                    var e = Assert.Throws<Exception>(a);
                    Assert.Equal("John Doe.", e.Message);
                }
            );
        }
    }
}
