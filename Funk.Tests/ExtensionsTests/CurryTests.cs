using Xunit;

namespace Funk.Tests
{
    public class CurryTests : Test
    {
        [Fact]
        public void Curry_2_Param_Func()
        {
            UnitTest(
                _ => func((int a, int b) => a + b),
                f => f.Curry(),
                curried => Assert.Equal(5, curried(2)(3))
            );
        }

        [Fact]
        public void Curry_3_Param_Func()
        {
            UnitTest(
                _ => func((int a, int b, int c) => a + b + c),
                f => f.Curry(),
                curried => Assert.Equal(6, curried(1)(2)(3))
            );
        }

        [Fact]
        public void Curry_4_Param_Func()
        {
            UnitTest(
                _ => func((int a, int b, int c, int d) => a + b + c + d),
                f => f.Curry(),
                curried => Assert.Equal(10, curried(1)(2)(3)(4))
            );
        }

        [Fact]
        public void Curry_5_Param_Func()
        {
            UnitTest(
                _ => func((int a, int b, int c, int d, int e) => a + b + c + d + e),
                f => f.Curry(),
                curried => Assert.Equal(15, curried(1)(2)(3)(4)(5))
            );
        }

        [Fact]
        public void Curry_2_Param_Func_Produces_Same_Result()
        {
            UnitTest(
                _ => func((int a, int b) => a * b),
                f => (Original: f, Curried: f.Curry()),
                r => Assert.Equal(r.Original(3, 7), r.Curried(3)(7))
            );
        }

        [Fact]
        public void Curry_2_Param_Partial_Application()
        {
            UnitTest(
                _ => func((string prefix, string name) => $"{prefix} {name}").Curry(),
                curried => curried("Hello"),
                greet => Assert.Equal("Hello World", greet("World"))
            );
        }

        [Fact]
        public void Curry_3_Param_Partial_Application()
        {
            UnitTest(
                _ => func((int a, int b, int c) => a * b + c).Curry(),
                curried => curried(2)(3),
                partial => Assert.Equal(10, partial(4))
            );
        }

        [Fact]
        public void Curry_4_Param_Partial_Application()
        {
            UnitTest(
                _ => func((int a, int b, int c, int d) => a + b + c + d).Curry(),
                curried => curried(10)(20),
                partial => Assert.Equal(60, partial(10)(20))
            );
        }

        [Fact]
        public void Curry_5_Param_Partial_Application()
        {
            UnitTest(
                _ => func((string a, string b, string c, string d, string e) => $"{a}-{b}-{c}-{d}-{e}").Curry(),
                curried => curried("a")("b")("c"),
                partial => Assert.Equal("a-b-c-d-e", partial("d")("e"))
            );
        }
    }
}
