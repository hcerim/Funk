using System;
using System.Threading;
using System.Threading.Tasks;

namespace Funk;

public static partial class Prelude
{
    /// <summary>
    /// Creates a completed Task with the specified result.
    /// </summary>
    public static Task<T> result<T>(T item) => Task.FromResult(item);

    /// <summary>
    /// Queues the specified work to run on the thread pool.
    /// </summary>
    public static Task run(Action action) => Task.Run(action);

    /// <summary>
    /// Queues the specified work to run on the thread pool with cancellation support.
    /// </summary>
    public static Task run(Action action, CancellationToken token) => Task.Run(action, token);

    /// <summary>
    /// Queues the specified work to run on the thread pool and returns its result.
    /// </summary>
    public static Task<T> run<T>(Func<T> action) => Task.Run(action);

    /// <summary>
    /// Queues the specified work to run on the thread pool and returns its result with cancellation support.
    /// </summary>
    public static Task<T> run<T>(Func<T> action, CancellationToken token) => Task.Run(action, token);

    /// <summary>
    /// Queues the specified asynchronous work to run on the thread pool.
    /// </summary>
    public static Task run(Func<Task> action) => Task.Run(action);

    /// <summary>
    /// Queues the specified asynchronous work to run on the thread pool with cancellation support.
    /// </summary>
    public static Task run(Func<Task> action, CancellationToken token) => Task.Run(action, token);

    /// <summary>
    /// Queues the specified asynchronous work to run on the thread pool and returns its result.
    /// </summary>
    public static Task<T> run<T>(Func<Task<T>> action) => Task.Run(action);

    /// <summary>
    /// Queues the specified asynchronous work to run on the thread pool and returns its result with cancellation support.
    /// </summary>
    public static Task<T> run<T>(Func<Task<T>> action, CancellationToken token) => Task.Run(action, token);
}