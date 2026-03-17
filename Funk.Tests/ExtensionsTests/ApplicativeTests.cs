using System;
using FsCheck.Xunit;
using Xunit;

namespace Funk.Tests
{
    public class ApplicativeTests : Test
    {
        [Property(Arbitrary = [typeof(ArbitraryLifter)])]
        public void ApplicativeLawForMaybe(Maybe<int> first, Maybe<int> second)
        {
            UnitTest(
                _ => func((int a, int b) => a + b),
                f => rec(first.Map(f.Apply).Apply(second), f.AsMaybe().Apply(first).Apply(second)),
                r => Assert.Equal(r.Item1, r.Item2)
            );
        }

        [Property(Arbitrary = [typeof(ArbitraryLifter)])]
        public void ApplicativeLawForExc(Exc<int, DivideByZeroException> first, Exc<int, DivideByZeroException> second)
        {
            UnitTest(
                _ => func((int a, int b) => a / b),
                f => rec(first.Map(f.Apply).Apply(second), success<Func<int, int, int>, DivideByZeroException>(f).Apply(first).Apply(second)),
                r => Assert.Equal(r.Item1.Success, r.Item2.Success)
            );
        }

        [Property(Arbitrary = [typeof(ArbitraryLifter)])]
        public void ApplicativeContractLawForExc(Exc<int, Exception> first, Exc<int, Exception> second, Exc<int, Exception> third)
        {
            UnitTest(
                _ => func((int a, int b, int c) => a / b * c),
                f => rec(first.Map(f.Apply).Validate(second).Validate(third), success<Func<int, int, int, int>, Exception>(f).Validate(first).Validate(second).Validate(third)),
                r => Assert.Equal(r.Item1.Success, r.Item2.Success)
            );
        }

        [Fact]
        public void ApplicativeContract()
        {
            UnitTest(
                _ => func((int a, int b, int c) => a + b + c),
                f => success<Func<int, int, int, int>, Exception>(f)
                    .Validate(failure<int, Exception>(new DivideByZeroException("One")))
                    .Validate(success<int, Exception>(2))
                    .Validate(failure<int, Exception>(new InvalidOperationException("Three"))),
                r =>
                {
                    Assert.True(r.Failure.NotEmpty);
                    Assert.Equal(2, r.NestedFailures.UnsafeGet().Count);
                    Assert.Equal("One", r.RootFailure.UnsafeGet().Message);
                }
            );
        }

        [Fact]
        public void Validate_All_Success_Returns_Result()
        {
            UnitTest(
                _ => func((int a, int b) => a + b),
                f => success<Func<int, int, int>, Exception>(f)
                    .Validate(success<int, Exception>(3))
                    .Validate(success<int, Exception>(4)),
                r =>
                {
                    Assert.True(r.IsSuccess);
                    Assert.Equal(7, r.UnsafeGetSuccess());
                }
            );
        }

        [Fact]
        public void Validate_Single_Failure_Returns_That_Error()
        {
            UnitTest(
                _ => func((int a, int b) => a + b),
                f => success<Func<int, int, int>, Exception>(f)
                    .Validate(success<int, Exception>(1))
                    .Validate(failure<int, Exception>(new InvalidOperationException("bad"))),
                r =>
                {
                    Assert.True(r.IsFailure);
                    Assert.Equal(1, r.NestedFailures.UnsafeGet().Count);
                    Assert.Equal("bad", r.RootFailure.UnsafeGet().Message);
                }
            );
        }

        [Fact]
        public void Validate_All_Failures_Collects_All_Errors()
        {
            UnitTest(
                _ => func((int a, int b) => a + b),
                f => success<Func<int, int, int>, Exception>(f)
                    .Validate(failure<int, Exception>(new ArgumentException("first")))
                    .Validate(failure<int, Exception>(new ArgumentException("second"))),
                r =>
                {
                    Assert.True(r.IsFailure);
                    Assert.Equal(2, r.NestedFailures.UnsafeGet().Count);
                    Assert.Equal("first", r.RootFailure.UnsafeGet().Message);
                }
            );
        }

        [Fact]
        public void Validate_With_Empty_Exc_Returns_Empty()
        {
            UnitTest(
                _ => func((int a, int b) => a + b),
                f => success<Func<int, int, int>, Exception>(f)
                    .Validate(Exc.Empty<int, Exception>())
                    .Validate(success<int, Exception>(1)),
                r => Assert.True(r.IsEmpty)
            );
        }

