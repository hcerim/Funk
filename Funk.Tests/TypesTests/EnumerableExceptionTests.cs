using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Funk.Prelude;

namespace Funk.Tests
{
    public class EnumerableExceptionTests : Test
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
        public void Create_From_Exception_Clear()
        {
            UnitTest(
                _ => new FunkException("Funk"),
#pragma warning disable CS0618
                e => e.ToEnumerableException().Clear().SafeCast<EnumerableException<FunkException>>().UnsafeGet(),
#pragma warning restore CS0618
                e =>
                {
                    Assert.Equal("EnumerableException is empty.", e.ToString());
                    Assert.True(e.IsEmpty());
                }
            );
        }

        [Fact]
        public void Create_From_Exception_Throws()
        {
            UnitTest(
                _ => new FunkException("Funk"),
                e => act(() => throw e.ToEnumerableException(e.Message)),
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
                e => e.ToEnumerableException().Add(_ => new EmptyValueException("Empty :(")),
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
                e => e.ToEnumerableException().AddRange(_ => new List<FunkException>
                {
                    new EmptyValueException("Empty :("),
                    new EmptyValueException("Empty again :/"),
                    new UnhandledValueException("You should handle this."),
                    new UnhandledValueException("Again the same issue ¯\\_(ツ)_/¯."),
                }),
                e =>
                {
                    Assert.Equal(5, e.Count);
                    Assert.Equal("Funk", e.Root.UnsafeGet().Message);
                    foreach (var rec in e.Select((exe, counter) => rec(exe, counter)))
                    {
                        if (rec.Item2.SafeEquals(0))
                        {
                            Assert.IsType<FunkException>(rec.Item1);
                        }
                        else if (rec.Item2.SafeEqualsToAny(1, 2))
                        {
                            Assert.IsType<EmptyValueException>(rec.Item1);
                        }
                        else
                        {
                            Assert.IsType<UnhandledValueException>(rec.Item1);
                        }
                    }
                    var dict = e.ToDictionary(f => f.Type).UnsafeGet();
                    var res = dict.Values.Map(v => v.ForEach<FunkException, FunkException>(f => throw f));
                    Assert.True(res.All(ex => ex.IsFailure));
                    Assert.Equal(2, e.OfSafeType<FunkException, EmptyValueException>().UnsafeGet().Count);
                    Assert.Equal(2, e.OfSafeType<FunkException, UnhandledValueException>().UnsafeGet().Count);
                }
            );
        }
    }
}
