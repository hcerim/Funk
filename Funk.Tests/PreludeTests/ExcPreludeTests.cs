using System;
using Xunit;

namespace Funk.Tests
{
    public partial class PreludeTests
    {
        [Fact]
        public void Success_Creates_Successful_Exc()
        {
            UnitTest(
                _ => success<string, Exception>("Funk"),
                e => e,
                e =>
                {
                    Assert.True(e.IsSuccess);
                    Assert.Equal("Funk", e.UnsafeGetSuccess());
                }
            );
        }

        [Fact]
        public void Failure_With_Exception_Creates_Failed_Exc()
        {
            UnitTest(
                _ => failure<string, Exception>(new Exception("error")),
                e => e,
                e =>
                {
                    Assert.True(e.IsFailure);
                    Assert.Equal("error", e.RootFailure.UnsafeGet().Message);
                }
            );
        }

        [Fact]
        public void Failure_With_EnumerableException_Creates_Failed_Exc()
        {
            UnitTest(
                _ =>
                {
                    var enumerable = exception(new Exception("inner"));
                    return failure<string, Exception>(enumerable);
                },
                e => e,
                e =>
                {
                    Assert.True(e.IsFailure);
                    Assert.Equal("inner", e.RootFailure.UnsafeGet().Message);
                }
            );
        }
    }
}
