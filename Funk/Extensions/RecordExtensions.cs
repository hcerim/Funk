namespace Funk
{
    public static class RecordExtensions
    {
        /// <summary>
        /// Creates a record of 1.
        /// </summary>
        /// <returns></returns>
        public static Record<T1> ToRecord<T1>(this T1 t1)
        {
            return Record.Create(t1);
        }

        /// <summary>
        /// Creates a record of 2.
        /// </summary>
        /// <returns></returns>
        public static Record<T1, T2> ToRecord<T1, T2>(this (T1 t1, T2 t2) tuple)
        {
            return Record.Create(tuple);
        }

        /// <summary>
        /// Creates a record of 3.
        /// </summary>
        /// <returns></returns>
        public static Record<T1, T2, T3> ToRecord<T1, T2, T3>(this (T1 t1, T2 t2, T3 t3) tuple)
        {
            return Record.Create(tuple);
        }

        /// <summary>
        /// Creates a record of 4.
        /// </summary>
        /// <returns></returns>
        public static Record<T1, T2, T3, T4> ToRecord<T1, T2, T3, T4>(this (T1 t1, T2 t2, T3 t3, T4 t4) tuple)
        {
            return Record.Create(tuple);
        }

        /// <summary>
        /// Creates a record of 5.
        /// </summary>
        /// <returns></returns>
        public static Record<T1, T2, T3, T4, T5> ToRecord<T1, T2, T3, T4, T5>(this (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) tuple)
        {
            return Record.Create(tuple);
        }
    }
}
