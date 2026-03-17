using System;
using System.Collections.Generic;
using Xunit;

namespace Funk.Tests
{
    public partial class PreludeTests
    {
        [Fact]
        public void Exception_From_Single_Creates_EnumerableException()
        {
            UnitTest(
                _ => new InvalidOperationException("Funk"),
                e => exception(e),
                e =>
                {
                    Assert.Single(e);
                    Assert.IsType<InvalidOperationException>(e.Root.UnsafeGet());
                    Assert.Equal("Funk", e.Root.UnsafeGet().Message);
                }
            );
        }

        [Fact]
        public void Exception_From_Single_With_Message_Creates_EnumerableException()
        {
            UnitTest(
                _ => new InvalidOperationException("inner"),
                e => exception(e, "outer message"),
                e =>
                {
                    Assert.Single(e);
                    Assert.Equal("outer message", e.Message);
                    Assert.IsType<InvalidOperationException>(e.Root.UnsafeGet());
                }
            );
        }

        [Fact]
        public void Exception_From_Sequence_Creates_EnumerableException()
        {
            UnitTest(
                _ => new List<ArgumentException>
                {
                    new ArgumentException("first"),
                    new ArgumentException("second"),
                    new ArgumentException("third")
                },
                e => exception<ArgumentException>(e),
                e =>
                {
                    Assert.Equal(3, e.Count);
                    Assert.Equal("first", e.Root.UnsafeGet().Message);
                    Assert.Equal(3, e.Nested.UnsafeGet().Count);
                }
            );
        }

        [Fact]
        public void Exception_From_Sequence_With_Message_Creates_EnumerableException()
        {
            UnitTest(
                _ => new List<ArgumentException>
                {
                    new ArgumentException("first"),
                    new ArgumentException("second")
                },
                e => exception<ArgumentException>(e, "batch error"),
                e =>
                {
                    Assert.Equal(2, e.Count);
                    Assert.Equal("batch error", e.Message);
                    Assert.Equal("first", e.Root.UnsafeGet().Message);
                }
            );
        }
    }
}
