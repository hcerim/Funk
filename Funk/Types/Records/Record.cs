using System;

namespace Funk
{
    /// <summary>
    /// Record with arity of 1.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public struct Record<T1>
    {
        /// <summary>
        /// Initializes a new record with 1 item.
        /// </summary>
        /// <param name="t1"></param>
        public Record(T1 t1)
        {
            Item1 = t1;
        }

        public T1 Item1 { get; }

        /// <summary>
        /// Maps corresponding record item to the result of the selector.
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        public R Map<R>(Func<T1, R> selector)
        {
            return selector(Item1);
        }

        /// <summary>
        /// Executes operation provided with record item.
        /// </summary>
        /// <param name="operation"></param>
        public void Do(Action<T1> operation)
        {
            operation(Item1);
        }
    }

    /// <summary>
    /// Record with arity of 2.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public struct Record<T1, T2>
    {
        /// <summary>
        /// Initializes a new record with 2 items.
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        public Record(T1 t1, T2 t2)
        {
            Item1 = t1;
            Item2 = t2;
        }

        public T1 Item1 { get; }
        public T2 Item2 { get; }

        /// <summary>
        /// Maps corresponding record items to the result of the selector.
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        public R Map<R>(Func<T1, T2, R> selector)
        {
            return selector(Item1, Item2);
        }

        /// <summary>
        /// Executes operation provided with record items.
        /// </summary>
        /// <param name="operation"></param>
        public void Do(Action<T1, T2> operation)
        {
            operation(Item1, Item2);
        }
    }

    /// <summary>
    /// Record with arity of 3.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    public struct Record<T1, T2, T3>
    {
        /// <summary>
        /// Initializes a new record with 3 items.
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        public Record(T1 t1, T2 t2, T3 t3)
        {
            Item1 = t1;
            Item2 = t2;
            Item3 = t3;
        }

        public T1 Item1 { get; }
        public T2 Item2 { get; }
        public T3 Item3 { get; }

        /// <summary>
        /// Maps corresponding record items to the result of the selector.
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        public R Map<R>(Func<T1, T2, T3, R> selector)
        {
            return selector(Item1, Item2, Item3);
        }

        /// <summary>
        /// Executes operation provided with record items.
        /// </summary>
        /// <param name="operation"></param>
        public void Do(Action<T1, T2, T3> operation)
        {
            operation(Item1, Item2, Item3);
        }
    }
}
