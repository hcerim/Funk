using System.Reflection;
using System.Threading.Tasks;

namespace Funk
{
    public static class PatternExt
    {
        public static Maybe<T> Match<T>(this Pattern<T> pattern, object value) => pattern.AsMaybe().FlatMap(p => p.patterns.AsFirstOrDefault(i => value.SafeEquals(i.Item1)).FlatMap(r => value.AsMaybe().Map(v => r.Item2.Apply(v))));

        public static Task<Maybe<T>> Match<T>(this AsyncPattern<T> pattern, object value) => pattern.AsMaybe().FlatMapAsync(p => p.patterns.AsFirstOrDefault(i => value.SafeEquals(i.Item1)).FlatMapAsync(r => value.AsMaybe().MapAsync(v => r.Item2.Apply(v))));

        public static Maybe<T> Match<T>(this TypePattern<T> pattern, object value) => pattern.AsMaybe().FlatMap(p => p.patterns.AsFirstOrDefault(i => value.GetType().GetTypeInfo().IsAssignableFrom(i.Item1.GetTypeInfo())).FlatMap(r => value.AsMaybe().Map(v => r.Item2.Apply(v))));

        public static Task<Maybe<T>> Match<T>(this AsyncTypePattern<T> pattern, object value) => pattern.AsMaybe().FlatMapAsync(p => p.patterns.AsFirstOrDefault(i => value.GetType().GetTypeInfo().IsAssignableFrom(i.Item1.GetTypeInfo())).FlatMapAsync(r => value.AsMaybe().MapAsync(v => r.Item2.Apply(v))));
    }
}
