using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace Funk;

/// <summary>
/// Provides lifting (create) functions for the Record type.
/// </summary>
public static class Record
{
    /// <summary>
    /// Creates a Record of 1.
    /// </summary>
    /// <typeparam name="T1">The type of the value.</typeparam>
    /// <param name="t1">The value.</param>
    /// <returns>A new Record of 1.</returns>
    [Pure]
    public static Record<T1> Create<T1>(T1 t1) => new Record<T1>(t1);

    /// <summary>
    /// Creates a Record of 2.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <param name="t1">The first value.</param>
    /// <param name="t2">The second value.</param>
    /// <returns>A new Record of 2.</returns>
    [Pure]
    public static Record<T1, T2> Create<T1, T2>(T1 t1, T2 t2) => new Record<T1, T2>(t1, t2);

    /// <summary>
    /// Creates a Record of 2 from ValueTuple.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <param name="tuple">The value tuple.</param>
    /// <returns>A new Record of 2.</returns>
    [Pure]
    public static Record<T1, T2> Create<T1, T2>((T1 t1, T2 t2) tuple) => new Record<T1, T2>(tuple);

    /// <summary>
    /// Creates a Record of 3.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <param name="t1">The first value.</param>
    /// <param name="t2">The second value.</param>
    /// <param name="t3">The third value.</param>
    /// <returns>A new Record of 3.</returns>
    [Pure]
    public static Record<T1, T2, T3> Create<T1, T2, T3>(T1 t1, T2 t2, T3 t3) => new Record<T1, T2, T3>(t1, t2, t3);

    /// <summary>
    /// Creates a Record of 3 from ValueTuple.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <param name="tuple">The value tuple.</param>
    /// <returns>A new Record of 3.</returns>
    [Pure]
    public static Record<T1, T2, T3> Create<T1, T2, T3>((T1 t1, T2 t2, T3 t3) tuple) => new Record<T1, T2, T3>(tuple);

    /// <summary>
    /// Creates a Record of 4.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <typeparam name="T4">The type of the fourth value.</typeparam>
    /// <param name="t1">The first value.</param>
    /// <param name="t2">The second value.</param>
    /// <param name="t3">The third value.</param>
    /// <param name="t4">The fourth value.</param>
    /// <returns>A new Record of 4.</returns>
    [Pure]
    public static Record<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4) => new Record<T1, T2, T3, T4>(t1, t2, t3, t4);

    /// <summary>
    /// Creates a Record of 4 from ValueTuple.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <typeparam name="T4">The type of the fourth value.</typeparam>
    /// <param name="tuple">The value tuple.</param>
    /// <returns>A new Record of 4.</returns>
    [Pure]
    public static Record<T1, T2, T3, T4> Create<T1, T2, T3, T4>((T1 t1, T2 t2, T3 t3, T4 t4) tuple) => new Record<T1, T2, T3, T4>(tuple);

    /// <summary>
    /// Creates a Record of 5.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <typeparam name="T4">The type of the fourth value.</typeparam>
    /// <typeparam name="T5">The type of the fifth value.</typeparam>
    /// <param name="t1">The first value.</param>
    /// <param name="t2">The second value.</param>
    /// <param name="t3">The third value.</param>
    /// <param name="t4">The fourth value.</param>
    /// <param name="t5">The fifth value.</param>
    /// <returns>A new Record of 5.</returns>
    [Pure]
    public static Record<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) => new Record<T1, T2, T3, T4, T5>(t1, t2, t3, t4, t5);

    /// <summary>
    /// Creates a Record of 5 from ValueTuple.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <typeparam name="T4">The type of the fourth value.</typeparam>
    /// <typeparam name="T5">The type of the fifth value.</typeparam>
    /// <param name="tuple">The value tuple.</param>
    /// <returns>A new Record of 5.</returns>
    [Pure]
    public static Record<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) tuple) => new Record<T1, T2, T3, T4, T5>(tuple);
}

