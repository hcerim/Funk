using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Funk.Internal;
using static Funk.Prelude;

namespace Funk;

/// <summary>
/// Type that provides fluent way of creating new immutable objects through the Builder type.
/// </summary>
/// <typeparam name="T">The concrete type that extends Data.</typeparam>
public abstract class Data<T> where T : Data<T>
{
    /// <summary>
    /// Returns the new Data object from the provided one.
    /// By default, takes the configuration specified in the Configure method.
    /// If the default configuration is desired, specified 'withDefaultConfiguration' parameter must be true.
    /// </summary>
    /// <param name="other">The source object to copy from.</param>
    /// <returns>A new copy of the object.</returns>
    public static T From(T other) => new Builder<T>(other).Build();
}

/// <summary>
/// Provides modification functions for the Data type.
/// </summary>
public static class Data
{
    extension<T>(Data<T> item) where T : Data<T>
    {
        /// <summary>
        /// Creates a Builder object of the new Data object with the modified field/property specified in the expression.
        /// Modification takes place once the Build method is called.
        /// If the immediate modification is desired, use WithBuild instead.
        /// </summary>
        /// <typeparam name="TKey">The type of the property or field.</typeparam>
        /// <param name="expression">An expression identifying the property or field to modify.</param>
        /// <param name="value">The new value.</param>
        /// <returns>A Builder with the pending modification.</returns>
        public Builder<T> With<TKey>(Expression<Func<T, TKey>> expression, TKey value) =>
            new Builder<T>(
                item,
                new List<(Expression<Func<T, object>> expression, object value)>
                {
                    (Expression.Lambda<Func<T, object>>(Expression.Convert(expression.Body, typeof(object)), expression.Parameters), value)
                }
            );

        /// <summary>
        /// Creates the new Data object with the modified field/property specified in the expression.
        /// </summary>
        /// <typeparam name="TKey">The type of the property or field.</typeparam>
        /// <param name="expression">An expression identifying the property or field to modify.</param>
        /// <param name="value">The new value.</param>
        /// <returns>A new object with the modification applied.</returns>
        public T WithBuild<TKey>(Expression<Func<T, TKey>> expression, TKey value) =>
            item.With(expression, value).Build();

        internal T Copy() =>
            Exc.Create(
                _ => (T)DeepCopy(item, new Dictionary<object, object>(ReferenceComparer.Instance))
            ).Match(
                v => v,
                e => throw new Funk.SerializationException(e.Root.Map(r => r.Message).GetOr(_ => "Item cannot be copied."))
            );
    }

    private static object DeepCopy(object source, Dictionary<object, object> visited)
    {
        if (source.IsNull()) return null;
        var type = source.GetType();
        if (type.IsPrimitive ||
            type.IsEnum ||
            type == typeof(string) ||
            type.IsValueType ||
            typeof(Delegate).IsAssignableFrom(type)
           ) return source;
        if (visited.TryGetValue(source, out var existing)) return existing;
        if (type.IsArray)
        {
            var sourceArray = (Array)source;
            var copy = Array.CreateInstance(type.GetElementType(), sourceArray.Length);
            visited[source] = copy;
            for (var i = 0; i < sourceArray.Length; i++)
            {
                copy.SetValue(DeepCopy(sourceArray.GetValue(i), visited), i);
            }
            return copy;
        }
#if NETSTANDARD2_0
        var target = FormatterServices.GetUninitializedObject(type);
#else
        var target = RuntimeHelpers.GetUninitializedObject(type);
#endif
        visited[source] = target;
        var currentType = type;
        while (currentType.IsNotNull())
        {
            foreach (var field in currentType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
            {
                field.SetValue(target, DeepCopy(field.GetValue(source), visited));
            }
            currentType = currentType.BaseType;
        }

        return target;
    }

    private sealed class ReferenceComparer : IEqualityComparer<object>
    {
        internal static readonly ReferenceComparer Instance = new ReferenceComparer();
        public new bool Equals(object x, object y) => ReferenceEquals(x, y);
        public int GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
    }
}
    
/// <summary>
/// Type that provides fluent way of creating new immutable objects.
/// Contains underlying modification expressions.
/// </summary>
/// <typeparam name="T">The concrete type that extends Data.</typeparam>
public sealed class Builder<T> where T : Data<T>
{
    internal Builder(Data<T> item)
    {
        Item = item;
    }
        
    internal Builder(Data<T> item, IEnumerable<(Expression<Func<T, object>> expression, object value)> expressions)
    {
        Item = item;
        Expressions.AddRange(expressions.Map());
    }

    internal Builder<T> With(Expression<Func<T, object>> expression, object value) =>
        new Builder<T>(Item, Expressions.Concat(list((expression, value))));

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal Data<T> Item { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal readonly List<(Expression<Func<T, object>> expression, object value)> Expressions = [];

    /// <summary>
    /// Lifts the Data to the Builder. 
    /// </summary>
    public static implicit operator Builder<T>(Data<T> data) =>
        new Builder<T>(data, list<(Expression<Func<T, object>>, object)>());
}
    
/// <summary>
/// Provides modification functions for the Builder type.
/// </summary>
public static class Builder
{
    extension<T>(Builder<T> builder) where T : Data<T>
    {
        /// <summary>
        /// Creates a Builder object of the new Data object with the modified field/property specified in the expression.
        /// Modification takes place once the Build method is called.
        /// If the immediate modification is desired, use WithBuild instead.
        /// </summary>
        /// <typeparam name="TKey">The type of the property or field.</typeparam>
        /// <param name="expression">An expression identifying the property or field to modify.</param>
        /// <param name="value">The new value.</param>
        /// <returns>A Builder with the pending modification.</returns>
        public Builder<T> With<TKey>(Expression<Func<T, TKey>> expression, TKey value) =>
            builder.With(Expression.Lambda<Func<T, object>>(Expression.Convert(expression.Body, typeof(object)), expression.Parameters), value);

        /// <summary>
        /// Creates the new Data object with the modified field/property specified in the expression.
        /// </summary>
        /// <typeparam name="TKey">The type of the property or field.</typeparam>
        /// <param name="expression">An expression identifying the property or field to modify.</param>
        /// <param name="value">The new value.</param>
        /// <returns>A new object with the modification applied.</returns>
        public T WithBuild<TKey>(Expression<Func<T, TKey>> expression, TKey value) =>
            builder.With(expression, value).Build();

        /// <summary>
        /// Creates the new Data object with modified field/properties specified in the underlying expressions.
        /// </summary>
        /// <returns>A new object with all pending modifications applied.</returns>
        public T Build() =>
            builder.Expressions.Aggregate(builder.Item.Copy(),
                (item, expressions) => item.Map(expressions.expression, expressions.value)
            );
    }
}