using System.Collections.Generic;
using System.Linq;
using Funk.Exceptions;
using Xunit;
using static Funk.Prelude;

namespace Funk.Tests
{
    public partial class EnumerableExceptionTests : Test
    {
        [Fact]
        public void Create_From_Exception()
        {
            UnitTest(
                _ => new FunkException("Funk"),
                e => e.ToException(),
                Assert.Empty
            );
        }

        [Fact]
        public void Create_From_Exception_Throws()
        {
            UnitTest(
                _ => new FunkException("Funk"),
                e => act(() => throw e.ToException()),
                a =>
                {
                    var exception = Assert.Throws<EnumerableException<FunkException>>(a);
                    Assert.Equal("Funk", exception.Message);
                }
            );
        }

        [Fact]
        public void Create_From_Exception_Contains()
        {
            UnitTest(
                _ => new FunkException("Funk"),
                e => e.ToException().MapWith(_ => new EmptyValueException("Empty :(")),
                e =>
                {
                    Assert.Single(e);
                    Assert.IsType<EmptyValueException>(e.ElementAt(0));
                    var action = act(() => throw e.Nested.UnsafeGet().First());
                    var exception = Assert.Throws<EmptyValueException>(action);
                    Assert.Equal("Empty :(", exception.Message);
                }
            );
        }

        [Fact]
        public void Create_From_Exception_Contains_Many()
        {
            UnitTest(
                _ => new FunkException("Funk"),
                e => e.ToException().MapWith(_ => new List<FunkException>
                {
                    new EmptyValueException("Empty :("),
                    new EmptyValueException("Empty again :/"),
                    new UnhandledValueException("You should handle this."),
                    new UnhandledValueException("Again the same issue ¯\\_(ツ)_/¯."),
                }),
                e =>
                {
                    Assert.Equal(4, e.Count());
                    foreach (var rec in e.Select((exe, counter) => rec(exe, counter)))
                    {
                        if (rec.Item2.SafeEqualsToAny(0, 1))
                        {
                            Assert.IsType<EmptyValueException>(rec.Item1);
                        }
                        else
                        {
                            Assert.IsType<UnhandledValueException>(rec.Item1);
                        }
                    }
                }
            );
        }
    }
}