/// <summary>
/// Record with arity of 1.
/// Type that represents a product of a single value.
/// </summary>
/// <typeparam name="T1">The type of the value.</typeparam>
public readonly struct Record<T1> : IEquatable<Record<T1>>
{
    internal Record(T1 t1)
    {
        Item1 = t1;
    }

    /// <summary>
    /// Underlying value.
    /// </summary>
    [Pure]
    public T1 Item1 { get; }

    /// <summary>
    /// Executes function provided with the underlying value and returns its result.
    /// </summary>
    /// <typeparam name="R">The return type.</typeparam>
    /// <param name="selector">The function to apply to the underlying values.</param>
    /// <returns>The result of the function.</returns>
    public R Match<R>(Func<T1, R> selector) => selector(Item1);

    /// <summary>
    /// Structure-preserving map. Maps this Record to the new Record of 1.
    /// </summary>
    /// <typeparam name="R1">The type of the mapped value.</typeparam>
    /// <param name="selector">The function to apply to the underlying values.</param>
    /// <returns>The Record returned by the selector.</returns>
    public Record<R1> FlatMap<R1>(Func<T1, Record<R1>> selector) => selector(Item1);

    /// <summary>
    /// Structure-preserving map. Maps this Record to the new Record of 1.
    /// In case the specified function returns the Record, use FlatMap instead.
    /// </summary>
    /// <typeparam name="R1">The type of the mapped value.</typeparam>
    /// <param name="selector">The function to apply to the underlying values.</param>
    /// <returns>A new Record containing the mapped values.</returns>
    public Record<R1> Map<R1>(Func<T1, R1> selector) => Record.Create(selector(Item1));

    /// <summary>
    /// Structure-preserving map. Maps this Record to the new Record of 1.
    /// </summary>
    /// <typeparam name="R1">The type of the mapped value.</typeparam>
    /// <param name="selector">The function to apply to the underlying values.</param>
    /// <returns>The Record returned by the selector.</returns>
    public async Task<Record<R1>> FlatMapAsync<R1>(Func<T1, Task<Record<R1>>> selector) => await selector(Item1).ConfigureAwait(false);

    /// <summary>
    /// Structure-preserving map. Maps this Record to the new Record of 1.
    /// In case the specified function returns the Record, use FlatMapAsync instead.
    /// </summary>
    /// <typeparam name="R1">The type of the mapped value.</typeparam>
    /// <param name="selector">The function to apply to the underlying values.</param>
    /// <returns>A new Record containing the mapped values.</returns>
    public async Task<Record<R1>> MapAsync<R1>(Func<T1, Task<R1>> selector) => Record.Create(await selector(Item1).ConfigureAwait(false));

    /// <summary>
    /// Executes non-returning function provided with the underlying value.
    /// </summary>
    /// <param name="operation">The action to execute with the underlying values.</param>
    public void Match(Action<T1> operation) => operation(Item1);

    /// <summary>
    /// Deconstructs the Record to the underlying value. 
    /// </summary>
    /// <param name="item1">The first value.</param>
    [Pure]
    public void Deconstruct(out T1 item1)
    {
        item1 = Item1;
    }

    /// <summary>
    /// Lifts the object to the Record of 1. 
    /// </summary>
    [Pure]
    public static implicit operator Record<T1>(T1 t1) => new Record<T1>(t1);

    /// <summary>
    /// ValueTuple based equality comparison.
    /// </summary>
    [Pure]
    public static bool operator ==(Record<T1> record, Record<T1> other) => record.Equals(other);

    /// <summary>
    /// ValueTuple based equality comparison.
    /// </summary>
    [Pure]
    public static bool operator !=(Record<T1> record, Record<T1> other) => !(record == other);

    /// <summary>
    /// ValueTuple based equality comparison.
    /// </summary>
    /// <param name="other">The other Record to compare with.</param>
    /// <returns>True if the Records are equal, false otherwise.</returns>
    [Pure]
    public bool Equals(Record<T1> other) => ValueTuple.Create(Item1).SafeEquals(ValueTuple.Create(other.Item1));

    /// <summary>
    /// ValueTuple based ToString method.
    /// </summary>
    public override string ToString() => ValueTuple.Create(Item1).ToString();

    /// <summary>
    /// ValueTuple based equality comparison.
    /// If the other object is not Record of 1 of the same type, returns false.
    /// </summary>
    /// <param name="obj">The object to compare with.</param>
    /// <returns>True if the objects are equal, false otherwise.</returns>
    [Pure]
    public override bool Equals(object obj)
    {
        var @this = this;
        return obj.SafeCast<Record<T1>>().Map(r => @this.Equals(r)).GetOrDefault();
    }

    /// <summary>
    /// ValueTuple based GetHashCode method.
    /// </summary>
    public override int GetHashCode() => ValueTuple.Create(Item1).GetHashCode();
}

