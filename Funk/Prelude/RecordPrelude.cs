using System.Diagnostics.Contracts;

namespace Funk
{
    public static partial class Prelude
    {
        /// <summary>
        /// Creates a record of 1.
        /// </summary>
        [Pure]
        public static Record<T1> record<T1>(T1 t1) => Record.Create(t1);

        /// <summary>
        /// Creates a record of 2.
        /// </summary>
        [Pure]
        public static Record<T1, T2> record<T1, T2>(T1 t1, T2 t2) => Record.Create(t1, t2);

        /// <summary>
        /// Creates a record of 3.
        /// </summary>
        [Pure]
        public static Record<T1, T2, T3> record<T1, T2, T3>(T1 t1, T2 t2, T3 t3) => Record.Create(t1, t2, t3);

        /// <summary>
        /// Creates a record of 4.
        /// </summary>
        [Pure]
        public static Record<T1, T2, T3, T4> record<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4) => Record.Create(t1, t2, t3, t4);

        /// <summary>
        /// Creates a record of 5.
        /// </summary>
        [Pure]
        public static Record<T1, T2, T3, T4, T5> record<T1, T2, T3, T4, T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) => Record.Create(t1, t2, t3, t4, t5);
    }
}
