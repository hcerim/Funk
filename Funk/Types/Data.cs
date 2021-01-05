using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Funk.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using static Funk.Prelude;

namespace Funk
{
    /// <summary>
    /// Provides fluent way of creating new immutable objects with updated fields/properties using builders.
    /// </summary>
    public abstract class Data<T> where T : Data<T>
    {
        /// <summary>
        /// Performs deep copy based on Newtonsoft.JSON serialization/deserialization.
        /// Override if desired copy behavior differs from this one.
        /// </summary>
        public virtual T Copy() =>
            Exc.Create(_ => JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(this), new JsonSerializerSettings
                    { 
                ContractResolver = new Writable()
            })).Match(
                v => v,
                e => throw new SerializationException(e.Root.Map(r => r.Message).GetOr(_ => "Item cannot be serialized."))
            );
    }

    public static class Data
    {
        /// <summary>
        /// Creates a Builder object for the specified Data type.
        /// </summary>
        public static Builder<T> With<T, TKey>(this Data<T> item, Expression<Func<T, TKey>> expression, TKey value) where T : Data<T> =>
            new Builder<T>(
                item,
                new List<(Expression<Func<T, object>> expression, object value)>
                {
                    (Expression.Lambda<Func<T, object>>(Expression.Convert(expression.Body, typeof(object)), expression.Parameters), value)
                }
            );
        
        /// <summary>
        /// Creates a Builder object for the specified Data type.
        /// </summary>
        public static Builder<T> With<T>(this Data<T> item, params (Expression<Func<T, object>> expression, object value)[] expressions) where T : Data<T> =>
            new Builder<T>(
                item,
                expressions
            );
        
        /// <summary>
        /// Creates a new object with the modified field/property specified in the expression.
        /// </summary>
        public static T WithBuild<T, TKey>(this Data<T> item, Expression<Func<T, TKey>> expression, TKey value) where T : Data<T> =>
            item.With(expression, value).Build();
        
        /// <summary>
        /// Creates a Builder object for the specified Data type.
        /// </summary>
        public static T WithBuild<T>(this Data<T> item, params (Expression<Func<T, object>> expression, object value)[] expressions) where T : Data<T> =>
            item.With(expressions).Build();

        /// <summary>
        /// Creates a Builder object from the provided one for the specified Data type.
        /// </summary>
        public static Builder<T> With<T, TKey>(this Builder<T> builder, Expression<Func<T, TKey>> expression, TKey value) where T : Data<T> =>
            builder.With(Expression.Lambda<Func<T, object>>(Expression.Convert(expression.Body, typeof(object)), expression.Parameters), value);
        
        /// <summary>
        /// Creates a Builder object from the provided one for the specified Data type.
        /// </summary>
        public static Builder<T> With<T>(this Builder<T> builder, params (Expression<Func<T, object>> expression, object value)[] expressions) where T : Data<T> =>
            builder.With(expressions);
        
        /// <summary>
        /// Creates a new object with the modified field/property specified in the expression.
        /// </summary>
        public static T WithBuild<T, TKey>(this Builder<T> builder, Expression<Func<T, TKey>> expression, TKey value) where T : Data<T> =>
            builder.With(expression, value).Build();
        
        /// <summary>
        /// Creates a Builder object from the provided one for the specified Data type.
        /// </summary>
        public static T WithBuild<T>(this Builder<T> builder, params (Expression<Func<T, object>> expression, object value)[] expressions) where T : Data<T> =>
            builder.With(expressions).Build();

        /// <summary>
        /// Creates a new Data object from the specified Builder.
        /// </summary>
        public static T Build<T>(this Builder<T> builder) where T : Data<T> =>
            builder.Expressions.Aggregate(builder.Item.Copy(), (item, expressions) => item.Map(expressions.expression, expressions.value));
    }

    public sealed class Builder<T> where T : Data<T>
    {
        internal Builder(Data<T> item, IEnumerable<(Expression<Func<T, object>> expression, object value)> expressions)
        {
            Item = item;
            Expressions.AddRange(expressions.Map());
        }

        internal Builder<T> With(Expression<Func<T, object>> expression, object value) =>
            new Builder<T>(Item, Expressions.Concat(list((expression, value))));
        
        internal Builder<T> With(IEnumerable<(Expression<Func<T, object>> expression, object value)> expressions) =>
            new Builder<T>(Item, Expressions.Concat(expressions));
        
        internal Data<T> Item { get; }

        internal readonly List<(Expression<Func<T, object>> expression, object value)> Expressions =
            new List<(Expression<Func<T, object>> expression, object value)>();

        public static implicit operator Builder<T>(Data<T> data) =>
            new Builder<T>(data, list<(Expression<Func<T, object>>, object)>());
    }
    
    internal class Writable : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            return new TypePattern<JsonProperty>
            {
                (PropertyInfo p) => p.GetSetMethod(true).AsMaybe().Map(_ =>
                {
                    property.Readable = true;
                    property.Writable = true;
                    return property;
                }).GetOr(_ => property),
                (FieldInfo f) =>
                {
                    property.Readable = true;
                    property.Writable = true;
                    return property;
                }
            }.Match(member).UnsafeGet(_ => new InvalidOperationException("Type member must be either a property or a field."));
        }
    }
}