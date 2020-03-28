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
                e => e.ToEnumerableException(),
                e =>
                {
                    Assert.NotEmpty(e);
                    Assert.IsType<FunkException>(e.Root.UnsafeGet());
                    Assert.Single(e);
                }
            );
        }

        [Fact]
        public void Create_From_Exception_Throws()
        {
            UnitTest(
                _ => new FunkException("Funk"),
                e => act(() => throw e.ToEnumerableException()),
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
                e => e.ToEnumerableException().MapWith(_ => new EmptyValueException("Empty :(")),
                e =>
                {
                    Assert.Equal(2, e.Count());
                    Assert.IsType<FunkException>(e.ElementAt(0));
                    var action = act(() => throw e.Nested.UnsafeGet().ElementAt(1));
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
                e => e.ToEnumerableException().MapWithMany(_ => new List<FunkException>
                {
                    new EmptyValueException("Empty :("),
                    new EmptyValueException("Empty again :/"),
                    new UnhandledValueException("You should handle this."),
                    new UnhandledValueException("Again the same issue ¯\\_(ツ)_/¯."),
                }),
                e =>
                {
                    Assert.Equal(5, e.Count());
                    Assert.Equal("Funk", e.Root.UnsafeGet().Message);
                    foreach (var rec in e.Select((exe, counter) => rec(exe, counter)))
                    {
                        if (rec.Item2.SafeEquals(0))
                        {
                            Assert.IsType<FunkException>(rec.Item1);
                        }
                        else if (rec.Item2.SafeEqualsToAny(1,2))
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
