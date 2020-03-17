using System;

namespace Funk.Tests
{
    public abstract class Test
    {
        /// <summary>
        /// Wrapper method that enforces Arrange-Act-Assert (AAA) principle.
        /// </summary>
        /// <typeparam name="TArrangeResult"></typeparam>
        /// <typeparam name="TActResult"></typeparam>
        /// <param name="arrange"></param>
        /// <param name="act"></param>
        /// <param name="assert"></param>
        protected static void UnitTest<TArrangeResult, TActResult>(
            Func<Unit, TArrangeResult> arrange,
            Func<TArrangeResult, TActResult> act,
            Action<TActResult> assert)
        {
            var arrangeResult = arrange(Unit.Value);
            var actResult = act(arrangeResult);
            assert(actResult);
        }
    }
}
