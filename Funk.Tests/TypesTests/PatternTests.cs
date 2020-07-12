using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Funk.Tests
{
    public class PatternTests : Test
    {
        [Fact]
        public void PatternCheck()
        {
            UnitTest(
                _ => "Funk",
                v =>
                {
                    return new Pattern<int>
                    {
                        ("Harun", _ => 1),
                        ("Cerim", _ => 2),
                        ("Bosnia", _ => 3),
                        ("Funk", _ => 4),
                        (new {Name = "Harun"}, _ => 5)
                    }.Match(v);
                },
                r => Assert.Equal(4, r)
            );
        }

        [Fact]
        public async Task PatternAsyncCheck()
        {
            await UnitTestAsync(
                _ => "Funk".ToTask(),
                v =>
                {
                    return new AsyncPattern<int>
                    {
                        ("Harun", _ => 1.ToTask()),
                        ("Cerim", _ => 2.ToTask()),
                        ("Bosnia", _ => 3.ToTask()),
                        ("Funky", _ => 4.ToTask()),
                        ("Funy", _ => 4.ToTask()),
                        ("F", _ => 4.ToTask()),
                        ("Funky", _ => 3.ToTask()),
                        ("Fu", _ => 00.ToTask()),
                        ("Funky", _ => 4.ToTask()),
                        ("ky", _ => 4.ToTask()),
                        (1, vv => vv.ToTask()),
                        ("Fy", _ => 7.ToTask())
                    }.Match(v);
                },
                r => Assert.True(r.IsEmpty)
            );
        }

        [Fact]
        public void TypePatternCheck()
        {
            UnitTest(
                _ => "Harun",
                v =>
                {
                    return new TypePattern<string>
                    {
                        (string s) => s.Split('r').First(),
                        (int s) => ""
                    }.Match(v);
                },
                r => Assert.Equal("Ha", r
            ));
        }

        [Fact]
        public async Task AsyncTypePatternCheck()
        {
            await UnitTestAsync(
                _ => "Harun".ToTask(),
                v =>
                {
                    return new AsyncTypePattern<string>
                    {
                        (string s) => s.Split('r').First().ToTask(),
                        (double s) => "It is double".ToTask(),
                        async (Task<string> s) => await s,
                        (int s) => "".ToTask()
                    }.Match(v);
                },
                r => Assert.Equal("Ha", r)
            );
        }
    }
}
