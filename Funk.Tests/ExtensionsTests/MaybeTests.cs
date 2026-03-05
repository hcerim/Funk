using System.Threading.Tasks;
using Xunit;

namespace Funk.Tests
{
    public partial class MaybeTests
    {
        [Fact]
        public void Get_Maybe_Or_Else()
        {
            UnitTest(
                _ => may(default(string)),
                m => m.GetOr(_ => "Funk"),
                s => Assert.Equal("Funk", s)
            );
        }

        [Fact]
        public void Get_Maybe_Or_Null()
        {
            UnitTest(
                _ => may(default(object)),
                m => m.GetOrDefault(),
                Assert.Null
            );
        }

        [Fact]
        public void Get_Maybe_Or()
        {
            UnitTest(
                _ => may(default(string)),
                m => m.Or(_ => may("Harun")),
                s => Assert.Equal("Harun", s.UnsafeGet())
            );
        }

        [Fact]
        public void Get_Maybe_With_Nullable()
        {
            UnitTest(
                _ =>
                {
                    int? number = 1;
                    return may(number);
                },
                m => m.Or(_ => may(2)).Or(_ => may(3)),
                s => Assert.Equal(1, s.UnsafeGet())
            );
        }

        [Fact]
        public async Task Get_Maybe_With_Nullable_Empty()
        {
            await UnitTestAsync(
                _ => new Maybe<int?>().ToTask(), 
                m => m.Map().Or(_ => new Maybe<int?>().Map()).Or(_ => may((int?)2)).OrAsync(_ => may((int?)1).ToTask()),
                s => Assert.Equal(2, s.GetOr(_ => 1))
            );
        }
        [Fact]
        public void Get_Maybe_With_Nullable_Empty_With_Default()
        {
            UnitTest(
                _ => may((int?)null),
                m => m.Or(_ => may(default(int?))).Or(_ => may(default(int?))),
                s => Assert.True(s.IsEmpty)
            );
        }

        [Fact]
        public void Merge_All_NonEmpty()
        {
            UnitTest(
                _ => may(1),
                m => m.Merge(may(2)),
                r =>
                {
                    Assert.True(r.NotEmpty);
                    Assert.Equal(2, r.UnsafeGet().Count);
                    Assert.Equal(1, r.UnsafeGet()[0]);
                    Assert.Equal(2, r.UnsafeGet()[1]);
                }
            );
        }

        [Fact]
        public void Merge_One_Empty()
        {
            UnitTest(
                _ => may(1),
                m => m.Merge(Maybe.Empty<int>()),
                r => Assert.True(r.IsEmpty)
            );
        }

        [Fact]
        public void MergeRange_All_NonEmpty()
        {
            UnitTest(
                _ => may("a"),
                m => m.MergeRange([may("b"), may("c")]),
                r =>
                {
                    Assert.True(r.NotEmpty);
                    Assert.Equal(3, r.UnsafeGet().Count);
                }
            );
        }

        [Fact]
        public void MergeRange_Any_Empty()
        {
            UnitTest(
                _ => may("a"),
                m => m.MergeRange([may("b"), may<string>(null)]),
                r => Assert.True(r.IsEmpty)
            );
        }
    }
}
