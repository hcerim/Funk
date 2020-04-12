using System;
using System.Threading;
using System.Threading.Tasks;

namespace Funk
{
    public static partial class Prelude
    {
        public static async Task<T> result<T>(T item) => await Task.FromResult(item).ConfigureAwait(false);

        public static async Task run(Action action) => await Task.Run(action).ConfigureAwait(false);

        public static async Task run(Action action, CancellationToken token) => await Task.Run(action, token).ConfigureAwait(false);

        public static async Task<T> run<T>(Func<T> action) => await Task.Run(action).ConfigureAwait(false);

        public static async Task<T> run<T>(Func<T> action, CancellationToken token) => await Task.Run(action, token).ConfigureAwait(false);

        public static async Task run(Func<Task> action) => await Task.Run(action).ConfigureAwait(false);

        public static async Task run(Func<Task> action, CancellationToken token) => await Task.Run(action, token).ConfigureAwait(false);

        public static async Task<T> run<T>(Func<Task<T>> action) => await Task.Run(action).ConfigureAwait(false);

        public static async Task<T> run<T>(Func<Task<T>> action, CancellationToken token) => await Task.Run(action, token).ConfigureAwait(false);
    }
}
