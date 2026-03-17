using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Funk.Tests
{
    public partial class PreludeTests
    {
        [Fact]
        public async Task Result_Creates_Completed_Task_With_Value()
        {
            UnitTest(
                _ => result(42),
                t => t.Result,
                r => Assert.Equal(42, r)
            );
        }

        [Fact]
        public async Task Run_Action_Executes_Work()
        {
            var executed = false;
            await run(() => executed = true);
            Assert.True(executed);
        }

        [Fact]
        public async Task Run_Action_With_NonCancelled_Token_Executes_Work()
        {
            var executed = false;
            using var cts = new CancellationTokenSource();
            await run(() => executed = true, cts.Token);
            Assert.True(executed);
        }

        [Fact]
        public async Task Run_Func_Returns_Result()
        {
            var value = await run(() => 42);
            Assert.Equal(42, value);
        }

        [Fact]
        public async Task Run_Func_With_NonCancelled_Token_Returns_Result()
        {
            using var cts = new CancellationTokenSource();
            var value = await run(() => "Funk", cts.Token);
            Assert.Equal("Funk", value);
        }

        [Fact]
        public async Task Run_Async_Action_Executes_Work()
        {
            var executed = false;
            await run(async () =>
            {
                await Task.Delay(1);
                executed = true;
            });
            Assert.True(executed);
        }

        [Fact]
        public async Task Run_Async_Action_With_NonCancelled_Token_Executes_Work()
        {
            var executed = false;
            using var cts = new CancellationTokenSource();
            await run(async () =>
            {
                await Task.Delay(1);
                executed = true;
            }, cts.Token);
            Assert.True(executed);
        }

        [Fact]
        public async Task Run_Async_Func_Returns_Result()
        {
            var value = await run(async () =>
            {
                await Task.Delay(1);
                return 42;
            });
            Assert.Equal(42, value);
        }

        [Fact]
        public async Task Run_Async_Func_With_NonCancelled_Token_Returns_Result()
        {
            using var cts = new CancellationTokenSource();
            var value = await run(async () =>
            {
                await Task.Delay(1);
                return "Funk";
            }, cts.Token);
            Assert.Equal("Funk", value);
        }
    }
}
