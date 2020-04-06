using System;
using System.Threading.Tasks;

namespace Funk
{
    /// <summary>
    /// Record with arity of 1.
    /// </summary>
    public struct Record<T1> : IEquatable<Record<T1>>
    {
        internal Record(T1 t1)
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
        /// Structure-preserving map. Maps corresponding record item to the new Record of 1.
        /// </summary>
        public async Task<Record<R1>> MapAsync<R1>(Func<T1, Task<Record<R1>>> selector) => await selector(Item1).ConfigureAwait(false);

        /// <summary>
        /// Structure-preserving map. Maps corresponding record item to the new Record of 1.
        /// </summary>
        public async Task<Record<R1>> MapAsync<R1>(Func<T1, Task<R1>> selector) => Record.Create(await selector(Item1).ConfigureAwait(false));

        /// <summary>
        /// Executes operation provided with record item.
        /// </summary>
        public void Match(Action<T1> operation) => operation(Item1);

        public static implicit operator Record<T1>(T1 t1) => new Record<T1>(t1);

        public static bool operator ==(Record<T1> record, Record<T1> other) => record.Equals(other);

        public static bool operator !=(Record<T1> record, Record<T1> other) => !(record == other);

        public bool Equals(Record<T1> other) => Item1.SafeEquals(other.Item1);

        public override string ToString() => ValueTuple.Create(Item1).ToString();

        public override bool Equals(object obj)
        {
            var @this = this;
            return obj.SafeCast<Record<T1>>().Map(r => @this.Equals(r)).GetOr(_ => false);
        }

        public override int GetHashCode() => ValueTuple.Create(Item1).GetHashCode();
    }

    /// <summary>
    /// Record with arity of 2.
    /// </summary>
    public struct Record<T1, T2> : IEquatable<Record<T1, T2>>
    {
        internal Record(T1 t1, T2 t2)
        {
            Item1 = t1;
            Item2 = t2;
        }

        internal Record((T1 t1, T2 t2) tuple)
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
        /// Structure-preserving map. Maps corresponding record items to the new Record of 2.
        /// </summary>
        public async Task<Record<R1, R2>> MapAsync<R1, R2>(Func<T1, T2, Task<Record<R1, R2>>> selector) => await selector(Item1, Item2).ConfigureAwait(false);

        /// <summary>
        /// Structure-preserving map. Maps corresponding record items to the new Record of 2.
        /// </summary>
        public async Task<Record<R1, R2>> MapAsync<R1, R2>(Func<T1, T2, Task<(R1, R2)>> selector) => Record.Create(await selector(Item1, Item2).ConfigureAwait(false));

        /// <summary>
        /// Executes operation provided with record items.
        /// </summary>
        public void Match(Action<T1, T2> operation) => operation(Item1, Item2);

        public static implicit operator Record<T1, T2>((T1 t1, T2 t2) tuple) => new Record<T1, T2>(tuple);

        public static bool operator ==(Record<T1, T2> record, Record<T1, T2> other) => record.Equals(other);

        public static bool operator !=(Record<T1, T2> record, Record<T1, T2> other) => !(record == other);

        public bool Equals(Record<T1, T2> other) => Item1.SafeEquals(other.Item1) && Item2.SafeEquals(other.Item2);

        public override string ToString() => ValueTuple.Create(Item1, Item2).ToString();

        public override bool Equals(object obj)
        {
            var @this = this;
            return obj.SafeCast<Record<T1, T2>>().Map(r => @this.Equals(r)).GetOr(_ => false);
        }

        public override int GetHashCode() => ValueTuple.Create(Item1, Item2).GetHashCode();
    }

    /// <summary>
    /// Record with arity of 3.
    /// </summary>
    public struct Record<T1, T2, T3> : IEquatable<Record<T1, T2, T3>>
    {
        internal Record(T1 t1, T2 t2, T3 t3)
        {
            Item1 = t1;
            Item2 = t2;
            Item3 = t3;
        }

        internal Record((T1 t1, T2 t2, T3 t3) tuple)
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
        /// Structure-preserving map. Maps corresponding record items to the new Record of 3.
        /// </summary>
        public async Task<Record<R1, R2, R3>> MapAsync<R1, R2, R3>(Func<T1, T2, T3, Task<Record<R1, R2, R3>>> selector) => await selector(Item1, Item2, Item3).ConfigureAwait(false);

        /// <summary>
        /// Structure-preserving map. Maps corresponding record items to the new Record of 3.
        /// </summary>
        public async Task<Record<R1, R2, R3>> MapAsync<R1, R2, R3>(Func<T1, T2, T3, Task<(R1, R2, R3)>> selector) => Record.Create(await selector(Item1, Item2, Item3).ConfigureAwait(false));

        /// <summary>
        /// Executes operation provided with record items.
        /// </summary>
        public void Match(Action<T1, T2, T3> operation) => operation(Item1, Item2, Item3);

        public static implicit operator Record<T1, T2, T3>((T1 t1, T2 t2, T3 t3) tuple) => new Record<T1, T2, T3>(tuple);

        public static bool operator ==(Record<T1, T2, T3> record, Record<T1, T2, T3> other) => record.Equals(other);

        public static bool operator !=(Record<T1, T2, T3> record, Record<T1, T2, T3> other) => !(record == other);

        public bool Equals(Record<T1, T2, T3> other) => Item1.SafeEquals(other.Item1) && Item2.SafeEquals(other.Item2) && Item3.SafeEquals(other.Item3);

        public override string ToString() => ValueTuple.Create(Item1, Item2, Item3).ToString();