/// <summary>
/// Record with arity of 2.
/// Type that represents a product of two values.
/// </summary>
/// <typeparam name="T1">The type of the first value.</typeparam>
/// <typeparam name="T2">The type of the second value.</typeparam>
public readonly struct Record<T1, T2> : IEquatable<Record<T1, T2>>
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

    /// <summary>
    /// First underlying value.
    /// </summary>
    [Pure]
    public T1 Item1 { get; }
        
    /// <summary>
    /// Second underlying value.
    /// </summary>
    [Pure]
    public T2 Item2 { get; }

    /// <summary>
    /// Executes function provided with the underlying values and returns its result.
    /// </summary>
    /// <typeparam name="R">The return type.</typeparam>
    /// <param name="selector">The function to apply to the underlying values.</param>
    /// <returns>The result of the function.</returns>
    public R Match<R>(Func<T1, T2, R> selector) => selector(Item1, Item2);

    /// <summary>
    /// Structure-preserving map. Maps this Record to the new Record of 2.
    /// </summary>
    /// <typeparam name="R1">The type of the first mapped value.</typeparam>
    /// <typeparam name="R2">The type of the second mapped value.</typeparam>
    /// <param name="selector">The function to apply to the underlying values.</param>
    /// <returns>The Record returned by the selector.</returns>
    public Record<R1, R2> FlatMap<R1, R2>(Func<T1, T2, Record<R1, R2>> selector) => selector(Item1, Item2);

    /// <summary>
    /// Structure-preserving map. Maps this Record to the new Record of 2.
    /// In case the specified function returns the Record, use FlatMap instead.
    /// </summary>
    /// <typeparam name="R1">The type of the first mapped value.</typeparam>
    /// <typeparam name="R2">The type of the second mapped value.</typeparam>
    /// <param name="selector">The function to apply to the underlying values.</param>
    /// <returns>A new Record containing the mapped values.</returns>
    public Record<R1, R2> Map<R1, R2>(Func<T1, T2, (R1, R2)> selector) => Record.Create(selector(Item1, Item2));

    /// <summary>
    /// Structure-preserving map. Maps this Record to the new Record of 2.
    /// </summary>
    /// <typeparam name="R1">The type of the first mapped value.</typeparam>
    /// <typeparam name="R2">The type of the second mapped value.</typeparam>
    /// <param name="selector">The function to apply to the underlying values.</param>
    /// <returns>The Record returned by the selector.</returns>
    public async Task<Record<R1, R2>> FlatMapAsync<R1, R2>(Func<T1, T2, Task<Record<R1, R2>>> selector) => await selector(Item1, Item2).ConfigureAwait(false);

    /// <summary>
    /// Structure-preserving map. Maps this Record to the new Record of 2.
    /// In case the specified function returns the Record, use FlatMapAsync instead.
    /// </summary>
    /// <typeparam name="R1">The type of the first mapped value.</typeparam>
    /// <typeparam name="R2">The type of the second mapped value.</typeparam>
    /// <param name="selector">The function to apply to the underlying values.</param>
    /// <returns>A new Record containing the mapped values.</returns>
    public async Task<Record<R1, R2>> MapAsync<R1, R2>(Func<T1, T2, Task<(R1, R2)>> selector) => Record.Create(await selector(Item1, Item2).ConfigureAwait(false));

    /// <summary>
    /// Executes non-returning function provided with the underlying values.
    /// </summary>
    /// <param name="operation">The action to execute with the underlying values.</param>
    public void Match(Action<T1, T2> operation) => operation(Item1, Item2);

    /// <summary>
    /// Deconstructs the Record to underlying values. 
    /// </summary>
    /// <param name="item1">The first value.</param>
    /// <param name="item2">The second value.</param>
    [Pure]
    public void Deconstruct(out T1 item1, out T2 item2)
    {
        item1 = Item1;
        item2 = Item2;
    }

    /// <summary>
    /// Lifts the ValueTuple to the Record of 2. 
    /// </summary>
    [Pure]
    public static implicit operator Record<T1, T2>((T1 t1, T2 t2) tuple) => new Record<T1, T2>(tuple);

    /// <summary>
    /// ValueTuple based equality comparison.
    /// </summary>
    [Pure]
    public static bool operator ==(Record<T1, T2> record, Record<T1, T2> other) => record.Equals(other);

    /// <summary>
    /// ValueTuple based equality comparison.
    /// </summary>
    [Pure]
    public static bool operator !=(Record<T1, T2> record, Record<T1, T2> other) => !(record == other);

    /// <summary>
    /// ValueTuple based equality comparison.
    /// </summary>
    /// <param name="other">The other Record to compare with.</param>
    /// <returns>True if the Records are equal, false otherwise.</returns>
    [Pure]
    public bool Equals(Record<T1, T2> other) => ValueTuple.Create(Item1, Item2).SafeEquals(ValueTuple.Create(other.Item1, other.Item2));

    /// <summary>
    /// ValueTuple based ToString method.
    /// </summary>
    public override string ToString() => ValueTuple.Create(Item1, Item2).ToString();

    /// <summary>
    /// ValueTuple based equality comparison.
    /// If the other object is not Record of 2 of same types, returns false.
    /// </summary>
    /// <param name="obj">The object to compare with.</param>
    /// <returns>True if the objects are equal, false otherwise.</returns>
    [Pure]
    public override bool Equals(object obj)
    {
        var @this = this;
        return obj.SafeCast<Record<T1, T2>>().Map(r => @this.Equals(r)).GetOrDefault();
    }

    /// <summary>
    /// ValueTuple based GetHashCode method.
    /// </summary>
    public override int GetHashCode() => ValueTuple.Create(Item1, Item2).GetHashCode();
}

