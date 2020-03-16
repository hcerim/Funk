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
            return new Record<T1, T2>(tuple);
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
            return new Record<T1, T2, T3>(tuple);
        }

        public static Record<T1, T2, T3> ToRecord<T1, T2, T3>(this (T1 t1, T2 t2, T3 t3) tuple)
        {
            return Create(tuple);
        }

        public static Record<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4)
        {
            return new Record<T1, T2, T3, T4>(t1, t2, t3, t4);
        }

        public static Record<T1, T2, T3, T4> Create<T1, T2, T3, T4>((T1 t1, T2 t2, T3 t3, T4 t4) tuple)
        {
            return new Record<T1, T2, T3, T4>(tuple);
        }

        public static Record<T1, T2, T3, T4> ToRecord<T1, T2, T3, T4>(this (T1 t1, T2 t2, T3 t3, T4 t4) tuple)
        {
            return Create(tuple);
        }

        public static Record<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            return new Record<T1, T2, T3, T4, T5>(t1, t2, t3, t4, t5);
        }

        public static Record<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) tuple)
        {
            return new Record<T1, T2, T3, T4, T5>(tuple);
        }

        public static Record<T1, T2, T3, T4, T5> ToRecord<T1, T2, T3, T4, T5>(this (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) tuple)
        {
            return Create(tuple);
        }
    }
}
