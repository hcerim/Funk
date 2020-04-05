using System;
using System.Net.Http;
using Xunit;
using static Funk.Prelude;

namespace Funk.Tests.ExtensionsTests
{
    public class DisposableTests : Test
    {
        [Fact]
        public void Dispose()
        {
            UnitTest(
                _ => new HttpClient(),
                c =>
                {
                    c.DisposeAfter(cl => cl.CancelPendingRequests());
                    return c;
                },
                c =>
                {
                    Assert.Throws<ObjectDisposedException>(act(c.CancelPendingRequests));
                }
            );
        }
    }
}