/// <summary>
/// Record with arity of 3.
/// Type that represents a product of three values.
/// </summary>
/// <typeparam name="T1">The type of the first value.</typeparam>
/// <typeparam name="T2">The type of the second value.</typeparam>
/// <typeparam name="T3">The type of the third value.</typeparam>
public readonly struct Record<T1, T2, T3> : IEquatable<Record<T1, T2, T3>>
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

    /// <summary>
    /// First underlying value.
    /// </summary>
    [Pure]
    public T1 Item1 { get; }
        
    /// <summary>
    /// Second underlying value.
    /// </summary>
    [Pure]
    public T2 Item2 { get; }
        
    /// <summary>
    /// Third underlying value.
    /// </summary>
    [Pure]
    public T3 Item3 { get; }

    /// <summary>
    /// Executes function provided with the underlying values and returns its result.
    /// </summary>
    /// <typeparam name="R">The return type.</typeparam>
    /// <param name="selector">The function to apply to the underlying values.</param>
    /// <returns>The result of the function.</returns>
    public R Match<R>(Func<T1, T2, T3, R> selector) => selector(Item1, Item2, Item3);

    /// <summary>
    /// Structure-preserving map. Maps this Record to the new Record of 3.
    /// </summary>
    /// <typeparam name="R1">The type of the first mapped value.</typeparam>
    /// <typeparam name="R2">The type of the second mapped value.</typeparam>
    /// <typeparam name="R3">The type of the third mapped value.</typeparam>
    /// <param name="selector">The function to apply to the underlying values.</param>
    /// <returns>The Record returned by the selector.</returns>
    public Record<R1, R2, R3> FlatMap<R1, R2, R3>(Func<T1, T2, T3, Record<R1, R2, R3>> selector) => selector(Item1, Item2, Item3);

    /// <summary>
    /// Structure-preserving map. Maps this Record to the new Record of 3.
    /// In case the specified function returns the Record, use FlatMap instead.
    /// </summary>
    /// <typeparam name="R1">The type of the first mapped value.</typeparam>
    /// <typeparam name="R2">The type of the second mapped value.</typeparam>
    /// <typeparam name="R3">The type of the third mapped value.</typeparam>
    /// <param name="selector">The function to apply to the underlying values.</param>
    /// <returns>A new Record containing the mapped values.</returns>
    public Record<R1, R2, R3> Map<R1, R2, R3>(Func<T1, T2, T3, (R1, R2, R3)> selector) => Record.Create(selector(Item1, Item2, Item3));

    /// <summary>
    /// Structure-preserving map. Maps this Record to the new Record of 3.
    /// </summary>
    /// <typeparam name="R1">The type of the first mapped value.</typeparam>
    /// <typeparam name="R2">The type of the second mapped value.</typeparam>
    /// <typeparam name="R3">The type of the third mapped value.</typeparam>
    /// <param name="selector">The function to apply to the underlying values.</param>
    /// <returns>The Record returned by the selector.</returns>
    public async Task<Record<R1, R2, R3>> FlatMapAsync<R1, R2, R3>(Func<T1, T2, T3, Task<Record<R1, R2, R3>>> selector) => await selector(Item1, Item2, Item3).ConfigureAwait(false);

    /// <summary>
    /// Structure-preserving map. Maps this Record to the new Record of 3.
    /// In case the specified function returns the Record, use FlatMapAsync instead.
    /// </summary>
    /// <typeparam name="R1">The type of the first mapped value.</typeparam>
    /// <typeparam name="R2">The type of the second mapped value.</typeparam>
    /// <typeparam name="R3">The type of the third mapped value.</typeparam>
    /// <param name="selector">The function to apply to the underlying values.</param>
    /// <returns>A new Record containing the mapped values.</returns>
    public async Task<Record<R1, R2, R3>> MapAsync<R1, R2, R3>(Func<T1, T2, T3, Task<(R1, R2, R3)>> selector) => Record.Create(await selector(Item1, Item2, Item3).ConfigureAwait(false));

    /// <summary>
    /// Executes non-returning function provided with the underlying values.
    /// </summary>
    /// <param name="operation">The action to execute with the underlying values.</param>
    public void Match(Action<T1, T2, T3> operation) => operation(Item1, Item2, Item3);
        
    /// <summary>
    /// Deconstructs the Record to underlying values. 
    /// </summary>
    /// <param name="item1">The first value.</param>
    /// <param name="item2">The second value.</param>
    /// <param name="item3">The third value.</param>
    [Pure]
    public void Deconstruct(out T1 item1, out T2 item2, out T3 item3)
    {
        item1 = Item1;
        item2 = Item2;
        item3 = Item3;
    }

    /// <summary>
    /// Lifts the ValueTuple to the Record of 3. 
    /// </summary>
    [Pure]
    public static implicit operator Record<T1, T2, T3>((T1 t1, T2 t2, T3 t3) tuple) => new Record<T1, T2, T3>(tuple);

    /// <summary>
    /// ValueTuple based equality comparison.
    /// </summary>
    [Pure]
    public static bool operator ==(Record<T1, T2, T3> record, Record<T1, T2, T3> other) => record.Equals(other);

    /// <summary>
    /// ValueTuple based equality comparison.
    /// </summary>
    [Pure]
    public static bool operator !=(Record<T1, T2, T3> record, Record<T1, T2, T3> other) => !(record == other);

    /// <summary>
    /// ValueTuple based equality comparison.
    /// </summary>
    /// <param name="other">The other Record to compare with.</param>
    /// <returns>True if the Records are equal, false otherwise.</returns>
    [Pure]
    public bool Equals(Record<T1, T2, T3> other) => ValueTuple.Create(Item1, Item2, Item3).SafeEquals(ValueTuple.Create(other.Item1, other.Item2, other.Item3));

    /// <summary>
    /// ValueTuple based ToString method.
    /// </summary>
    public override string ToString() => ValueTuple.Create(Item1, Item2, Item3).ToString();

    /// <summary>
    /// ValueTuple based equality comparison.
    /// If the other object is not Record of 3 of same types, returns false.
    /// </summary>
    /// <param name="obj">The object to compare with.</param>
    /// <returns>True if the objects are equal, false otherwise.</returns>
    [Pure]
    public override bool Equals(object obj)
    {
        var @this = this;
        return obj.SafeCast<Record<T1, T2, T3>>().Map(r => @this.Equals(r)).GetOrDefault();
    }

    /// <summary>
    /// ValueTuple based GetHashCode method.
    /// </summary>
    public override int GetHashCode() => ValueTuple.Create(Item1, Item2, Item3).GetHashCode();
}

