using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Funk.Tests
{
    public class TaskTests : Test
    {
        [Fact]
        public async Task WithResult_On_Task_Produces_Unit()
        {
            await UnitTestAsync(
                _ => result(Task.CompletedTask),
                t => t.WithResult(),
                u => Assert.Equal(Unit.Value, u)
            );
        }

        [Fact]
        public async Task WithResult_On_Task_Produces_Specified_Result()
        {
            await UnitTestAsync(
                _ => result(Task.CompletedTask),
                t => t.WithResult(_ => 42),
                r => Assert.Equal(42, r)
            );
        }

        [Fact]
        public async Task ToTask_On_Value_Wraps_In_Completed_Task()
        {
            await UnitTestAsync(
                _ => result("Funk"),
                s => s.ToTask(),
                s => Assert.Equal("Funk", s)
            );
        }

        [Fact]
        public async Task ToTask_On_Action_Queues_Work()
        {
            await UnitTestAsync(
                _ =>
                {
                    var flag = false;
                    Action action = () => flag = true;
                    return result((action, func(() => flag)));
                },
                async arrange =>
                {
                    await arrange.action.ToTask();
                    return arrange.Item2();
                },
                Assert.True
            );
        }

        [Fact]
        public async Task ToTask_On_Action_With_CancellationToken()
        {
            await UnitTestAsync(
                _ =>
                {
                    var flag = false;
                    Action action = () => flag = true;
                    return result((action, func(() => flag)));
                },
                async arrange =>
                {
                    using var cts = new CancellationTokenSource();
                    await arrange.action.ToTask(cts.Token);
                    return arrange.Item2();
                },
                Assert.True
            );
        }

        [Fact]
        public async Task ToTask_On_Func_Queues_And_Returns_Result()
        {
            await UnitTestAsync(
                _ => result(func(() => 42)),
                f => f.ToTask(),
                r => Assert.Equal(42, r)
            );
        }

        [Fact]
        public async Task ToTask_On_FuncTask_Queues_Async_Work()
        {
            await UnitTestAsync(
                _ =>
                {
                    Func<Task> asyncAction = async () => await Task.Delay(1);
                    return result(asyncAction);
                },
                async f =>
                {
                    await f.ToTask();
                    return true;
                },
                Assert.True
            );
        }

        [Fact]
        public async Task ToTask_On_FuncTaskT_Queues_Async_Work_And_Returns_Result()
        {
            await UnitTestAsync(
                _ => result(func(async () =>
                {
                    await Task.Delay(1);
                    return "Funk";
                })),
                f => f.ToTask(),
                r => Assert.Equal("Funk", r)
            );
        }
    }
}
