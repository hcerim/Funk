using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace Funk;

/// <summary>
/// Provides the Record related extension methods.
/// </summary>
public static class RecordExt
{
    extension<T1>(Task<Record<T1>> record)
    {
        /// <summary>
        /// Structure-preserving map. Maps this Record to the new Record of 1.
        /// </summary>
        public async Task<Record<R1>> FlatMapAsync<R1>(Func<T1, Task<Record<R1>>> selector) => await (await record).FlatMapAsync(selector).ConfigureAwait(false);

        /// <summary>
        /// Structure-preserving map. Maps this Record to the new Record of 1.
        /// In case the the specified function returns the Record, use FlatMapAsync instead.
        /// </summary>
        public async Task<Record<R1>> MapAsync<R1>(Func<T1, Task<R1>> selector) => await (await record).MapAsync(selector).ConfigureAwait(false);
    }

    extension<T1, T2>(Task<Record<T1, T2>> record)
    {
        /// <summary>
        /// Structure-preserving map. Maps this Record to the new Record of 2.
        /// </summary>
        public async Task<Record<R1, R2>> FlatMapAsync<R1, R2>(Func<T1, T2, Task<Record<R1, R2>>> selector) => await (await record).FlatMapAsync(selector).ConfigureAwait(false);

        /// <summary>
        /// Structure-preserving map. Maps this Record to the new Record of 2.
        /// In case the the specified function returns the Record, use FlatMapAsync instead.
        /// </summary>
        public async Task<Record<R1, R2>> MapAsync<R1, R2>(Func<T1, T2, Task<(R1, R2)>> selector) => await (await record).MapAsync(selector).ConfigureAwait(false);
    }

    extension<T1, T2, T3>(Task<Record<T1, T2, T3>> record)
    {
        /// <summary>
        /// Structure-preserving map. Maps this Record to the new Record of 3.
        /// </summary>
        public async Task<Record<R1, R2, R3>> FlatMapAsync<R1, R2, R3>(Func<T1, T2, T3, Task<Record<R1, R2, R3>>> selector) => await (await record).FlatMapAsync(selector).ConfigureAwait(false);

        /// <summary>
        /// Structure-preserving map. Maps this Record to the new Record of 3.
        /// In case the the specified function returns the Record, use FlatMapAsync instead.
        /// </summary>
        public async Task<Record<R1, R2, R3>> MapAsync<R1, R2, R3>(Func<T1, T2, T3, Task<(R1, R2, R3)>> selector) => await (await record).MapAsync(selector).ConfigureAwait(false);
    }

    extension<T1, T2, T3, T4>(Task<Record<T1, T2, T3, T4>> record)
    {
        /// <summary>
        /// Structure-preserving map. Maps this Record to the new Record of 4.
        /// </summary>
        public async Task<Record<R1, R2, R3, R4>> FlatMapAsync<R1, R2, R3, R4>(Func<T1, T2, T3, T4, Task<Record<R1, R2, R3, R4>>> selector) => await (await record).FlatMapAsync(selector).ConfigureAwait(false);

        /// <summary>
        /// Structure-preserving map. Maps this Record to the new Record of 4.
        /// In case the the specified function returns the Record, use FlatMapAsync instead.
        /// </summary>
        public async Task<Record<R1, R2, R3, R4>> MapAsync<R1, R2, R3, R4>(Func<T1, T2, T3, T4, Task<(R1, R2, R3, R4)>> selector) => await (await record).MapAsync(selector).ConfigureAwait(false);
    }

    extension<T1, T2, T3, T4, T5>(Task<Record<T1, T2, T3, T4, T5>> record)
    {
        /// <summary>
        /// Structure-preserving map. Maps this Record to the new Record of 5.
        /// </summary>
        public async Task<Record<R1, R2, R3, R4, R5>> FlatMapAsync<R1, R2, R3, R4, R5>(Func<T1, T2, T3, T4, T5, Task<Record<R1, R2, R3, R4, R5>>> selector) => await (await record).FlatMapAsync(selector).ConfigureAwait(false);

        /// <summary>
        /// Structure-preserving map. Maps this Record to the new Record of 5.
        /// In case the the specified function returns the Record, use FlatMapAsync instead.
        /// </summary>
        public async Task<Record<R1, R2, R3, R4, R5>> MapAsync<R1, R2, R3, R4, R5>(Func<T1, T2, T3, T4, T5, Task<(R1, R2, R3, R4, R5)>> selector) => await (await record).MapAsync(selector).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a Record of 1.
    /// </summary>
    [Pure]
    public static Record<T1> ToRecord<T1>(this T1 t1) => Record.Create(t1);

    /// <summary>
    /// Creates a Record of 2.
    /// </summary>
    [Pure]
    public static Record<T1, T2> ToRecord<T1, T2>(this (T1 t1, T2 t2) tuple) => Record.Create(tuple);

    /// <summary>
    /// Creates a Record of 3.
    /// </summary>
    [Pure]
    public static Record<T1, T2, T3> ToRecord<T1, T2, T3>(this (T1 t1, T2 t2, T3 t3) tuple) => Record.Create(tuple);

    /// <summary>
    /// Creates a Record of 4.
    /// </summary>
    [Pure]
    public static Record<T1, T2, T3, T4> ToRecord<T1, T2, T3, T4>(this (T1 t1, T2 t2, T3 t3, T4 t4) tuple) => Record.Create(tuple);

    /// <summary>
    /// Creates a Record of 5.
    /// </summary>
    [Pure]
    public static Record<T1, T2, T3, T4, T5> ToRecord<T1, T2, T3, T4, T5>(this (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) tuple) => Record.Create(tuple);
}