/// <summary>
/// Record with arity of 4.
/// Type that represents a product of four values.
/// </summary>
/// <typeparam name="T1">The type of the first value.</typeparam>
/// <typeparam name="T2">The type of the second value.</typeparam>
/// <typeparam name="T3">The type of the third value.</typeparam>
/// <typeparam name="T4">The type of the fourth value.</typeparam>
public readonly struct Record<T1, T2, T3, T4> : IEquatable<Record<T1, T2, T3, T4>>
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

    /// <summary>
    /// First underlying value.
    /// </summary>
    [Pure]
    public T1 Item1 { get; }
        
    /// <summary>
    /// Second underlying value.
    /// </summary>
    [Pure]
    public T2 Item2 { get; }
        
    /// <summary>
    /// Third underlying value.
    /// </summary>
    [Pure]
    public T3 Item3 { get; }
        
    /// <summary>
    /// Fourth underlying value.
    /// </summary>
    [Pure]
    public T4 Item4 { get; }

    /// <summary>
    /// Executes function provided with the underlying values and returns its result.
    /// </summary>
    /// <typeparam name="R">The return type.</typeparam>
    /// <param name="selector">The function to apply to the underlying values.</param>
    /// <returns>The result of the function.</returns>
    public R Match<R>(Func<T1, T2, T3, T4, R> selector) => selector(Item1, Item2, Item3, Item4);

    /// <summary>
    /// Structure-preserving map. Maps this Record to the new Record of 4.
    /// </summary>
    /// <typeparam name="R1">The type of the first mapped value.</typeparam>
    /// <typeparam name="R2">The type of the second mapped value.</typeparam>
    /// <typeparam name="R3">The type of the third mapped value.</typeparam>
    /// <typeparam name="R4">The type of the fourth mapped value.</typeparam>
    /// <param name="selector">The function to apply to the underlying values.</param>
    /// <returns>The Record returned by the selector.</returns>
    public Record<R1, R2, R3, R4> FlatMap<R1, R2, R3, R4>(Func<T1, T2, T3, T4, Record<R1, R2, R3, R4>> selector) => selector(Item1, Item2, Item3, Item4);

    /// <summary>
    /// Structure-preserving map. Maps this Record to the new Record of 4.
    /// In case the specified function returns the Record, use FlatMap instead.
    /// </summary>
    /// <typeparam name="R1">The type of the first mapped value.</typeparam>
    /// <typeparam name="R2">The type of the second mapped value.</typeparam>
    /// <typeparam name="R3">The type of the third mapped value.</typeparam>
    /// <typeparam name="R4">The type of the fourth mapped value.</typeparam>
    /// <param name="selector">The function to apply to the underlying values.</param>
    /// <returns>A new Record containing the mapped values.</returns>
    public Record<R1, R2, R3, R4> Map<R1, R2, R3, R4>(Func<T1, T2, T3, T4, (R1, R2, R3, R4)> selector) => Record.Create(selector(Item1, Item2, Item3, Item4));

    /// <summary>
    /// Structure-preserving map. Maps this Record to the new Record of 4.
    /// </summary>
    /// <typeparam name="R1">The type of the first mapped value.</typeparam>
    /// <typeparam name="R2">The type of the second mapped value.</typeparam>
    /// <typeparam name="R3">The type of the third mapped value.</typeparam>
    /// <typeparam name="R4">The type of the fourth mapped value.</typeparam>
    /// <param name="selector">The function to apply to the underlying values.</param>
    /// <returns>The Record returned by the selector.</returns>
    public async Task<Record<R1, R2, R3, R4>> FlatMapAsync<R1, R2, R3, R4>(Func<T1, T2, T3, T4, Task<Record<R1, R2, R3, R4>>> selector) => await selector(Item1, Item2, Item3, Item4).ConfigureAwait(false);

    /// <summary>
    /// Structure-preserving map. Maps this Record to the new Record of 4.
    /// In case the specified function returns the Record, use FlatMapAsync instead.
    /// </summary>
    /// <typeparam name="R1">The type of the first mapped value.</typeparam>
    /// <typeparam name="R2">The type of the second mapped value.</typeparam>
    /// <typeparam name="R3">The type of the third mapped value.</typeparam>
    /// <typeparam name="R4">The type of the fourth mapped value.</typeparam>
    /// <param name="selector">The function to apply to the underlying values.</param>
    /// <returns>A new Record containing the mapped values.</returns>
    public async Task<Record<R1, R2, R3, R4>> MapAsync<R1, R2, R3, R4>(Func<T1, T2, T3, T4, Task<(R1, R2, R3, R4)>> selector) => Record.Create(await selector(Item1, Item2, Item3, Item4).ConfigureAwait(false));

    /// <summary>
    /// Executes non-returning function provided with the underlying values.
    /// </summary>
    /// <param name="operation">The action to execute with the underlying values.</param>
    public void Match(Action<T1, T2, T3, T4> operation) => operation(Item1, Item2, Item3, Item4);
        
    /// <summary>
    /// Deconstructs the Record to underlying values. 
    /// </summary>
    /// <param name="item1">The first value.</param>
    /// <param name="item2">The second value.</param>
    /// <param name="item3">The third value.</param>
    /// <param name="item4">The fourth value.</param>
    [Pure]
    public void Deconstruct(out T1 item1, out T2 item2, out T3 item3, out T4 item4)
    {
        item1 = Item1;
        item2 = Item2;
        item3 = Item3;
        item4 = Item4;
    }

    /// <summary>
    /// Lifts the ValueTuple to the Record of 4. 
    /// </summary>
    [Pure]
    public static implicit operator Record<T1, T2, T3, T4>((T1 t1, T2 t2, T3 t3, T4 t4) tuple) => new Record<T1, T2, T3, T4>(tuple);

    /// <summary>
    /// ValueTuple based equality comparison.
    /// </summary>
    [Pure]
    public static bool operator ==(Record<T1, T2, T3, T4> record, Record<T1, T2, T3, T4> other) => record.Equals(other);

    /// <summary>
    /// ValueTuple based equality comparison.
    /// </summary>
    [Pure]
    public static bool operator !=(Record<T1, T2, T3, T4> record, Record<T1, T2, T3, T4> other) => !(record == other);

    /// <summary>
    /// ValueTuple based equality comparison.
    /// </summary>
    /// <param name="other">The other Record to compare with.</param>
    /// <returns>True if the Records are equal, false otherwise.</returns>
    [Pure]
    public bool Equals(Record<T1, T2, T3, T4> other) => ValueTuple.Create(Item1, Item2, Item3, Item4).SafeEquals(ValueTuple.Create(other.Item1, other.Item2, other.Item3, other.Item4));

    /// <summary>
    /// ValueTuple based ToString method.
    /// </summary>
    public override string ToString() => ValueTuple.Create(Item1, Item2, Item3, Item4).ToString();

    /// <summary>
    /// ValueTuple based equality comparison.
    /// If the other object is not Record of 4 of same types, returns false.
    /// </summary>
    /// <param name="obj">The object to compare with.</param>
    /// <returns>True if the objects are equal, false otherwise.</returns>
    [Pure]
    public override bool Equals(object obj)
    {
        var @this = this;
        return obj.SafeCast<Record<T1, T2, T3, T4>>().Map(r => @this.Equals(r)).GetOrDefault();
    }

    /// <summary>
    /// ValueTuple based GetHashCode method.
    /// </summary>
    public override int GetHashCode() => ValueTuple.Create(Item1, Item2, Item3, Item4).GetHashCode();
}

