namespace Funk
{
    public static partial class Operators
    {
        public static Record<T1> record<T1>(T1 t1)
        {
            return Record.Create(t1);
        }

        public static Record<T1, T2> record<T1, T2>(T1 t1, T2 t2)
        {
            return Record.Create(t1, t2);
        }

        public static Record<T1, T2, T3> record<T1, T2, T3>(T1 t1, T2 t2, T3 t3)
        {
            return Record.Create(t1, t2, t3);
        }
    }
}
