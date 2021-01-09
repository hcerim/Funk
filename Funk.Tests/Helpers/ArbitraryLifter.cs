using System;
using FsCheck;
using static Funk.Prelude;

namespace Funk.Tests
{
    public static class ArbitraryLifter
    {
        public static Arbitrary<Maybe<T>> CreateRandomMaybe<T>()
        {
            return Arb.Generate<bool>().SelectMany(
                isSome => Arb.Generate<T>(),
                (isSome, val) => isSome ? may(val) : empty
            ).ToArbitrary();
        }

        public static Arbitrary<Exc<T, E>> CreateRandomExc<T, E>() where E : Exception
        {
            return Arb.Generate<bool>().SelectMany(
                isSome => Arb.Generate<T>(),
                (isSome, val) => isSome ? success<T, E>(val) : empty
            ).ToArbitrary();
        }
    }
}
