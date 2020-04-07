using System;

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
            Action<TActResult> assert)
        {
            var arrangeResult = arrange(Unit.Value);
            var actResult = act(arrangeResult);
            assert(actResult);
        }
    }
}