/// <summary>
/// Record with arity of 5.
/// Type that represents a product of five values.
/// </summary>
/// <typeparam name="T1">The type of the first value.</typeparam>
/// <typeparam name="T2">The type of the second value.</typeparam>
/// <typeparam name="T3">The type of the third value.</typeparam>
/// <typeparam name="T4">The type of the fourth value.</typeparam>
/// <typeparam name="T5">The type of the fifth value.</typeparam>
public readonly struct Record<T1, T2, T3, T4, T5> : IEquatable<Record<T1, T2, T3, T4, T5>>
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

    /// <summary>
    /// First underlying value.
    /// </summary>
    [Pure]
    public T1 Item1 { get; }
        
    /// <summary>
    /// Second underlying value.
    /// </summary>
    [Pure]
    public T2 Item2 { get; }
        
    /// <summary>
    /// Third underlying value.
    /// </summary>
    [Pure]
    public T3 Item3 { get; }
        
    /// <summary>
    /// Fourth underlying value.
    /// </summary>
    [Pure]
    public T4 Item4 { get; }
        
    /// <summary>
    /// Fifth underlying value.
    /// </summary>
    [Pure]
    public T5 Item5 { get; }

    /// <summary>
    /// Executes function provided with the underlying values and returns its result.
    /// </summary>
    /// <typeparam name="R">The return type.</typeparam>
    /// <param name="selector">The function to apply to the underlying values.</param>
    /// <returns>The result of the function.</returns>
    public R Match<R>(Func<T1, T2, T3, T4, T5, R> selector) => selector(Item1, Item2, Item3, Item4, Item5);

    /// <summary>
    /// Structure-preserving map. Maps this Record to the new Record of 5.
    /// </summary>
    /// <typeparam name="R1">The type of the first mapped value.</typeparam>
    /// <typeparam name="R2">The type of the second mapped value.</typeparam>
    /// <typeparam name="R3">The type of the third mapped value.</typeparam>
    /// <typeparam name="R4">The type of the fourth mapped value.</typeparam>
    /// <typeparam name="R5">The type of the fifth mapped value.</typeparam>
    /// <param name="selector">The function to apply to the underlying values.</param>
    /// <returns>The Record returned by the selector.</returns>
    public Record<R1, R2, R3, R4, R5> FlatMap<R1, R2, R3, R4, R5>(Func<T1, T2, T3, T4, T5, Record<R1, R2, R3, R4, R5>> selector) => selector(Item1, Item2, Item3, Item4, Item5);

    /// <summary>
    /// Structure-preserving map. Maps this Record to the new Record of 5.
    /// In case the specified function returns the Record, use FlatMap instead.
    /// </summary>
    /// <typeparam name="R1">The type of the first mapped value.</typeparam>
    /// <typeparam name="R2">The type of the second mapped value.</typeparam>
    /// <typeparam name="R3">The type of the third mapped value.</typeparam>
    /// <typeparam name="R4">The type of the fourth mapped value.</typeparam>
    /// <typeparam name="R5">The type of the fifth mapped value.</typeparam>
    /// <param name="selector">The function to apply to the underlying values.</param>
    /// <returns>A new Record containing the mapped values.</returns>
    public Record<R1, R2, R3, R4, R5> Map<R1, R2, R3, R4, R5>(Func<T1, T2, T3, T4, T5, (R1, R2, R3, R4, R5)> selector) => Record.Create(selector(Item1, Item2, Item3, Item4, Item5));

    /// <summary>
    /// Structure-preserving map. Maps this Record to the new Record of 5.
    /// </summary>
    /// <typeparam name="R1">The type of the first mapped value.</typeparam>
    /// <typeparam name="R2">The type of the second mapped value.</typeparam>
    /// <typeparam name="R3">The type of the third mapped value.</typeparam>
    /// <typeparam name="R4">The type of the fourth mapped value.</typeparam>
    /// <typeparam name="R5">The type of the fifth mapped value.</typeparam>
    /// <param name="selector">The function to apply to the underlying values.</param>
    /// <returns>The Record returned by the selector.</returns>
    public async Task<Record<R1, R2, R3, R4, R5>> FlatMapAsync<R1, R2, R3, R4, R5>(Func<T1, T2, T3, T4, T5, Task<Record<R1, R2, R3, R4, R5>>> selector) => await selector(Item1, Item2, Item3, Item4, Item5).ConfigureAwait(false);

    /// <summary>
    /// Structure-preserving map. Maps this Record to the new Record of 5.
    /// In case the specified function returns the Record, use FlatMapAsync instead.
    /// </summary>
    /// <typeparam name="R1">The type of the first mapped value.</typeparam>
    /// <typeparam name="R2">The type of the second mapped value.</typeparam>
    /// <typeparam name="R3">The type of the third mapped value.</typeparam>
    /// <typeparam name="R4">The type of the fourth mapped value.</typeparam>
    /// <typeparam name="R5">The type of the fifth mapped value.</typeparam>
    /// <param name="selector">The function to apply to the underlying values.</param>
    /// <returns>A new Record containing the mapped values.</returns>
    public async Task<Record<R1, R2, R3, R4, R5>> MapAsync<R1, R2, R3, R4, R5>(Func<T1, T2, T3, T4, T5, Task<(R1, R2, R3, R4, R5)>> selector) => Record.Create(await selector(Item1, Item2, Item3, Item4, Item5).ConfigureAwait(false));

    /// <summary>
    /// Executes non-returning function provided with the underlying values.
    /// </summary>
    /// <param name="operation">The action to execute with the underlying values.</param>
    public void Match(Action<T1, T2, T3, T4, T5> operation) => operation(Item1, Item2, Item3, Item4, Item5);
        
    /// <summary>
    /// Deconstructs the Record to underlying values. 
    /// </summary>
    /// <param name="item1">The first value.</param>
    /// <param name="item2">The second value.</param>
    /// <param name="item3">The third value.</param>
    /// <param name="item4">The fourth value.</param>
    /// <param name="item5">The fifth value.</param>
    [Pure]
    public void Deconstruct(out T1 item1, out T2 item2, out T3 item3, out T4 item4, out T5 item5)
    {
        item1 = Item1;
        item2 = Item2;
        item3 = Item3;
        item4 = Item4;
        item5 = Item5;
    }

    /// <summary>
    /// Lifts the ValueTuple to the Record of 5. 
    /// </summary>
    [Pure]
    public static implicit operator Record<T1, T2, T3, T4, T5>((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) tuple) => new Record<T1, T2, T3, T4, T5>(tuple);

    /// <summary>
    /// ValueTuple based equality comparison.
    /// </summary>
    [Pure]
    public static bool operator ==(Record<T1, T2, T3, T4, T5> record, Record<T1, T2, T3, T4, T5> other) => record.Equals(other);

    /// <summary>
    /// ValueTuple based equality comparison.
    /// </summary>
    [Pure]
    public static bool operator !=(Record<T1, T2, T3, T4, T5> record, Record<T1, T2, T3, T4, T5> other) => !(record == other);

    /// <summary>
    /// ValueTuple based equality comparison.
    /// </summary>
    /// <param name="other">The other Record to compare with.</param>
    /// <returns>True if the Records are equal, false otherwise.</returns>
    [Pure]
    public bool Equals(Record<T1, T2, T3, T4, T5> other) => ValueTuple.Create(Item1, Item2, Item3, Item4, Item5).SafeEquals(ValueTuple.Create(other.Item1, other.Item2, other.Item3, other.Item4, other.Item5));

    /// <summary>
    /// ValueTuple based ToString method.
    /// </summary>
    public override string ToString() => ValueTuple.Create(Item1, Item2, Item3, Item4, Item5).ToString();

    /// <summary>
    /// ValueTuple based equality comparison.
    /// If the other object is not Record of 5 of same types, returns false.
    /// </summary>
    /// <param name="obj">The object to compare with.</param>
    /// <returns>True if the objects are equal, false otherwise.</returns>
    [Pure]
    public override bool Equals(object obj)
    {
        var @this = this;
        return obj.SafeCast<Record<T1, T2, T3, T4, T5>>().Map(r => @this.Equals(r)).GetOrDefault();
    }

    /// <summary>
    /// ValueTuple based GetHashCode method.
    /// </summary>
    public override int GetHashCode() => ValueTuple.Create(Item1, Item2, Item3, Item4, Item5).GetHashCode();
}