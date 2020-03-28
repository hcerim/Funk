using System;
using Funk.Exceptions;
using Funk.Extensions;
using Xunit;
using static Funk.Prelude;

namespace Funk.Tests.TypesTests
{
    public class ExcTests : Test
    {
        [Fact]
        public void Create_Exceptional()
        {
            UnitTest(
                _ => Exc.Create<string, ArgumentException>(__ => GetNameById("Funk12")),
                e => e.Failure,
                f =>
                {
                    Assert.True(f.NotEmpty);
                    Assert.Empty(f.UnsafeGet());
                    var action = act(() => throw f.UnsafeGet());
                    Assert.Throws<EnumerableException<ArgumentException>>(action);
                }
            );
        }

        [Fact]
        public void Create_Exceptional_Throws()
        {
            UnitTest(
                _ => "Funk12",
                s => act(() => Exc.Create<string, EmptyValueException>(_ => GetNameById(s))),
                a => Assert.Throws<ArgumentException>(a)
            );
        }

        [Fact]
        public void Create_Exceptional_Throws_Recover()
        {
            UnitTest(
                _ => "Funk12",
                s => Exc.Create<string, ArgumentException>(_ => GetNameById(s)).RecoverOnFailure(e => GetNameById("Funk123")),
                e =>
                {
                    Assert.True(e.IsSuccess);
                    Assert.Equal("Harun", e.Success.UnsafeGet());
                }
            );
        }

        private static string GetNameById(string id)
        {
            if (id.SafeEquals("Funk123"))
            {
                return "Harun";
            }
            throw new ArgumentException("Invalid id");
        }
    }
}
