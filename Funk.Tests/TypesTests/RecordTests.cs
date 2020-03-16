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
                r => Assert.Equal("John Doe", r.Item1)
            );
        }

        [Fact]
        public void Create_Record_With_Two_Items_With_Constructor()
        {
            UnitTest(
                () => ("John", "Doe"),
                p => new Record<string, string>(p.Item1, p.Item2),
                r =>
                {
                    Assert.Equal("John", r.Item1);
                    Assert.Equal("Doe", r.Item2);
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
                    Assert.Equal("John", r.Item1);
                    Assert.Equal("Doe", r.Item2);
                    Assert.Equal(30, r.Item3);
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
                    Assert.Equal("John", r.Item1);
                    Assert.Equal(30, r.Item2);
                }
            );
        }

        [Fact]
        public void Match_Record_With_Two_Items_To_String()
        {
            UnitTest(
                () => Record.Create("John", "Doe"),
                r => r.Match((name, surname) => $"{name} {surname}"),
                s =>
                {
                    Assert.Equal("John Doe", s);
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
                    var a = act(() => r.Match((name, surname) => throw new Exception($"{name} {surname}.")));
                    var e = Assert.Throws<Exception>(a);
                    Assert.Equal("John Doe.", e.Message);
                }
            );
        }

        [Fact]
        public void Map_Record_With_Two_Items()
        {
            UnitTest(
                () => record("John", "Doe"),
                r => r.Map((name, surname) => record($"{name} {surname}", 30)),
                r =>
                {
                    Assert.Equal("John Doe", r.Item1);
                    Assert.Equal(30, r.Item2);
                }
            );
        }
    }
}
