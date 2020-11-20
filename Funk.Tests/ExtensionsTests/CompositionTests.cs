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
                f => f.ComposeRight((int i) => i * 2),
                f => Assert.Equal(6, f.Apply(1))
            );
        }
        
        [Fact]
        public void ComposeTwoFunctionsAssertExecutionOrder()
        {
            UnitTest(
                _ => func((int i) => i - 1),
                f => f.ComposeRight((int i) => i * 2),
                f => Assert.Equal(1, f.Apply(1))
            );
        }
        
        [Fact]
        public void ComposeInvertTwoFunctionsAssertExecutionOrder()
        {
            UnitTest(
                _ => func((int i) => i - 1),
                f => f.ComposeLeft(i => i * 2),
                f => Assert.Equal(0, f.Apply(1))
            );
        }
        
        [Fact]
        public void ComposeCombineThreeFunctions()
        {
            UnitTest(
                _ => func((int i) => i * 3),
                f => f.ComposeRight((int i) => i * 2).ComposeLeft(i => $"{i}"),
                f => Assert.Equal("6", f.Apply(1))
            );
        }
        
        [Fact]
        public void ComposeCombineChainFunctions()
        {
            UnitTest(
                _ => func((int i) => i * 3),
                f => f
                    .ComposeRight((int i) => i * 2)
                    .ComposeLeft(i => i - 3)
                    .ComposeRight((int i) => i * 2)
                    .ComposeLeft(i => i / 3)
                    .ComposeRight((int i) => i / 2)
                    .ComposeRight((int i) => i / 2)
                    .ComposeLeft(i => i * 2),
                // execution goes from the latest ComposeRight function through the initiator function to the last ComposeLeft function.
                f =>
                {
                    Assert.Equal(14, f.Apply(8));
                }
            );
        }
    }
}