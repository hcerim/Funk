using System;
using System.Threading;
using System.Threading.Tasks;

namespace Funk
{
    public static partial class Prelude
    {
        public static Task<T> result<T>(T item) => Task.FromResult(item);

        public static Task run(Action action) => Task.Run(action);

        public static Task run(Action action, CancellationToken token) => Task.Run(action, token);

        public static Task<T> run<T>(Func<T> action) => Task.Run(action);

        public static Task<T> run<T>(Func<T> action, CancellationToken token) => Task.Run(action, token);

        public static Task run(Func<Task> action) => Task.Run(action);

        public static Task run(Func<Task> action, CancellationToken token) => Task.Run(action, token);

        public static Task<T> run<T>(Func<Task<T>> action) => Task.Run(action);

        public static Task<T> run<T>(Func<Task<T>> action, CancellationToken token) => Task.Run(action, token);
    }
}