        public override bool Equals(object obj)
        {
            var @this = this;
            return obj.SafeCast<Record<T1, T2, T3>>().Map(r => @this.Equals(r)).GetOr(_ => false);
        }

        public override int GetHashCode() => ValueTuple.Create(Item1, Item2, Item3).GetHashCode();
    }

    /// <summary>
    /// Record with arity of 4.
    /// </summary>
    public struct Record<T1, T2, T3, T4> : IEquatable<Record<T1, T2, T3, T4>>
    {
        internal Record(T1 t1, T2 t2, T3 t3, T4 t4)
        {
            Item1 = t1;
            Item2 = t2;
            Item3 = t3;
            Item4 = t4;
        }

        internal Record((T1 t1, T2 t2, T3 t3, T4 t4) tuple)
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
        /// Structure-preserving map. Maps corresponding record items to the new Record of 4.
        /// </summary>
        public async Task<Record<R1, R2, R3, R4>> MapAsync<R1, R2, R3, R4>(Func<T1, T2, T3, T4, Task<Record<R1, R2, R3, R4>>> selector) => await selector(Item1, Item2, Item3, Item4).ConfigureAwait(false);

        /// <summary>
        /// Structure-preserving map. Maps corresponding record items to the new Record of 4.
        /// </summary>
        public async Task<Record<R1, R2, R3, R4>> MapAsync<R1, R2, R3, R4>(Func<T1, T2, T3, T4, Task<(R1, R2, R3, R4)>> selector) => Record.Create(await selector(Item1, Item2, Item3, Item4).ConfigureAwait(false));

        /// <summary>
        /// Executes operation provided with record items.
        /// </summary>
        public void Match(Action<T1, T2, T3, T4> operation) => operation(Item1, Item2, Item3, Item4);

        public static implicit operator Record<T1, T2, T3, T4>((T1 t1, T2 t2, T3 t3, T4 t4) tuple) => new Record<T1, T2, T3, T4>(tuple);

        public static bool operator ==(Record<T1, T2, T3, T4> record, Record<T1, T2, T3, T4> other) => record.Equals(other);

        public static bool operator !=(Record<T1, T2, T3, T4> record, Record<T1, T2, T3, T4> other) => !(record == other);

        public bool Equals(Record<T1, T2, T3, T4> other) => Item1.SafeEquals(other.Item1) && Item2.SafeEquals(other.Item2) && Item3.SafeEquals(other.Item3) && Item4.SafeEquals(other.Item4);

        public override string ToString() => ValueTuple.Create(Item1, Item2, Item3, Item4).ToString();

        public override bool Equals(object obj)
        {
            var @this = this;
            return obj.SafeCast<Record<T1, T2, T3, T4>>().Map(r => @this.Equals(r)).GetOr(_ => false);
        }

        public override int GetHashCode() => ValueTuple.Create(Item1, Item2, Item3, Item4).GetHashCode();
    }

    /// <summary>
    /// Record with arity of 5.
    /// </summary>
    public struct Record<T1, T2, T3, T4, T5> : IEquatable<Record<T1, T2, T3, T4, T5>>
    {
        internal Record(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            Item1 = t1;
            Item2 = t2;
            Item3 = t3;
            Item4 = t4;
            Item5 = t5;
        }

        internal Record((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) tuple)
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
        /// Structure-preserving map. Maps corresponding record items to the new Record of 5.
        /// </summary>
        public async Task<Record<R1, R2, R3, R4, R5>> MapAsync<R1, R2, R3, R4, R5>(Func<T1, T2, T3, T4, T5, Task<Record<R1, R2, R3, R4, R5>>> selector) => await selector(Item1, Item2, Item3, Item4, Item5).ConfigureAwait(false);

        /// <summary>
        /// Structure-preserving map. Maps corresponding record items to the new Record of 5.
        /// </summary>
        public async Task<Record<R1, R2, R3, R4, R5>> MapAsync<R1, R2, R3, R4, R5>(Func<T1, T2, T3, T4, T5, Task<(R1, R2, R3, R4, R5)>> selector) => Record.Create(await selector(Item1, Item2, Item3, Item4, Item5).ConfigureAwait(false));

        /// <summary>
        /// Executes operation provided with record items.
        /// </summary>
        public void Match(Action<T1, T2, T3, T4, T5> operation) => operation(Item1, Item2, Item3, Item4, Item5);

        public static implicit operator Record<T1, T2, T3, T4, T5>((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) tuple) => new Record<T1,T2,T3,T4,T5>(tuple);

        public static bool operator ==(Record<T1, T2, T3, T4, T5> record, Record<T1, T2, T3, T4, T5> other) => record.Equals(other);

        public static bool operator !=(Record<T1, T2, T3, T4, T5> record, Record<T1, T2, T3, T4, T5> other) => !(record == other);

        public bool Equals(Record<T1, T2, T3, T4, T5> other) => Item1.SafeEquals(other.Item1) && Item2.SafeEquals(other.Item2) && Item3.SafeEquals(other.Item3) && Item4.SafeEquals(other.Item4) && Item5.SafeEquals(other.Item5);

        public override string ToString() => ValueTuple.Create(Item1, Item2, Item3, Item4, Item5).ToString();

        public override bool Equals(object obj)
        {
            var @this = this;
            return obj.SafeCast<Record<T1, T2, T3, T4, T5>>().Map(r => @this.Equals(r)).GetOr(_ => false);
        }

        public override int GetHashCode() => ValueTuple.Create(Item1, Item2, Item3, Item4, Item5).GetHashCode();
    }
}
