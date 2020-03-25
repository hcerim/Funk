using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Funk.Tests
{
    public class EnumerableTests : Test
    {
        [Fact]
        public void Create_ReadOnlyCollection_From_Not_Empty_List()
        {
            UnitTest(
                _ => new List<string>{"Funk", "Funky Funk", "Da Funk"}, 
                l => l.ToReadOnlyCollection(),
                c => Assert.Equal("Funky Funk", c.ElementAt(1))
            );
        }

        [Fact]
        public void Create_ReadOnlyCollection_From_Null_List()
        {
            UnitTest(
                _ => default(List<string>),
                l => l.ToReadOnlyCollection(),
                Assert.Empty
            );
        }

        [Fact]
        public void Create_ReadOnlyCollection_From_Empty_Maybe()
        {
            UnitTest(
                _ => default(string).AsMaybe(),
                m => m.AsReadOnlyCollection(),
                Assert.Empty
            );
        }

        [Fact]
        public void Create_ReadOnlyCollection_From_Maybe()
        {
            UnitTest(
                _ => "Funk".AsMaybe(),
                m => m.AsReadOnlyCollection(),
                c =>
                {
                    Assert.NotEmpty(c);
                    Assert.Equal(1, c.Count);
                    Assert.Equal("Funk", c.ElementAt(0));
                }
            );
        }

        [Fact]
        public void Create_ReadOnlyCollection_Without_Nulls()
        {
            UnitTest(
                _ => new List<string>{"Funk", null, "Funk not null", null}, 
                l => l.ExceptNulls(),
                c =>
                {
                    Assert.NotEmpty(c);
                    Assert.Equal(2, c.Count);
                    Assert.Equal("Funk not null", c.ElementAt(1));
                }
            );
        }

        [Fact]
        public void Create_ReadOnlyCollection_Without_Nulls_Nullable()
        {
            UnitTest(
                _ => new List<double?> { 0.11, null, null, 0.2 },
                l => l.ExceptNulls(),
                c =>
                {
                    Assert.NotEmpty(c);
                    Assert.Equal(2, c.Count);
                    Assert.Equal(0.2, c.ElementAt(1));
                }
            );
        }

        [Fact]
        public void Create_ReadOnlyCollection_From_Null_Enumerable()
        {
            UnitTest(
                _ => default(List<string>),
                l => l.ExceptNulls(),
                Assert.Empty
            );
        }

        [Fact]
        public void Create_ReadOnlyCollection_From_Maybes()
        {
            UnitTest(
                _ => new List<Maybe<string>>{"Funk".AsMaybe(), default(string).AsMaybe(), "Funky".AsMaybe()},
                l => l.Flatten(),
                c =>
                {
                    Assert.NotEmpty(c);
                    Assert.Equal(2, c.Count);
                    Assert.Equal("Funky", c.ElementAt(1));
                }
            );
        }

        [Fact]
        public void Create_ReadOnlyCollection_From_Null_Maybe_Enumerable()
        {
            UnitTest(
                _ => default(List<Maybe<string>>),
                l => l.Flatten(),
                Assert.Empty
            );
        }

        [Fact]
        public void Create_ReadOnlyCollection_From_Enumerable_Of_Enumerables()
        {
            UnitTest(
                _ => default(List<List<string>>),
                l => l.Flatten(),
                Assert.Empty
            );
        }

        [Fact]
        public void Create_ReadOnlyCollection_From_Enumerable_Of_Combined_Enumerables()
        {
            UnitTest(
                _ => new List<List<string>>{new List<string>{"Harun"}, null, new List<string>{"Funk", null, "Funky"}}, 
                l => l.Flatten(),
                c =>
                {
                    Assert.NotEmpty(c);
                    Assert.Equal(3, c.Count);
                    Assert.Equal("Funky", c.ElementAt(2));
                }
            );
        }

        [Fact]
        public void Create_ReadOnlyCollection_From_Enumerable_Of_Combined_Enumerables_Of_Nullables()
        {
            UnitTest(
                _ => new List<List<double?>> { new List<double?> { 0.1 }, null, new List<double?> { 0.2, null, 0.22 } },
                l => l.Flatten(),
                c =>
                {
                    Assert.NotEmpty(c);
                    Assert.Equal(3, c.Count);
                    Assert.Equal(0.22, c.ElementAt(2));
                }
            );
        }

        [Fact]
        public void Create_ReadOnlyCollection_From_Maybes_With_Merging()
        {
            UnitTest(
                _ => Record.Create("Funky Funk".AsMaybe(), new List<Maybe<string>> { "Funk".AsMaybe(), default(string).AsMaybe(), "Funky".AsMaybe() }),
                r => r.Item1.FlatMerge(r.Item2),
                c =>
                {
                    Assert.NotEmpty(c);
                    Assert.Equal(3, c.Count);
                    Assert.Equal("Funk", c.ElementAt(1));
                }
            );
        }

        [Fact]
        public void Create_ReadOnlyCollection_From_Maybes_With_Merging_With_Null_Enumerable()
        {
            UnitTest(
                _ => Record.Create("Funky Funk".AsMaybe(), default(List<Maybe<string>>)),
                r => r.Item1.FlatMerge(r.Item2),
                c =>
                {
                    Assert.NotEmpty(c);
                    Assert.Equal(1, c.Count);
                    Assert.Equal("Funky Funk", c.ElementAt(0));
                }
            );
        }

        [Fact]
        public void Create_Maybe_Of_Empty_Enumerable()
        {
            UnitTest(
                _ => new List<string>().ToNotEmptyCollection(), 
                m => m.Map(c => c.ElementAt(2)),
                s => Assert.True(s.IsEmpty)
            );
        }

        [Fact]
        public void Create_Maybe_Of_Last_Item_In_Enumerable()
        {
            UnitTest(
                _ => new List<string>(),
                m => m.AsLastOrDefault(),
                s => Assert.True(s.IsEmpty)
            );
        }

        [Fact]
        public void Create_Maybe_Of_First_Item_In_Enumerable()
        {
            UnitTest(
                _ => new List<string>{null, "Funk"},
                m => m.AsFirstOrDefault(),
                s => Assert.True(s.IsEmpty)
            );
        }

        [Fact]
        public void Create_Maybe_Of_Enumerable_With_Predicate()
        {
            UnitTest(
                _ => new List<string> { null, "Funk", "Funky" },
                m => m.WhereOrDefault(s => s.SafeEquals("Funky")),
                s =>
                {
                    Assert.False(s.IsEmpty);
                    Assert.Equal("Funky", s.UnsafeGet().ElementAt(0));
                }
            );
        }

        [Fact]
        public void Create_Maybe_Of_Enumerable_Without_Predicate()
        {
            UnitTest(
                _ => new List<string> { null, "Funk", "Funky" },
                m => m.WhereOrDefault(),
                s =>
                {
                    Assert.False(s.IsEmpty);
                    Assert.Equal("Funky", s.UnsafeGet().ElementAt(2));
                }
            );
        }

        [Fact]
        public void Conditional_Split()
        {
            UnitTest(
                _ => new List<string> { null, "Funk", "Funky", "Harun", "Bosnia" },
                m => m.ExceptNulls().ConditionalSplit(s => s.Contains("Funk")),
                r =>
                {
                    Assert.Equal(2, r.Item1.Count);
                    Assert.Equal(2, r.Item2.Count);
                }
            );
        }
    }
}
