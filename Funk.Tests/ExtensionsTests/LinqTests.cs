using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Funk.Tests
{
    public class LinqTests : Test
    {
        [Fact]
        public void Maybe_Select()
        {
            UnitTest(
                _ => "Harun".AsMaybe(),
                m => from s in m select s.Split("r"),
                r => Assert.Equal("Ha", r.UnsafeGet().First())
            );
        }

        [Fact]
        public void Maybe_SelectMany()
        {
            UnitTest(
                _ => new List<string>{"Harun", "Funk"}.AsMaybe(),
                m =>
                {
                    return 
                        from s in m
                        from i in s.WhereOrDefault(i => i.SafeEquals("H"))
                        where i.IsEmpty()
                        select i;
                },
                r => Assert.True(r.IsEmpty)
            );
        }

        [Fact]
        public void Maybe_SelectMany_Explicit()
        {
            UnitTest(
                _ => new List<string> { "Harun", "Funk" }.AsMaybe(),
                m =>
                {
                    return
                        from s in m
                        from i in s.AsLastOrDefault(i => true)
                        let may = s.AsFirstOrDefault()
                        select may;
                },
                r =>
                {
                    Assert.True(r.NotEmpty);
                    Assert.Equal("Harun", r.UnsafeGet());
                }
            );
        }

        [Fact]
        public void Exc_SelectMany()
        {
            UnitTest(
                _ => Exc.Success<string, Exception>("Harun"),
                e =>
                {
                    return
                        from s in e
                        from f in Exc.Success<string, Exception>($"{s} Cerim")
                        where f.SafeNotEquals("Harun")
                        select f;
                },
                r =>
                {
                    Assert.True(r.IsSuccess);
                    Assert.Equal("Harun Cerim", r.Success.UnsafeGet());
                }
            );
        }

        [Fact]
        public void Exc_SelectMany_Explicit()
        {
            UnitTest(
                _ => Exc.Success<string, Exception>("Harun"),
                e =>
                {
                    return
                        from s in e
                        from f in Exc.Failure<string, Exception>(new Exception().ToEnumerableException(s))
                        let ex = s.Split("a").First()
                        select Exc.Success<string, Exception>(ex);
                },
                r =>
                {
                    Assert.True(r.IsFailure);
                    Assert.Equal("Harun", r.Failure.UnsafeGet().Message);
                }
            );
        }
    }
}
