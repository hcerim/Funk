using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using static Funk.Prelude;

namespace Funk.Tests
{
    public class EnumerableTests : Test
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public EnumerableTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Create_ReadOnlyCollection_From_Not_Empty_List()
        {
            UnitTest(
                _ => new List<string>{"Funk", "Funky Funk", "Da Funk"}, 
                l => l.Map(),
                c => Assert.Equal("Funky Funk", c.ElementAt(1))
            );
        }

        [Fact]
        public void Create_ReadOnlyCollection_From_Null_List()
        {
            UnitTest(
                _ => default(List<string>),
                l => l.Map(),
                Assert.Empty
            );
        }

        [Fact]
        public void Create_ReadOnlyCollection_From_Empty_Maybe()
        {
            UnitTest(
                _ => default(string).AsMaybe(),
                m => m.AsImmutableList(),
                Assert.Empty
            );
        }

        [Fact]
        public void Create_ReadOnlyCollection_From_Maybe()
        {
            UnitTest(
                _ => "Funk".AsMaybe(),
                m => m.AsImmutableList(),
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
                _ => new List<string>().AsNotEmptyList(), 
                m => m.Map(c => c.ElementAt(2)),
                s => Assert.True(s.IsEmpty)
            );
        }

        [Fact]
        public void Create_Maybe_Of_Last_Item_In_Enumerable()
        {
            UnitTest(
                _ => new List<string>(),
                l => l.AsLastOrDefault(),
                s => Assert.True(s.IsEmpty)
            );
        }

        [Fact]
        public void Create_Maybe_Of_First_Item_In_Enumerable()
        {
            UnitTest(
                _ => list(null, "Funk"),
                l => l.AsFirstOrDefault(),
                s => Assert.True(s.IsEmpty)
            );
        }

        [Fact]
        public void Create_Flatten_List()
        {
            UnitTest(
                _ => list(empty, "Funk".AsMaybe()),
                l => l.First(),
                s => Assert.Equal("Funk", s)
            );
        }

        [Fact]
        public void Create_Maybe_Of_Enumerable_With_Predicate()
        {
            UnitTest(
                _ => new List<string> { null, "Funk", "Funky" },
                l => l.WhereOrDefault(s => s.SafeEquals("Funky")),
                s =>
                {
                    Assert.False(s.IsEmpty);
                    Assert.Equal("Funky", s.UnsafeGet().ElementAt(0));
                }
            );
        }

        [Fact]
        public void Create_Maybe_Of_Empty_Enumerable_With_Predicate()
        {
            UnitTest(
                _ => default(List<string>),
                l => l.WhereOrDefault(s => s.SafeEquals("Funky")),
                s => Assert.True(s.IsEmpty)
            );
        }

        [Fact]
        public void Create_Maybe_Of_Enumerable_Without_Predicate()
        {
            UnitTest(
                _ => new List<string> { null, "Funk", "Funky" },
                l => l.WhereOrDefault(),
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
                l => l.ExceptNulls().ConditionalSplit(s => s.Contains("Funk")),
                r =>
                {
                    Assert.Equal(2, r.Item1.Count);
                    Assert.Equal("Funky", r.Item1.ElementAt(1));
                    Assert.Equal(2, r.Item2.Count);
                    Assert.Equal("Harun", r.Item2.ElementAt(0));
                }
            );
        }

        [Fact]
        public void Enumerable_To_Dictionary()
        {
            UnitTest(
                _ => new List<string> { "Funk", "Funky", "Harun", "Bosnia" },
                l => l.ToDictionary(i => i),
                d =>
                {
                    Assert.Equal(4, d.Count);
                    Assert.Equal("Funky", d["Funky"].ElementAt(0));
                }
            );
        }

        [Fact]
        public void Enumerable_To_Dictionary_Empty()
        {
            UnitTest(
                _ => default(List<string>),
                l => l.ToDictionary(i => i),
                Assert.Empty
            );
        }

        [Fact]
        public void Enumerable_To_Record()
        {
            UnitTest(
                _ => new List<string> { "Funk", "Funky", "Harun", "Bosnia" },
                l => l.ToRecordList(i => i),
                r =>
                {
                    Assert.Equal(4, r.Count);
                    Assert.True(r.AsFirstOrDefault(record => record.Item2.Contains("Funky")).NotEmpty);
                }
            );
        }

