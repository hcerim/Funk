using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

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

        [Fact]
        public void DisposeAfter_Func_Returns_Result()
        {
            UnitTest(
                _ => new MemoryStream(new byte[] { 1, 2, 3 }),
                s => s.DisposeAfter(ms => (int)ms.Length),
                r => Assert.Equal(3, r)
            );
        }

        [Fact]
        public void DisposeAfter_Action_Disposes()
        {
            UnitTest(
                _ => new MemoryStream(),
                s =>
                {
                    s.DisposeAfter(ms => ms.WriteByte(1));
                    return s;
                },
                s => Assert.Throws<ObjectDisposedException>(() => s.WriteByte(1))
            );
        }

        [Fact]
        public async Task DisposeAfterAsync_Action_Disposes()
        {
            await UnitTestAsync(
                _ => result(new MemoryStream()),
                async s =>
                {
                    await s.DisposeAfterAsync(ms => ms.WriteAsync(new byte[] { 1 }, 0, 1));
                    return s;
                },
                s => Assert.Throws<ObjectDisposedException>(() => s.WriteByte(1))
            );
        }

        [Fact]
        public async Task DisposeAfterAsync_Func_Returns_Result_And_Disposes()
        {
            await UnitTestAsync(
                _ => result(new MemoryStream(new byte[] { 1, 2, 3, 4, 5 })),
                async s =>
                {
                    var length = await s.DisposeAfterAsync(async ms =>
                    {
                        await Task.CompletedTask;
                        return (int)ms.Length;
                    });
                    return (s, length);
                },
                r =>
                {
                    Assert.Equal(5, r.length);
                    Assert.Throws<ObjectDisposedException>(() => r.s.WriteByte(1));
                }
            );
        }

        [Fact]
        public async Task AsyncDisposeAfter_Sync_Op_Disposes()
        {
            await UnitTestAsync(
                _ => result(new MemoryStream(new byte[] { 1, 2, 3 })),
                async s =>
                {
                    var length = await s.AsyncDisposeAfter(ms => (int)ms.Length);
                    return (s, length);
                },
                r =>
                {
                    Assert.Equal(3, r.length);
                    Assert.Throws<ObjectDisposedException>(() => r.s.WriteByte(1));
                }
            );
        }

        [Fact]
        public async Task AsyncDisposeAfterAsync_Disposes()
        {
            await UnitTestAsync(
                _ => result(new MemoryStream(new byte[] { 10, 20, 30 })),
                async s =>
                {
                    var length = await s.AsyncDisposeAfterAsync(async ms =>
                    {
                        await Task.CompletedTask;
                        return (int)ms.Length;
                    });
                    return (s, length);
                },
                r =>
                {
                    Assert.Equal(3, r.length);
                    Assert.Throws<ObjectDisposedException>(() => r.s.WriteByte(1));
                }
            );
        }

        [Fact]
        public async Task AsyncDisposeAfter_Action_Disposes()
        {
            await UnitTestAsync(
                _ => result(new MemoryStream()),
                async s =>
                {
                    await s.AsyncDisposeAfter(ms => ms.WriteByte(1));
                    return s;
                },
                s => Assert.Throws<ObjectDisposedException>(() => s.WriteByte(1))
            );
        }

        [Fact]
        public async Task AsyncDisposeAfterAsync_Action_Disposes()
        {
            await UnitTestAsync(
                _ => result(new MemoryStream()),
                async s =>
                {
                    await s.AsyncDisposeAfterAsync(ms => ms.WriteAsync(new byte[] { 1 }, 0, 1));
                    return s;
                },
                s => Assert.Throws<ObjectDisposedException>(() => s.WriteByte(1))
            );
        }
    }
}
