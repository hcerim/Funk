using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Funk.Internal;
using Newtonsoft.Json;
using static Funk.Prelude;

namespace Funk
{
    /// <summary>
    /// Type that provides fluent way of creating new immutable objects through the Builder type.
    /// </summary>
    [JsonObject(MemberSerialization.Fields)]
    public abstract class Data<T> where T : Data<T>
    {
        /// <summary>
        /// Returns the new Data object from the provided one.
        /// By default, takes the configuration specified in the Configure method.
        /// If the default configuration is desired, specified 'withDefaultConfiguration' parameter must be true.
        /// </summary>
        public static T From(T other) => new Builder<T>(other).Build();
    }

    /// <summary>
    /// Provides modification functions for the Data type.
    /// </summary>
    public static class Data
    {
        /// <summary>
        /// Creates a Builder object of the new Data object with the modified field/property specified in the expression.
        /// Modification takes place once the Build method is called.
        /// If the immediate modification is desired, use WithBuild instead.
        /// </summary>
        public static Builder<T> With<T>(this Data<T> item, Expression<Func<T, object>> expression, object value) where T : Data<T> =>
            new Builder<T>(
                item,
                new List<(Expression<Func<T, object>> expression, object value)>
                {
                    (expression, value)
                }
            );

        /// <summary>
        /// Creates the new Data object with the modified field/property specified in the expression.
        /// </summary>
        public static T WithBuild<T>(this Data<T> item, Expression<Func<T, object>> expression, object value) where T : Data<T> =>
            item.With(expression, value).Build();

        internal static T Copy<T>(this Data<T> data) where T : Data<T> =>
            Exc.Create(
                _ => JsonConvert.DeserializeObject<T>
                (
                    JsonConvert.SerializeObject
                    (
                        data,
                        new JsonSerializerSettings
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        }
                    )
                )
            ).Match(
                v => v,
                e => throw new SerializationException(e.Root.Map(r => r.Message).GetOr(_ => "Item cannot be serialized."))
            );
    }
    
    /// <summary>
    /// Type that provides fluent way of creating new immutable objects.
    /// Contains underlying modification expressions.
    /// </summary>
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
        internal readonly List<(Expression<Func<T, object>> expression, object value)> Expressions =
            new List<(Expression<Func<T, object>>, object)>();

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
        /// <summary>
        /// Creates a Builder object of the new Data object with the modified field/property specified in the expression.
        /// Modification takes place once the Build method is called.
        /// If the immediate modification is desired, use WithBuild instead.
        /// </summary>
        public static Builder<T> With<T>(this Builder<T> builder, Expression<Func<T, object>> expression, object value) where T : Data<T> =>
            builder.With(expression, value);

        /// <summary>
        /// Creates the new Data object with the modified field/property specified in the expression.
        /// </summary>
        public static T WithBuild<T>(this Builder<T> builder, Expression<Func<T, object>> expression, object value) where T : Data<T> =>
            builder.With(expression, value).Build();

        /// <summary>
        /// Creates the new Data object with modified field/properties specified in the underlying expressions.
        /// </summary>
        public static T Build<T>(this Builder<T> builder) where T : Data<T> =>
            builder.Expressions.Aggregate(builder.Item.Copy(),
                (item, expressions) => item.Map(expressions.expression, expressions.value)
            );
    }
}