using System;
using System.Collections.Concurrent;
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
        public T Copy() =>
            Exc.Create(_ => JsonConvert.DeserializeObject<T>
            (
                JsonConvert.SerializeObject(this, new JsonSerializerSettings 
                {
                    ContractResolver = new NonPublic()
                }),
                new JsonSerializerSettings
                {
                    ContractResolver = new Writable()
                })).Match(
                v => v,
                e => throw new SerializationException(e.Root.Map(r => r.Message)
                    .GetOr(_ => "Item cannot be serialized."))
            );

        protected void Include(Expression<Func<T, object>> expression) => Include(expression.ToImmutableList());

        protected void Include(params Expression<Func<T, object>>[] expressions) => Include(expressions.Map());
        
        protected void Exclude(Expression<Func<T, object>> expression) => Exclude(expression.ToImmutableList());

        protected void Exclude(params Expression<Func<T, object>>[] expressions) => Exclude(expressions.Map());

        private static void Include(IEnumerable<Expression<Func<T, object>>> expressions)
        {
            foreach (var e in expressions)
            {
                Data.Inclusions.Add((typeof(T), e.GetMemberName()));
            }
        }
        
        private static void Exclude(IEnumerable<Expression<Func<T, object>>> expressions)
        {
            foreach (var e in expressions)
            {
                Data.Exclusions.Add((typeof(T), e.GetMemberName()));
            }
        }
    }

    public static class Data
    {
        /// <summary>
        /// Creates a Builder object for the specified Data type.
        /// </summary>
        public static Builder<T> With<T, TKey>(this Data<T> item, Expression<Func<T, object>> expression, TKey value) where T : Data<T> =>
            new Builder<T>(
                item,
                new List<(Expression<Func<T, object>> expression, object value)>
                {
                    (expression, value)
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
        public static T WithBuild<T, TKey>(this Data<T> item, Expression<Func<T, object>> expression, TKey value) where T : Data<T> =>
            item.With(expression, value).Build();
        
        /// <summary>
        /// Creates a Builder object for the specified Data type.
        /// </summary>
        public static T WithBuild<T>(this Data<T> item, params (Expression<Func<T, object>> expression, object value)[] expressions) where T : Data<T> =>
            item.With(expressions).Build();

        /// <summary>
        /// Creates a Builder object from the provided one for the specified Data type.
        /// </summary>
        public static Builder<T> With<T, TKey>(this Builder<T> builder, Expression<Func<T, object>> expression, TKey value) where T : Data<T> =>
            builder.With(expression, value);
        
        /// <summary>
        /// Creates a Builder object from the provided one for the specified Data type.
        /// </summary>
        public static Builder<T> With<T>(this Builder<T> builder, params (Expression<Func<T, object>> expression, object value)[] expressions) where T : Data<T> =>
            builder.With(expressions);
        
        /// <summary>
        /// Creates a new object with the modified field/property specified in the expression.
        /// </summary>
        public static T WithBuild<T, TKey>(this Builder<T> builder, Expression<Func<T, object>> expression, TKey value) where T : Data<T> =>
            builder.With(expression, value).Build();
        
        /// <summary>
        /// Creates a Builder object from the provided one for the specified Data type.
        /// </summary>
        public static T WithBuild<T>(this Builder<T> builder, params (Expression<Func<T, object>> expression, object value)[] expressions) where T : Data<T> =>
            builder.With(expressions).Build();

        /// <summary>
        /// Creates a new Data object from the specified Builder.
        /// </summary>
        public static T Build<T>(this Builder<T> builder) where T : Data<T>
        {
            var aggregate = builder.Expressions.Aggregate(builder.Item.Copy(),
                (item, expressions) => item.Map(expressions.expression, expressions.value));
            return aggregate.Copy();
        }

        internal static readonly ConcurrentBag<(Type type, string member)> Inclusions =
            new ConcurrentBag<(Type, string)>();
        
        internal static readonly ConcurrentBag<(Type type, string member)> Exclusions =
            new ConcurrentBag<(Type, string)>();
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
            new List<(Expression<Func<T, object>>, object)>();

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
        
        protected override List<MemberInfo> GetSerializableMembers(Type type)
        {
            var result = base.GetSerializableMembers(type);
            var members = Data.Inclusions.Distinct().Where(i => i.type.SafeEquals(type)).Select(i =>
                type.GetMember(i.member, BindingFlags.NonPublic | BindingFlags.Instance).Single()
            );
            result.AddRange(members);
            return result;
        }
        
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);
            var inclusions = Data.Inclusions.Where(i => i.type.SafeEquals(type)).Select(i => i.member).Distinct();
            var exclusions = Data.Exclusions.Where(i => i.type.SafeEquals(type)).Select(i => i.member).Distinct();
            var included = properties.Where(p => inclusions.Contains(p.PropertyName));
            var excluded = properties.Where(p => exclusions.Contains(p.PropertyName));
            foreach (var p in included)
            {
                p.Readable = true;
            }
            foreach (var p in excluded)
            {
                p.Readable = false;
            }
            return properties;
        }
    }

    internal class NonPublic : DefaultContractResolver
    {
        protected override List<MemberInfo> GetSerializableMembers(Type type)
        {
            var result = base.GetSerializableMembers(type);
            var members = Data.Inclusions.Distinct().Where(i => i.type.SafeEquals(type)).Select(i =>
                type.GetMember(i.member, BindingFlags.NonPublic | BindingFlags.Instance).Single()
            );
            result.AddRange(members);
            return result;
        }
        
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);
            var inclusions = Data.Inclusions.Where(i => i.type.SafeEquals(type)).Select(i => i.member).Distinct();
            var exclusions = Data.Exclusions.Where(i => i.type.SafeEquals(type)).Select(i => i.member).Distinct();
            var included = properties.Where(p => inclusions.Contains(p.PropertyName));
            var excluded = properties.Where(p => exclusions.Contains(p.PropertyName));
            foreach (var p in included)
            {
                p.Readable = true;
            }
            foreach (var p in excluded)
            {
                p.Readable = false;
            }
            return properties;
        }
    }
}