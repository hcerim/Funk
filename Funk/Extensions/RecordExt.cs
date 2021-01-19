using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace Funk
{
    public static class RecordExt
    {
        /// <summary>
        /// Structure-preserving map. Maps corresponding record items to the new Record of 1.
        /// </summary>
        public static async Task<Record<R1>> FlatMapAsync<T1, R1>(this Task<Record<T1>> record, Func<T1, Task<Record<R1>>> selector) => await (await record).FlatMapAsync(selector).ConfigureAwait(false);

        /// <summary>
        /// Structure-preserving map. Maps corresponding record items to the new Record of 1.
        /// </summary>
        public static async Task<Record<R1>> MapAsync<T1, R1>(this Task<Record<T1>> record, Func<T1, Task<R1>> selector) => await (await record).MapAsync(selector).ConfigureAwait(false);
        
        /// <summary>
        /// Structure-preserving map. Maps corresponding record items to the new Record of 2.
        /// </summary>
        public static async Task<Record<R1, R2>> FlatMapAsync<T1, T2, R1, R2>(this Task<Record<T1, T2>> record, Func<T1, T2, Task<Record<R1, R2>>> selector) => await (await record).FlatMapAsync(selector).ConfigureAwait(false);

        /// <summary>
        /// Structure-preserving map. Maps corresponding record items to the new Record of 2.
        /// </summary>
        public static async Task<Record<R1, R2>> MapAsync<T1, T2, R1, R2>(this Task<Record<T1, T2>> record, Func<T1, T2, Task<(R1, R2)>> selector) => await (await record).MapAsync(selector).ConfigureAwait(false);
        
        /// <summary>
        /// Structure-preserving map. Maps corresponding record items to the new Record of 3.
        /// </summary>
        public static async Task<Record<R1, R2, R3>> FlatMapAsync<T1, T2, T3, R1, R2, R3>(this Task<Record<T1, T2, T3>> record, Func<T1, T2, T3, Task<Record<R1, R2, R3>>> selector) => await (await record).FlatMapAsync(selector).ConfigureAwait(false);

        /// <summary>
        /// Structure-preserving map. Maps corresponding record items to the new Record of 3.
        /// </summary>
        public static async Task<Record<R1, R2, R3>> MapAsync<T1, T2, T3, R1, R2, R3>(this Task<Record<T1, T2, T3>> record, Func<T1, T2, T3, Task<(R1, R2, R3)>> selector) => await (await record).MapAsync(selector).ConfigureAwait(false);
        
        /// <summary>
        /// Structure-preserving map. Maps corresponding record items to the new Record of 4.
        /// </summary>
        public static async Task<Record<R1, R2, R3, R4>> FlatMapAsync<T1, T2, T3, T4, R1, R2, R3, R4>(this Task<Record<T1, T2, T3, T4>> record, Func<T1, T2, T3, T4, Task<Record<R1, R2, R3, R4>>> selector) => await (await record).FlatMapAsync(selector).ConfigureAwait(false);

        /// <summary>
        /// Structure-preserving map. Maps corresponding record items to the new Record of 4.
        /// </summary>
        public static async Task<Record<R1, R2, R3, R4>> MapAsync<T1, T2, T3, T4, R1, R2, R3, R4>(this Task<Record<T1, T2, T3, T4>> record, Func<T1, T2, T3, T4, Task<(R1, R2, R3, R4)>> selector) => await (await record).MapAsync(selector).ConfigureAwait(false);
        
        /// <summary>
        /// Structure-preserving map. Maps corresponding record items to the new Record of 5.
        /// </summary>
        public static async Task<Record<R1, R2, R3, R4, R5>> FlatMapAsync<T1, T2, T3, T4, T5, R1, R2, R3, R4, R5>(this Task<Record<T1, T2, T3, T4, T5>> record, Func<T1, T2, T3, T4, T5, Task<Record<R1, R2, R3, R4, R5>>> selector) => await (await record).FlatMapAsync(selector).ConfigureAwait(false);

        /// <summary>
        /// Structure-preserving map. Maps corresponding record items to the new Record of 5.
        /// </summary>
        public static async Task<Record<R1, R2, R3, R4, R5>> MapAsync<T1, T2, T3, T4, T5, R1, R2, R3, R4, R5>(this Task<Record<T1, T2, T3, T4, T5>> record, Func<T1, T2, T3, T4, T5, Task<(R1, R2, R3, R4, R5)>> selector) => await (await record).MapAsync(selector).ConfigureAwait(false);
        
        /// <summary>
        /// Creates a record of 1.
        /// </summary>
        [Pure]
        public static Record<T1> ToRecord<T1>(this T1 t1) => Record.Create(t1);

        /// <summary>
        /// Creates a record of 2.
        /// </summary>
        [Pure]
        public static Record<T1, T2> ToRecord<T1, T2>(this (T1 t1, T2 t2) tuple) => Record.Create(tuple);

        /// <summary>
        /// Creates a record of 3.
        /// </summary>
        [Pure]
        public static Record<T1, T2, T3> ToRecord<T1, T2, T3>(this (T1 t1, T2 t2, T3 t3) tuple) => Record.Create(tuple);

        /// <summary>
        /// Creates a record of 4.
        /// </summary>
        [Pure]
        public static Record<T1, T2, T3, T4> ToRecord<T1, T2, T3, T4>(this (T1 t1, T2 t2, T3 t3, T4 t4) tuple) => Record.Create(tuple);

        /// <summary>
        /// Creates a record of 5.
        /// </summary>
        [Pure]
        public static Record<T1, T2, T3, T4, T5> ToRecord<T1, T2, T3, T4, T5>(this (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) tuple) => Record.Create(tuple);
    }
}
