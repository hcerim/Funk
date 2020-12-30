using FsCheck.Xunit;
using Xunit;
using Xunit.Abstractions;
using static Funk.Prelude;

namespace Funk.Tests
{
    public class CompositionTests : Test
    {
        private readonly ITestOutputHelper _output;

        public CompositionTests(ITestOutputHelper output)
        {
            _output = output;
        }

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
        
        [Fact]
        public void ComposeCombineChainFunctionsWithActions1()
        {
            UnitTest(
                _ => act((int i) =>
                {
                    _output.WriteLine("Initialize logging:\n");
                    _output.WriteLine($"Initial item - {i.ToString()}\n");
                }),
                f => f
                    .ComposeLeft(i => i * 3)
                    .ComposeLeft((_, j) =>
                    {
                        _output.WriteLine("Middle step:\n");
                        _output.WriteLine($"Operation result - {j.ToString()}\n");
                    })
                    .ComposeLeft(i => i * 3)
                    .ComposeLeft((_, j) =>
                    {
                        _output.WriteLine("Finalize logging:\n");
                        _output.WriteLine($"Final item - {j.ToString()}\n");
                    }),
                f => Assert.Equal(27, f.Apply(3))
            );
        }
        
        [Fact]
        public void ComposeCombineChainFunctionsWithActions2()
        {
            UnitTest(
                _ => func((int i) => i * 3),
                f => f
                    .ComposeLeft(i => i * 3)
                    .ComposeRight(i =>
                    {
                        _output.WriteLine("Initialize logging\n");
                        _output.WriteLine($"Initial item - {i.ToString()}\n");
                    })
                    .ComposeLeft((_, j) =>
                    {
                        _output.WriteLine("Finalize logging\n");
                        _output.WriteLine($"Final item - {j.ToString()}\n");
                    }),
                f => Assert.Equal(27, f.Apply(3))
            );
        }
        
        [Fact]
        public void ComposeCombineChainFunctionsWithActions3()
        {
            UnitTest(
                _ => func((int i) => i * 3),
                f => 
                    act((int i, int j) =>
                    {
                        _output.WriteLine("First step:\n");
                        _output.WriteLine($"Initial item - {i.ToString()}\n");
                        _output.WriteLine($"Operation result - {j.ToString()}\n");
                    })
                    .ComposeRight(f)
                    .ComposeLeft(i => i * 3)
                    .ComposeRight(i =>
                    {
                        _output.WriteLine("Initialize logging:\n");
                        _output.WriteLine($"Initial item - {i.ToString()}\n");
                    })
                    .ComposeLeft((i, j) =>
                    {
                        _output.WriteLine("Second step:\n");
                        _output.WriteLine($"Initial item - {i.ToString()}\n");
                        _output.WriteLine($"Operation result - {j.ToString()}\n");
                    })
                    .ComposeLeft(i => i * 3)
                    .ComposeLeft((i, j) =>
                    {
                        _output.WriteLine("Finalize logging:\n");
                        _output.WriteLine($"Initial item - {i.ToString()}\n");
                        _output.WriteLine($"Final item - {j.ToString()}\n");
                    }),
                f => Assert.Equal(81, f.Apply(3))
            );
        }
        
        [Property]
        public void LeftAssociativity(int num)
        {
            UnitTest(
                _ =>
                {
                    var triple = func((int i) => i * 3);
                    var @double = func((int i) => i * 2);
                    var subtractThree = func((int i) => i - 3);
                    return (triple, @double, subtractThree);
                },
                f =>
                {
                    var composition = f.@double.ComposeLeft(f.triple).ComposeLeft(f.subtractThree);
                    var chaining = func((int i) => f.subtractThree(f.triple(f.@double(i))));
                    return (composition, chaining);
                },
                r =>
                {
                    Assert.True(r.composition.Apply(num).SafeEquals(r.chaining.Apply(num)));
                }
            );
        }
        
        [Property]
        public void RightAssociativity(int num)
        {
            UnitTest(
                _ =>
                {
                    var triple = func((int i) => i * 3);
                    var @double = func((int i) => i * 2);
                    var subtractThree = func((int i) => i - 3);
                    return (triple, @double, subtractThree);
                },
                f =>
                {
                    var composition = f.@double.ComposeRight(f.triple).ComposeRight(f.subtractThree);
                    var chaining = func((int i) => f.@double(f.triple(f.subtractThree(i))));
                    return (composition, chaining);
                },
                r =>
                {
                    Assert.True(r.composition.Apply(num).SafeEquals(r.chaining.Apply(num)));
                }
            );
        }
    }
}