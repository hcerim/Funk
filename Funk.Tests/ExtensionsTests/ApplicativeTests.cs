using System;
using FsCheck.Xunit;
using Xunit;
using static Funk.Prelude;

namespace Funk.Tests.ExtensionsTests
{
    public class ApplicativeTests : Test
    {
        [Property]
        public void ApplicativeLawForMaybe(int first, int second)
        {
            UnitTest(
                _ => func((int a, int b) => a + b),
                f => rec(first.AsMaybe().Map(f.Apply).Apply(second), f.AsMaybe().Apply(first).Apply(second)),
                r => Assert.Equal(r.Item1, r.Item2)
            );
        }

        [Property]
        public void ApplicativeLawForExc(int first, int second)
        {
            UnitTest(
                _ => func((int a, int b) => a / b),
                f => rec(success<int, Exception>(first).Map(f.Apply).Apply(second), success<Func<int, int, int>, Exception>(f).Apply(first).Apply(second)),
                r => Assert.Equal(r.Item1.Success, r.Item2.Success)
            );
        }
    }
}
