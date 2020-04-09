using System;
using Xunit;
using static Funk.Prelude;

namespace Funk.Tests
{
    public partial class RecordTests : Test
    {
        [Fact]
        public void Create_2_Identical_Records_With_3_Items_With_Prelude()
        {
            UnitTest(
                _ => ("John", "Doe", 30),
                t =>
                {
                    var record = rec(t);
                    var record2 = rec(t.Item1, t.Item2, t.Item3);
                    return record.SafeEquals(record2);
                },
                Assert.True
            );
        }

        [Fact]
        public void Create_2_Different_Records_With_3_Items_With_Prelude()
        {
            UnitTest(
                _ => ("John", "Doe", 30),
                t =>
                {
                    var record = rec(t);
                    var record2 = rec(t.Item1, t.Item2, 40);
                    return record.SafeEquals(record2);
                },
                Assert.False
            );
        }

        [Fact]
        public void Create_Record_With_1_Item_With_Factory_Create()
        {
            UnitTest(
                _ => "John Doe",
                Record.Create,
                r => Assert.Equal("John Doe", r.Item1)
            );
        }

        [Fact]
        public void Create_Record_With_2_Items_With_Prelude()
        {
            UnitTest(
                _ => ("John", "Doe"),
                p => rec(p.Item1, p.Item2),
                r =>
                {
                    Assert.Equal("John", r.Item1);
                    Assert.Equal("Doe", r.Item2);
                }
            );
        }

        [Fact]
        public void Create_Record_With_3_Items_From_Tuple_With_Factory_Create()
        {
            UnitTest(
                _ => ("John", "Doe", 30),
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
        public void Match_Record_With_2_Items_To_String()
        {
            UnitTest(
                _ => Record.Create("John", "Doe"),
                r => r.Match((name, surname) => $"{name} {surname}"),
                s =>
                {
                    Assert.Equal("John Doe", s);
                }
            );
        }

        [Fact]
        public void Execute_Operation_On_Record_With_2_Items()
        {
            UnitTest(
                _ => ("John", "Doe"),
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
        public void Map_Record_With_2_Items_Explicit()
        {
            UnitTest(
                _ => rec("John", "Doe"),
                r => r.FlatMap((name, surname) => rec($"{name} {surname}", 30)),
                r =>
                {
                    Assert.Equal("John Doe", r.Item1);
                    Assert.Equal(30, r.Item2);
                }
            );
        }

        [Fact]
        public void Map_Record_With_2_Items_Implicit()
        {
            UnitTest(
                _ => rec("John", "Doe"),
                r => r.Map((name, surname) => ($"{name} {surname}", 30)),
                r =>
                {
                    Assert.Equal("John Doe", r.Item1);
                    Assert.Equal(30, r.Item2);
                }
            );
        }
    }
}
