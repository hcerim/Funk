using Xunit;

namespace Funk.Tests
{
    public class PartialTests : Test
    {
        [Fact]
        public void Apply_Func_1_Immediate()
        {
            UnitTest(
                _ => func((int x) => x * 2),
                f => f.Apply(5),
                result => Assert.Equal(10, result)
            );
        }

        [Fact]
        public void Apply_Action_1_Returns_Unit()
        {
            UnitTest(
                _ => act((int _) => { }),
                a => a.Apply(1),
                result => Assert.Equal(Unit.Value, result)
            );
        }

        [Fact]
        public void Partial_Apply_Func_2()
        {
            UnitTest(
                _ => func((int a, int b) => a + b),
                f => f.Apply(3),
                partial => Assert.Equal(7, partial(4))
            );
        }

        [Fact]
        public void Partial_Apply_Action_2()
        {
            UnitTest(
                _ =>
                {
                    var captured = 0;
                    var action = act((int a, int b) => { captured = a + b; });
                    return (action, getCaptured: func(() => captured));
                },
                r =>
                {
                    var partial = r.action.Apply(3);
                    partial(7);
                    return r.getCaptured();
                },
                captured => Assert.Equal(10, captured)
            );
        }

        [Fact]
        public void Partial_Apply_Func_3()
        {
            UnitTest(
                _ => func((int a, int b, int c) => a + b + c),
                f => f.Apply(1),
                partial => Assert.Equal(6, partial(2, 3))
            );
        }

        [Fact]
        public void Partial_Apply_Action_3()
        {
            UnitTest(
                _ =>
                {
                    var captured = string.Empty;
                    var action = act((string a, string b, string c) => { captured = $"{a}-{b}-{c}"; });
                    return (action, getCaptured: func(() => captured));
                },
                r =>
                {
                    var partial = r.action.Apply("x");
                    partial("y", "z");
                    return r.getCaptured();
                },
                captured => Assert.Equal("x-y-z", captured)
            );
        }

        [Fact]
        public void Partial_Apply_Func_4()
        {
            UnitTest(
                _ => func((int a, int b, int c, int d) => a * b * c * d),
                f => f.Apply(2),
                partial => Assert.Equal(120, partial(3, 4, 5))
            );
        }

        [Fact]
        public void Partial_Apply_Func_5()
        {
            UnitTest(
                _ => func((int a, int b, int c, int d, int e) => a + b + c + d + e),
                f => f.Apply(10),
                partial => Assert.Equal(55, partial(11, 12, 13, 9))
            );
        }

        [Fact]
        public void Chain_Partial_Applications()
        {
            UnitTest(
                _ => func((int a, int b, int c, int d, int e) => a + b + c + d + e),
                f => f.Apply(1).Apply(2).Apply(3).Apply(4),
                partial => Assert.Equal(15, partial(5))
            );
        }
    }
}
