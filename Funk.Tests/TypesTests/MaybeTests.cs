using System;
using FsCheck.Xunit;
using Xunit;
using static Funk.Prelude;

namespace Funk.Tests
{
    public partial class MaybeTests : Test
    {
        [Fact]
        public void Create_Maybe_With_Value()
        {
            UnitTest(
                _ => "Funk",
                s =>
                {
                    var maybe = s.AsMaybe().Map(ss => $"{s} is here");
                    var result = maybe.Match(
                        _ => "Funk is empty",
                        ss => ss
                    );
                    return result;
                },
                r => Assert.Equal("Funk is here", r)
            );
        }

        [Fact]
        public void Create_Maybe_With_Empty_String_Incorrect()
        {
            UnitTest(
                _ => "",
                s =>
                {
                    var maybe = s.AsMaybe().Map(ss => $"{s} is here");
                    var result = maybe.Match(
                        _ => "Funk is empty",
                        ss => ss
                    );
                    return result;
                },
                r => Assert.Equal(" is here", r)
            );
        }

        [Fact]
        public void Create_Maybe_With_Empty_String_Correct()
        {
            UnitTest(
                _ => "",
                s =>
                {
                    var maybe = s.AsNotEmptyString().Map(ss => $"{s} is here");
                    var result = maybe.Match(
                        _ => "Funk is empty",
                        ss => ss
                    );
                    return result;
                },
                r => Assert.Equal("Funk is empty", r)
            );
        }

        [Fact]
        public void Create_Maybe_With_Empty_Value()
        {
            UnitTest(
                _ => (object)null,
                s =>
                {
                    var maybe = s.AsMaybe().Map(ss => $"{s} is here");
                    return maybe.Match(
                        _ => "Funk is empty",
                        ss => ss
                    );
                },
                r => Assert.Equal("Funk is empty", r)
            );
        }

        [Fact]
        public void Create_Maybe_From_Maybe_With_Empty_Value()
        {
            UnitTest(
                _ => (object)null,
                s =>
                {
                    var maybe = s.AsMaybe().FlatMap(ss => 2.AsMaybe());
                    return act(() => maybe.Match(
                        v => throw new Exception("Empty maybe."),
                        _ => 1
                    ));
                },
                a => Assert.Throws<Exception>(a)
            );
        }

        [Fact]
        public void Create_Maybe_From_Maybe_With_Not_Empty_Value()
        {
            UnitTest(
                _ => "Funk",
                s =>
                {
                    var maybe = s.AsMaybe().FlatMap(ss => 2.AsMaybe());
                    return maybe.Match(
                        v => 1,
                        _ => new Exception("Empty maybe.")
                    );
                },
                r => Assert.Equal(1, r)
            );
        }

        [Fact]
        public void Create_Maybe_Unsafe_Get_Fail()
        {
            UnitTest(
                _ => (object)null,
                s =>
                {
                    var maybe = s.AsMaybe().FlatMap(ss => 2.AsMaybe());
                    return act(() => maybe.UnsafeGet());
                },
                a => Assert.Throws<EmptyValueException>(a)
            );
        }

        [Property(Arbitrary = new[] { typeof(ArbitraryLifter) })]
        public void RightIdentity(Maybe<object> maybe)
        {
            UnitTest(
                _ => maybe,
                m => m.SafeEquals(m.FlatMap(v => v.AsMaybe())),
                Assert.True
            );
        }

        [Property]
        public void LeftIdentity(object obj)
        {
            UnitTest(
                _ => func((object o) => o.AsMaybe()),
                f => obj.AsMaybe().FlatMap(f).SafeEquals(f(obj)),
                Assert.True
            );
        }

        [Property(Arbitrary = new[] { typeof(ArbitraryLifter) })]
        public void Associativity(Maybe<int> maybe)
        {
            UnitTest(
                _ => rec(func((int o) => may(o * 3)), func((int o) => may(o * 4))),
                r => maybe.FlatMap(r.Item1).FlatMap(r.Item2).SafeEquals(maybe.FlatMap(v => r.Item1(v).FlatMap(r.Item2))),
                Assert.True
            );
        }
    }
}
