using System;
using Xunit;
using static Funk.Prelude;

namespace Funk.Tests
{
    public class ActionTests : Test
    {
        [Fact]
        public void Action_of_3_To_Func()
        {
            UnitTest(
                _ => act((int a, string b, double c) => { }),
                a => a.ToFunc(),
                f => Assert.IsType<Func<int, string, double, Unit>>(f)
            );
        }

        [Fact]
        public void Action_of_2_To_Func()
        {
            UnitTest(
                _ => act((int a, string b) => throw new Exception("Funk")),
                a => a.ToFunc(),
                f => Assert.IsType<Func<int, string, Unit>>(f)
            );
        }
    }
}
