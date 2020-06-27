using System;
using FsCheck.Xunit;
using Xunit;
using static Funk.Prelude;

namespace Funk.Tests
{
    public class ApplicativeTests : Test
    {
        [Property(Arbitrary = new[] { typeof(ArbitraryLifter) })]
        public void ApplicativeLawForMaybe(Maybe<int> first, Maybe<int> second)
        {
            UnitTest(
                _ => func((int a, int b) => a + b),
                f => rec(first.Map(f.Apply).Apply(second), f.AsMaybe().Apply(first).Apply(second)),
                r => Assert.Equal(r.Item1, r.Item2)
            );
        }

        [Property(Arbitrary = new[] { typeof(ArbitraryLifter) })]
        public void ApplicativeLawForExc(Exc<int, DivideByZeroException> first, Exc<int, DivideByZeroException> second)
        {
            UnitTest(
                _ => func((int a, int b) => a / b),
                f => rec(first.Map(f.Apply).Apply(second), success<Func<int, int, int>, DivideByZeroException>(f).Apply(first).Apply(second)),
                r => Assert.Equal(r.Item1.Success, r.Item2.Success)
            );
        }
    }
}
