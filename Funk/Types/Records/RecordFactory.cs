namespace Funk
{
    public static class Record
    {
        public static Record<T1> Create<T1>(T1 t1)
        {
            return new Record<T1>(t1);
        }

        public static Record<T1, T2> Create<T1, T2>(T1 t1, T2 t2)
        {
            return new Record<T1, T2>(t1, t2);
        }

        public static Record<T1, T2> Create<T1, T2>((T1 t1, T2 t2) tuple)
        {
            return new Record<T1, T2>(tuple.t1, tuple.t2);
        }

        public static Record<T1, T2> ToRecord<T1, T2>(this (T1 t1, T2 t2) tuple)
        {
            return Create(tuple);
        }

        public static Record<T1, T2, T3> Create<T1, T2, T3>(T1 t1, T2 t2, T3 t3)
        {
            return new Record<T1, T2, T3>(t1, t2, t3);
        }

        public static Record<T1, T2, T3> Create<T1, T2, T3>((T1 t1, T2 t2, T3 t3) tuple)
        {
            return new Record<T1, T2, T3>(tuple.t1, tuple.t2, tuple.t3);
        }

        public static Record<T1, T2, T3> ToRecord<T1, T2, T3>(this (T1 t1, T2 t2, T3 t3) tuple)
        {
            return Create(tuple);
        }
    }
}
