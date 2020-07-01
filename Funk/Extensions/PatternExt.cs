using System.Reflection;
using System.Threading.Tasks;

namespace Funk
{
    public static class PatternExt
    {
        public static Task<Maybe<T>> Apply<T>(this AsyncPattern<T> pattern, object value)
        {
            return pattern.Patterns.FlatMap(t => t.AsFirstOrDefault(i => i.Item1.SafeEquals(value))).FlatMapAsync(r => value.AsMaybe().MapAsync(v => r.Item2.Apply(v)));
        }

        public static Maybe<T> Apply<T>(this Pattern<T> pattern, object value)
        {
            return pattern.Patterns.FlatMap(t => t.AsFirstOrDefault(i => i.Item1.SafeEquals(value))).FlatMap(r => value.AsMaybe().Map(v => r.Item2.Apply(v)));
        }

        public static Maybe<T> Apply<T>(this TypePattern<T> pattern, object value) => pattern.AsMaybe().FlatMap(p => p.patterns.AsFirstOrDefault(i => value.GetType().GetTypeInfo().IsAssignableFrom(i.Item1.GetTypeInfo())).FlatMap(r => value.AsMaybe().Map(v => r.Item2.Apply(v))));

        public static Task<Maybe<T>> Apply<T>(this AsyncTypePattern<T> pattern, object value) => pattern.AsMaybe().FlatMapAsync(p => p.patterns.AsFirstOrDefault(i => value.GetType().GetTypeInfo().IsAssignableFrom(i.Item1.GetTypeInfo())).FlatMapAsync(r => value.AsMaybe().MapAsync(v => r.Item2.Apply(v))));
    }
}