        [Fact]
        public void Enumerable_MapReduce()
        {
            UnitTest(
                _ => new List<string> { "Funk", "Funky", "Harun", "Bosnia" },
                l => l.MapReduce(
                    s => new
                    {
                        Name = s,
                        InitialLetter = s.FirstOrDefault()
                    },
                    (first, second) => new
                    {
                        second.Name,
                        first.InitialLetter
                    }
                ),
                o =>
                {
                    Assert.True(o.NotEmpty);
                    Assert.Equal('F', o.UnsafeGet().InitialLetter);
                    Assert.Equal("Bosnia", o.UnsafeGet().Name);
                }
            );
        }

        [Fact]
        public void Enumerable_MapFold()
        {
            UnitTest(
                _ => new List<Maybe<string>> { "Funk", "Funky", "Harun", null },
                l => l.MapFold(
                    s => new
                    {
                        Name = s,
                        InitialLetter = s.FirstOrDefault()
                    },
                    (first, second) => new
                    {
                        second.Name,
                        first.InitialLetter
                    }
                ),
                o =>
                {
                    Assert.True(o.NotEmpty);
                    Assert.Equal('F', o.UnsafeGet().InitialLetter);
                    Assert.Equal("Harun", o.UnsafeGet().Name);
                }
            );
        }

        [Fact]
        public void Enumerable_FlatMapReduce()
        {
            UnitTest(
                _ => new List<string> { "Funk", "Funky", "Harun", "Bosnia" },
                l => l.FlatMapReduce(
                    s => new List<Record<string, char>>
                    {
                        rec(s, s.FirstOrDefault())
                    },
                    (first, second) => rec(second.Item1, first.Item2)
                ),
                o =>
                {
                    Assert.True(o.NotEmpty);
                    Assert.Equal('F', o.UnsafeGet().Item2);
                    Assert.Equal("Bosnia", o.UnsafeGet().Item1);
                }
            );
        }

        [Fact]
        public void Enumerable_FlatMapFold()
        {
            UnitTest(
                _ => new List<Maybe<string>> { "Funk", "Funky", "Harun", null },
                l => l.FlatMapFold(
                    s => new List<Record<string, char>>
                    {
                        rec(s, s.FirstOrDefault())
                    },
                    (first, second) => rec(second.Item1, first.Item2)
                ),
                o =>
                {
                    Assert.True(o.NotEmpty);
                    Assert.Equal('F', o.UnsafeGet().Item2);
                    Assert.Equal("Harun", o.UnsafeGet().Item1);
                }
            );
        }

        [Fact]
        public void Enumerable_Fold()
        {
            UnitTest(
                _ => new List<Maybe<string>> { "Funk", "Funky", null, null },
                l => l.Map().Reverse().Fold((a, b) => $"{a} {b}"),
                m =>
                {
                    Assert.True(m.NotEmpty);
                    Assert.Equal("Funky Funk", m.UnsafeGet());
                }
            );
        }

        [Fact]
        public void Enumerable_MapReduce_Empty()
        {
            UnitTest(
                _ => default(List<string>),
                l => l.MapReduce(
                    s => new
                    {
                        Name = s,
                        InitialLetter = s.FirstOrDefault()
                    },
                    (first, second) => new
                    {
                        second.Name,
                        first.InitialLetter
                    }
                ),
                o => Assert.True(o.IsEmpty)
            );
        }

        [Fact]
        public void ForEach_Enumerable_Failure()
        {
            UnitTest(
                _ => list("Harun", "Funk", "Funky"),
                l => l.ForEach<string, FunkException>(i => throw new FunkException(i)),
                e => Assert.True(e.IsFailure)
            );
        }

        [Fact]
        public async Task ForEach_Enumerable_Success()
        {
            await UnitTestAsync(
                _ => result(list("Harun", "Funk", "Funky")),
                l => l.ForEachAsync<string, FunkException>(i => run(act(() => _testOutputHelper.WriteLine(i)))),
                e => Assert.True(e.IsSuccess)
            );
        }
    }
}
