using System;

namespace Funk
{
    /// <summary>
    /// Record with arity of 1.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public class Record<T1>
    {
        /// <summary>
        /// Initializes a new record with 1 item.
        /// </summary>
        /// <param name="t1"></param>
        public Record(T1 t1)
        {
            FirstItem = t1;
        }

        public T1 FirstItem { get; }

        /// <summary>
        /// Maps corresponding record item to the result of the selector.
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        public R Map<R>(Func<T1, R> selector)
        {
            return selector(FirstItem);
        }

        /// <summary>
        /// Executes operation provided with record item.
        /// </summary>
        /// <param name="operation"></param>
        public void Do(Action<T1> operation)
        {
            operation(FirstItem);
        }
    }

    /// <summary>
    /// Record with arity of 2.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class Record<T1, T2>
    {
        /// <summary>
        /// Initializes a new record with 2 items.
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        public Record(T1 t1, T2 t2)
        {
            FirstItem = t1;
            SecondItem = t2;
        }

        public T1 FirstItem { get; }
        public T2 SecondItem { get; }

        /// <summary>
        /// Maps corresponding record items to the result of the selector.
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        public R Map<R>(Func<T1, T2, R> selector)
        {
            return selector(FirstItem, SecondItem);
        }

        /// <summary>
        /// Executes operation provided with record items.
        /// </summary>
        /// <param name="operation"></param>
        public void Do(Action<T1, T2> operation)
        {
            operation(FirstItem, SecondItem);
        }
    }

    /// <summary>
    /// Record with arity of 3.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    public class Record<T1, T2, T3>
    {
        /// <summary>
        /// Initializes a new record with 3 items.
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        public Record(T1 t1, T2 t2, T3 t3)
        {
            FirstItem = t1;
            SecondItem = t2;
            ThirdItem = t3;
        }

        public T1 FirstItem { get; }
        public T2 SecondItem { get; }
        public T3 ThirdItem { get; }

        /// <summary>
        /// Maps corresponding record items to the result of the selector.
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        public R Map<R>(Func<T1, T2, T3, R> selector)
        {
            return selector(FirstItem, SecondItem, ThirdItem);
        }

        /// <summary>
        /// Executes operation provided with record items.
        /// </summary>
        /// <param name="operation"></param>
        public void Do(Action<T1, T2, T3> operation)
        {
            operation(FirstItem, SecondItem, ThirdItem);
        }
    }
}
