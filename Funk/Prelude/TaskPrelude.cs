using System;
using System.Threading;
using System.Threading.Tasks;

namespace Funk;

public static partial class Prelude
{
    /// <summary>
    /// Creates a completed Task with the specified result.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="item">The value.</param>
    /// <returns>A completed Task containing the value.</returns>
    public static Task<T> result<T>(T item) => Task.FromResult(item);

    /// <summary>
    /// Queues the specified work to run on the thread pool.
    /// </summary>
    /// <param name="action">The work to execute.</param>
    /// <returns>A Task representing the operation.</returns>
    public static Task run(Action action) => Task.Run(action);

    /// <summary>
    /// Queues the specified work to run on the thread pool with cancellation support.
    /// </summary>
    /// <param name="action">The work to execute.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>A Task representing the operation.</returns>
    public static Task run(Action action, CancellationToken token) => Task.Run(action, token);

    /// <summary>
    /// Queues the specified work to run on the thread pool and returns its result.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="action">The work to execute.</param>
    /// <returns>A Task representing the operation.</returns>
    public static Task<T> run<T>(Func<T> action) => Task.Run(action);

    /// <summary>
    /// Queues the specified work to run on the thread pool and returns its result with cancellation support.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="action">The work to execute.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>A Task representing the operation.</returns>
    public static Task<T> run<T>(Func<T> action, CancellationToken token) => Task.Run(action, token);

    /// <summary>
    /// Queues the specified asynchronous work to run on the thread pool.
    /// </summary>
    /// <param name="action">The work to execute.</param>
    /// <returns>A Task representing the operation.</returns>
    public static Task run(Func<Task> action) => Task.Run(action);

    /// <summary>
    /// Queues the specified asynchronous work to run on the thread pool with cancellation support.
    /// </summary>
    /// <param name="action">The work to execute.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>A Task representing the operation.</returns>
    public static Task run(Func<Task> action, CancellationToken token) => Task.Run(action, token);

    /// <summary>
    /// Queues the specified asynchronous work to run on the thread pool and returns its result.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="action">The work to execute.</param>
    /// <returns>A Task representing the operation.</returns>
    public static Task<T> run<T>(Func<Task<T>> action) => Task.Run(action);

    /// <summary>
    /// Queues the specified asynchronous work to run on the thread pool and returns its result with cancellation support.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="action">The work to execute.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>A Task representing the operation.</returns>
    public static Task<T> run<T>(Func<Task<T>> action, CancellationToken token) => Task.Run(action, token);
}