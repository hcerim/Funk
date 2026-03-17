using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Funk.Tests
{
    public class ExceptionExtTests : Test
    {
        [Fact]
        public void ToEnumerableException_From_Single()
        {
            UnitTest(
                _ => new FunkException("Funk"),
                e => e.ToEnumerableException(),
                e =>
                {
                    Assert.Single(e);
                    Assert.True(e.Root.NotEmpty);
                }
            );
        }

        [Fact]
        public void ToEnumerableException_From_Single_With_Message()
        {
            UnitTest(
                _ => new FunkException("Funk"),
                e => e.ToEnumerableException("Custom message"),
                e =>
                {
                    Assert.Single(e);
                    Assert.Equal("Custom message", e.Message);
                }
            );
        }

        [Fact]
        public void ToEnumerableException_From_Sequence()
        {
            UnitTest(
                _ => new List<FunkException>
                {
                    new FunkException("First"),
                    new FunkException("Second"),
                    new FunkException("Third")
                },
                e => e.ToEnumerableException(),
                e =>
                {
                    Assert.Equal(3, e.Count());
                    Assert.Equal("First", e.Root.UnsafeGet().Message);
                }
            );
        }

        [Fact]
        public void ToEnumerableException_From_Sequence_With_Message()
        {
            UnitTest(
                _ => new List<FunkException>
                {
                    new FunkException("First"),
                    new FunkException("Second")
                },
                e => e.ToEnumerableException("Aggregated errors"),
                e =>
                {
                    Assert.Equal(2, e.Count());
                    Assert.Equal("Aggregated errors", e.Message);
                }
            );
        }

        [Fact]
        public void Merge_Two_Exceptions()
        {
            UnitTest(
                _ => (First: new FunkException("First"), Second: new FunkException("Second")),
                r => r.First.Merge(r.Second),
                e =>
                {
                    Assert.Equal(2, e.Count());
                    Assert.Equal("First", e.Root.UnsafeGet().Message);
                    Assert.Equal("Second", e.ElementAt(1).Message);
                }
            );
        }

        [Fact]
        public void MergeRange_Exception_With_Sequence()
        {
            UnitTest(
                _ => (
                    Root: new FunkException("Root"),
                    Others: new List<FunkException>
                    {
                        new FunkException("A"),
                        new FunkException("B"),
                        new FunkException("C")
                    }
                ),
                r => r.Root.MergeRange(r.Others),
                e =>
                {
                    Assert.Equal(4, e.Count());
                    Assert.Equal("Root", e.Root.UnsafeGet().Message);
                    Assert.Equal("C", e.ElementAt(3).Message);
                }
            );
        }
    }
}
