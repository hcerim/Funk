using System;
using System.Threading.Tasks;

namespace Funk.Tests
{
    public abstract class Test
    {
        /// <summary>
        /// Wrapper method that enforces Arrange-Act-Assert (AAA) principle.
        /// </summary>
        protected static void UnitTest<TArrangeResult, TActResult>(
            Func<Unit, TArrangeResult> arrange,
            Func<TArrangeResult, TActResult> act,
            Action<TActResult> assert
        ) => assert(act(arrange(Unit.Value)));

        /// <summary>
        /// Wrapper method that enforces Arrange-Act-Assert (AAA) principle.
        /// </summary>
        protected static async Task UnitTestAsync<TArrangeResult, TActResult>(
            Func<Unit, Task<TArrangeResult>> arrange,
            Func<TArrangeResult, Task<TActResult>> act,
            Action<TActResult> assert
        ) => assert(await act(await arrange(Unit.Value)));

        /// <summary>
        /// Wrapper method that enforces Arrange-Act-Assert (AAA) principle.
        /// </summary>
        protected static async Task UnitTestAsync<TArrangeResult, TActResult>(
            Func<Unit, Task<TArrangeResult>> arrange,
            Func<TArrangeResult, Task<TActResult>> act,
            Action<Task<TActResult>> assert
        ) => assert(act(await arrange(Unit.Value)));
    }
}