        [Fact]
        public void Validate_4_Params_All_Success()
        {
            UnitTest(
                _ => func((int a, int b, int c, int d) => a + b + c + d),
                f => success<Func<int, int, int, int, int>, Exception>(f)
                    .Validate(success<int, Exception>(1))
                    .Validate(success<int, Exception>(2))
                    .Validate(success<int, Exception>(3))
                    .Validate(success<int, Exception>(4)),
                r =>
                {
                    Assert.True(r.IsSuccess);
                    Assert.Equal(10, r.UnsafeGetSuccess());
                }
            );
        }

        [Fact]
        public void Validate_5_Params_Mixed_Failures()
        {
            UnitTest(
                _ => func((int a, int b, int c, int d, int e) => a + b + c + d + e),
                f => success<Func<int, int, int, int, int, int>, Exception>(f)
                    .Validate(failure<int, Exception>(new Exception("e1")))
                    .Validate(success<int, Exception>(2))
                    .Validate(failure<int, Exception>(new Exception("e3")))
                    .Validate(success<int, Exception>(4))
                    .Validate(failure<int, Exception>(new Exception("e5"))),
                r =>
                {
                    Assert.True(r.IsFailure);
                    Assert.Equal(3, r.NestedFailures.UnsafeGet().Count);
                }
            );
        }

        [Fact]
        public void Validate_Preserves_Error_Messages()
        {
            UnitTest(
                _ => func((string name, int age) => $"{name}:{age}"),
                f => success<Func<string, int, string>, Exception>(f)
                    .Validate(failure<string, Exception>(new ArgumentException("Name is required")))
                    .Validate(failure<int, Exception>(new ArgumentException("Age must be positive"))),
                r =>
                {
                    Assert.True(r.IsFailure);
                    var failures = r.NestedFailures.UnsafeGet();
                    Assert.Equal(2, failures.Count);
                    Assert.Equal("Name is required", r.RootFailure.UnsafeGet().Message);
                    Assert.Contains(failures, e => e.Message == "Age must be positive");
                }
            );
        }

        [Fact]
        public void Apply_Maybe_Both_Empty_Returns_Empty()
        {
            UnitTest(
                _ => Maybe.Empty<Func<int, int>>(),
                f => f.Apply(Maybe.Empty<int>()),
                m => Assert.True(m.IsEmpty)
            );
        }

        [Fact]
        public void Apply_Maybe_Function_Empty_Returns_Empty()
        {
            UnitTest(
                _ => Maybe.Empty<Func<int, int>>(),
                f => f.Apply(Maybe.Create(42)),
                m => Assert.True(m.IsEmpty)
            );
        }

        [Fact]
        public void Apply_Maybe_Argument_Empty_Returns_Empty()
        {
            UnitTest(
                _ => Maybe.Create<Func<int, int>>(x => x * 2),
                f => f.Apply(Maybe.Empty<int>()),
                m => Assert.True(m.IsEmpty)
            );
        }

        [Fact]
        public void Apply_Maybe_Both_Present_Returns_Result()
        {
            UnitTest(
                _ => Maybe.Create<Func<int, int>>(x => x * 2),
                f => f.Apply(Maybe.Create(5)),
                m =>
                {
                    Assert.True(m.NotEmpty);
                    Assert.Equal(10, m.UnsafeGet());
                }
            );
        }

        [Fact]
        public void Validate_Action_All_Success_Executes()
        {
            UnitTest(
                _ => (Action<int, int>)((a, b) => { }),
                f => success<Action<int, int>, Exception>(f)
                    .Validate(success<int, Exception>(3))
                    .Validate(success<int, Exception>(4)),
                r => Assert.True(r.IsSuccess)
            );
        }

        [Fact]
        public void Validate_Action_Failures_Collected()
        {
            UnitTest(
                _ => (Action<int, int>)((a, b) => { }),
                f => success<Action<int, int>, Exception>(f)
                    .Validate(failure<int, Exception>(new Exception("e1")))
                    .Validate(failure<int, Exception>(new Exception("e2"))),
                r =>
                {
                    Assert.True(r.IsFailure);
                    Assert.Equal(2, r.NestedFailures.UnsafeGet().Count);
                }
            );
        }
    }
}
