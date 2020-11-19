using Xunit;
using static Funk.Prelude;

namespace Funk.Tests
{
    public class CompositionTests : Test
    {
        [Fact]
        public void ComposeTwoFunctions()
        {
            UnitTest(
                _ => func((int i) => i * 3),
                f => f.Compose((int i) => i * 2),
                f => Assert.Equal(6, f.Apply(1))
            );
        }
        
        [Fact]
        public void ComposeTwoFunctionsAssertExecutionOrder()
        {
            UnitTest(
                _ => func((int i) => i - 1),
                f => f.Compose((int i) => i * 2),
                f => Assert.Equal(1, f.Apply(1))
            );
        }
        
        [Fact]
        public void ComposeInvertTwoFunctionsAssertExecutionOrder()
        {
            UnitTest(
                _ => func((int i) => i - 1),
                f => f.ComposeInvert(i => i * 2),
                f => Assert.Equal(0, f.Apply(1))
            );
        }
        
        [Fact]
        public void ComposeCombineThreeFunctions()
        {
            UnitTest(
                _ => func((int i) => i * 3),
                f => f.Compose((int i) => i * 2).ComposeInvert(i => $"{i}"),
                f => Assert.Equal("6", f.Apply(1))
            );
        }
        
        [Fact]
        public void ComposeCombineChainFunctions()
        {
            UnitTest(
                _ => func((int i) => i * 3),
                f => f
                    .Compose((int i) => i * 2)
                    .ComposeInvert(i => i - 3)
                    .Compose((int i) => i * 2)
                    .ComposeInvert(i => i / 3)
                    .Compose((int i) => i / 2)
                    .Compose((int i) => i / 2)
                    .ComposeInvert(i => i * 2),
                f =>
                {
                    Assert.Equal(14, f.Apply(8));
                }
            );
        }
    }
}