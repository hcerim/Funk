using System.Diagnostics.Contracts;

namespace Funk
{
    public struct Record
    {
        /// <summary>
        /// Creates a record of 1.
        /// </summary>
        [Pure]
        public static Record<T1> Create<T1>(T1 t1) => new Record<T1>(t1);

        /// <summary>
        /// Creates a record of 2.
        /// </summary>
        [Pure]
        public static Record<T1, T2> Create<T1, T2>(T1 t1, T2 t2) => new Record<T1, T2>(t1, t2);

        /// <summary>
        /// Creates a record of 2.
        /// </summary>
        [Pure]
        public static Record<T1, T2> Create<T1, T2>((T1 t1, T2 t2) tuple) => new Record<T1, T2>(tuple);

        /// <summary>
        /// Creates a record of 3.
        /// </summary>
        [Pure]
        public static Record<T1, T2, T3> Create<T1, T2, T3>(T1 t1, T2 t2, T3 t3) => new Record<T1, T2, T3>(t1, t2, t3);

        /// <summary>
        /// Creates a record of 3.
        /// </summary>
        [Pure]
        public static Record<T1, T2, T3> Create<T1, T2, T3>((T1 t1, T2 t2, T3 t3) tuple) => new Record<T1, T2, T3>(tuple);

        /// <summary>
        /// Creates a record of 4.
        /// </summary>
        [Pure]
        public static Record<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4) => new Record<T1, T2, T3, T4>(t1, t2, t3, t4);

        /// <summary>
        /// Creates a record of 4.
        /// </summary>
        [Pure]
        public static Record<T1, T2, T3, T4> Create<T1, T2, T3, T4>((T1 t1, T2 t2, T3 t3, T4 t4) tuple) => new Record<T1, T2, T3, T4>(tuple);

        /// <summary>
        /// Creates a record of 5.
        /// </summary>
        [Pure]
        public static Record<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) => new Record<T1, T2, T3, T4, T5>(t1, t2, t3, t4, t5);

        /// <summary>
        /// Creates a record of 5.
        /// </summary>
        [Pure]
        public static Record<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) tuple) => new Record<T1, T2, T3, T4, T5>(tuple);
    }
}
