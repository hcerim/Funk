using System;
using Funk.Exceptions;
using Xunit;
using static Funk.Prelude;

namespace Funk.Tests
{
    public partial class MaybeTests : Test
    {
        [Fact]
        public void Create_Maybe_With_Value()
        {
            UnitTest(_ => "Funk", 
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
        public void Create_Maybe_With_Empty_Value()
        {
            UnitTest(_ => (object) null,
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
            UnitTest(_ => (object)null,
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
            UnitTest(_ => "Funk",
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
            UnitTest(_ => (object)null,
                s =>
                {
                    var maybe = s.AsMaybe().FlatMap(ss => 2.AsMaybe());
                    return act(() => maybe.UnsafeGet());
                },
                a => Assert.Throws<EmptyValueException>(a)
            );
        }
    }
}
