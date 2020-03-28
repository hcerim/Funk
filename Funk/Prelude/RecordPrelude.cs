using System.Diagnostics.Contracts;

namespace Funk
{
    public static partial class Prelude
    {
        /// <summary>
        /// Creates a record of 1.
        /// </summary>
        [Pure]
        public static Record<T1> rec<T1>(T1 t1) => t1.ToRecord();

        /// <summary>
        /// Creates a record of 2.
        /// </summary>
        [Pure]
        public static Record<T1, T2> rec<T1, T2>(T1 t1, T2 t2) => (t1, t2).ToRecord();

        /// <summary>
        /// Creates a record of 2.
        /// </summary>
        [Pure]
        public static Record<T1, T2> rec<T1, T2>((T1 t1, T2 t2) tuple) => tuple.ToRecord();

        /// <summary>
        /// Creates a record of 3.
        /// </summary>
        [Pure]
        public static Record<T1, T2, T3> rec<T1, T2, T3>(T1 t1, T2 t2, T3 t3) => (t1, t2, t3).ToRecord();

        /// <summary>
        /// Creates a record of 2.
        /// </summary>
        [Pure]
        public static Record<T1, T2, T3> rec<T1, T2, T3>((T1 t1, T2 t2, T3 t3) tuple) => tuple.ToRecord();

        /// <summary>
        /// Creates a record of 4.
        /// </summary>
        [Pure]
        public static Record<T1, T2, T3, T4> rec<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4) => (t1, t2, t3, t4).ToRecord();

        /// <summary>
        /// Creates a record of 4.
        /// </summary>
        [Pure]
        public static Record<T1, T2, T3, T4> rec<T1, T2, T3, T4>((T1 t1, T2 t2, T3 t3, T4 t4) tuple) => tuple.ToRecord();

        /// <summary>
        /// Creates a record of 5.
        /// </summary>
        [Pure]
        public static Record<T1, T2, T3, T4, T5> rec<T1, T2, T3, T4, T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) => (t1, t2, t3, t4, t5).ToRecord();

        /// <summary>
        /// Creates a record of 5.
        /// </summary>
        [Pure]
        public static Record<T1, T2, T3, T4, T5> rec<T1, T2, T3, T4, T5>((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) tuple) => tuple.ToRecord();
    }
}
