using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using static Funk.Prelude;

namespace Funk.Tests
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
                    c.DisposeAfter(cl => cl.GetAsync(new Uri("https://www.google.com")));
                    return c;
                },
                c => Assert.Throws<ObjectDisposedException>(act(c.CancelPendingRequests))
            );
        }

        [Fact]
        public async Task Dispose_Async()
        {
            await UnitTestAsync(
                _ => result(new HttpClient()),
                async c =>
                {
                    await c.DisposeAfterAsync(cl => cl.GetAsync(new Uri("https://www.google.com")));
                    return c;
                },
                c => Assert.Throws<ObjectDisposedException>(act(c.CancelPendingRequests))
            );
        }
    }
}
