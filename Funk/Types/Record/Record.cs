using System;

namespace Funk
{
    /// <summary>
    /// Record with arity of 1.
    /// </summary>
    public struct Record<T1>
    {
        public Record(T1 t1)
        {
            Item1 = t1;
        }

        public T1 Item1 { get; }

        /// <summary>
        /// Maps corresponding record item to the result of the selector.
        /// </summary>
        public R Match<R>(Func<T1, R> selector) => selector(Item1);

        /// <summary>
        /// Structure-preserving map. Maps corresponding record item to the new Record of 1.
        /// </summary>
        public Record<R1> Map<R1>(Func<T1, Record<R1>> selector) => selector(Item1);

        /// <summary>
        /// Structure-preserving map. Maps corresponding record item to the new Record of 1.
        /// </summary>
        public Record<R1> Map<R1>(Func<T1, R1> selector) => Record.Create(selector(Item1));

        /// <summary>
        /// Executes operation provided with record item.
        /// </summary>
        public void Match(Action<T1> operation) => operation(Item1);
    }

    /// <summary>
    /// Record with arity of 2.
    /// </summary>
    public struct Record<T1, T2>
    {
        public Record(T1 t1, T2 t2)
        {
            Item1 = t1;
            Item2 = t2;
        }

        public Record((T1 t1, T2 t2) tuple)
        {
            Item1 = tuple.t1;
            Item2 = tuple.t2;
        }

        public T1 Item1 { get; }
        public T2 Item2 { get; }

        /// <summary>
        /// Maps corresponding record items to the result of the selector.
        /// </summary>
        public R Match<R>(Func<T1, T2, R> selector) => selector(Item1, Item2);

        /// <summary>
        /// Structure-preserving map. Maps corresponding record items to the new Record of 2.
        /// </summary>
        public Record<R1, R2> Map<R1, R2>(Func<T1, T2, Record<R1, R2>> selector) => selector(Item1, Item2);

        /// <summary>
        /// Structure-preserving map. Maps corresponding record items to the new Record of 2.
        /// </summary>
        public Record<R1, R2> Map<R1, R2>(Func<T1, T2, (R1, R2)> selector) => Record.Create(selector(Item1, Item2));

        /// <summary>
        /// Executes operation provided with record items.
        /// </summary>
        public void Match(Action<T1, T2> operation) => operation(Item1, Item2);
    }

    /// <summary>
    /// Record with arity of 3.
    /// </summary>
    public struct Record<T1, T2, T3>
    {
        public Record(T1 t1, T2 t2, T3 t3)
        {
            Item1 = t1;
            Item2 = t2;
            Item3 = t3;
        }

        public Record((T1 t1, T2 t2, T3 t3) tuple)
        {
            Item1 = tuple.t1;
            Item2 = tuple.t2;
            Item3 = tuple.t3;
        }

        public T1 Item1 { get; }
        public T2 Item2 { get; }
        public T3 Item3 { get; }

        /// <summary>
        /// Maps corresponding record items to the result of the selector.
        /// </summary>
        public R Match<R>(Func<T1, T2, T3, R> selector) => selector(Item1, Item2, Item3);

        /// <summary>
        /// Structure-preserving map. Maps corresponding record items to the new Record of 3.
        /// </summary>
        public Record<R1, R2, R3> Map<R1, R2, R3>(Func<T1, T2, T3, Record<R1, R2, R3>> selector) => selector(Item1, Item2, Item3);

        /// <summary>
        /// Structure-preserving map. Maps corresponding record items to the new Record of 3.
        /// </summary>
        public Record<R1, R2, R3> Map<R1, R2, R3>(Func<T1, T2, T3, (R1, R2, R3)> selector) => Record.Create(selector(Item1, Item2, Item3));

        /// <summary>
        /// Executes operation provided with record items.
        /// </summary>
        public void Match(Action<T1, T2, T3> operation) => operation(Item1, Item2, Item3);
    }

    /// <summary>
    /// Record with arity of 4.
    /// </summary>
    public struct Record<T1, T2, T3, T4>
    {
        public Record(T1 t1, T2 t2, T3 t3, T4 t4)
        {
            Item1 = t1;
            Item2 = t2;
            Item3 = t3;
            Item4 = t4;
        }

        public Record((T1 t1, T2 t2, T3 t3, T4 t4) tuple)
        {
            Item1 = tuple.t1;
            Item2 = tuple.t2;
            Item3 = tuple.t3;
            Item4 = tuple.t4;
        }

        public T1 Item1 { get; }
        public T2 Item2 { get; }
        public T3 Item3 { get; }
        public T4 Item4 { get; }

        /// <summary>
        /// Maps corresponding record items to the result of the selector.
        /// </summary>
        public R Match<R>(Func<T1, T2, T3, T4, R> selector) => selector(Item1, Item2, Item3, Item4);

        /// <summary>
        /// Structure-preserving map. Maps corresponding record items to the new Record of 4.
        /// </summary>
        public Record<R1, R2, R3, R4> Map<R1, R2, R3, R4>(Func<T1, T2, T3, T4, Record<R1, R2, R3, R4>> selector) => selector(Item1, Item2, Item3, Item4);

        /// <summary>
        /// Structure-preserving map. Maps corresponding record items to the new Record of 4.
        /// </summary>
        public Record<R1, R2, R3, R4> Map<R1, R2, R3, R4>(Func<T1, T2, T3, T4, (R1, R2, R3, R4)> selector) => Record.Create(selector(Item1, Item2, Item3, Item4));

        /// <summary>
        /// Executes operation provided with record items.
        /// </summary>
        public void Match(Action<T1, T2, T3, T4> operation) => operation(Item1, Item2, Item3, Item4);
    }

    /// <summary>
    /// Record with arity of 5.
    /// </summary>
    public struct Record<T1, T2, T3, T4, T5>
    {
        public Record(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            Item1 = t1;
            Item2 = t2;
            Item3 = t3;
            Item4 = t4;
            Item5 = t5;
        }

        public Record((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) tuple)
        {
            Item1 = tuple.t1;
            Item2 = tuple.t2;
            Item3 = tuple.t3;
            Item4 = tuple.t4;
            Item5 = tuple.t5;
        }

        public T1 Item1 { get; }
        public T2 Item2 { get; }
        public T3 Item3 { get; }
        public T4 Item4 { get; }
        public T5 Item5 { get; }

        /// <summary>
        /// Maps corresponding record items to the result of the selector.
        /// </summary>
        public R Match<R>(Func<T1, T2, T3, T4, T5, R> selector) => selector(Item1, Item2, Item3, Item4, Item5);

        /// <summary>
        /// Structure-preserving map. Maps corresponding record items to the new Record of 5.
        /// </summary>
        public Record<R1, R2, R3, R4, R5> Map<R1, R2, R3, R4, R5>(Func<T1, T2, T3, T4, T5, Record<R1, R2, R3, R4, R5>> selector) => selector(Item1, Item2, Item3, Item4, Item5);

        /// <summary>
        /// Structure-preserving map. Maps corresponding record items to the new Record of 5.
        /// </summary>
        public Record<R1, R2, R3, R4, R5> Map<R1, R2, R3, R4, R5>(Func<T1, T2, T3, T4, T5, (R1, R2, R3, R4, R5)> selector) => Record.Create(selector(Item1, Item2, Item3, Item4, Item5));

        /// <summary>
        /// Executes operation provided with record items.
        /// </summary>
        public void Match(Action<T1, T2, T3, T4, T5> operation) => operation(Item1, Item2, Item3, Item4, Item5);
    }
}
