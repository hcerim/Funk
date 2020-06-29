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

        [Property(Arbitrary = new[] { typeof(ArbitraryLifter) })]
        public void ApplicativeContractLawForExc(Exc<int, Exception> first, Exc<int, Exception> second, Exc<int, Exception> third)
        {
            UnitTest(
                _ => func((int a, int b, int c) => a / b * c),
                f => rec(first.Map(f.Apply).ApplyContract(second).ApplyContract(third), success<Func<int, int, int, int>, Exception>(f).ApplyContract(first).ApplyContract(second).ApplyContract(third)),
                r => Assert.Equal(r.Item1.Success, r.Item2.Success)
            );
        }

        [Fact]
        public void ApplicativeContract()
        {
            UnitTest(
                _ => func((int a, int b, int c) => a + b + c),
                f => success<Func<int, int, int, int>, Exception>(f)
                    .ApplyContract(failure<int, Exception>(new DivideByZeroException("One")))
                    .ApplyContract(failure<int, Exception>(new InvalidCastException("Two")))
                    .ApplyContract(failure<int, Exception>(new InvalidOperationException("Three"))),
                r =>
                {
                    Assert.True(r.Failure.NotEmpty);
                    Assert.Equal(3, r.NestedFailures.UnsafeGet().Count);
                    Assert.Equal("One", r.RootFailure.UnsafeGet().Message);
                }
            );
        }
    }
}
