using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Funk.Internal;
using Newtonsoft.Json;
using static Funk.Prelude;

namespace Funk
{
    /// <summary>
    /// Provides fluent way of creating new immutable objects with updated fields/properties using builders.
    /// </summary>
    [JsonObject(MemberSerialization.Fields)]
    public abstract class Data<T> where T : Data<T>
    {
        protected Data()
        {
            Configure();
            Update();
        }

        internal bool WithDefaultBehavior => defaultConfigurationActivated.Or(_ => Exclusions.IsEmpty());

        private bool defaultConfigurationActivated;

        /// <summary>
        /// Override when desired exclusions are intended.
        /// Exclusions can be specified only in this method.
        /// </summary>
        protected virtual void Configure()
        {
        }

        /// <summary>
        /// Returns new Data object from the provided one.
        /// </summary>
        public static T From(T other, bool withDefaultConfiguration = false) => other.WithBuild().WithConfiguration(withDefaultConfiguration);

        /// <summary>
        /// Specify true if the default configuration is desired.
        /// </summary>
        public T WithConfiguration(bool withDefaultConfiguration = false)
        {
            defaultConfigurationActivated = withDefaultConfiguration;
            return (T)this;
        }
        
        /// <summary>
        /// Excludes specified field/property.
        /// Specify exclusions in Configure method.
        /// </summary>
        protected void Exclude<TKey>(Expression<Func<T, TKey>> expression) =>
            exclusions.Add((Expression.Lambda<Func<T, object>>(Expression.Convert(expression.Body, typeof(object)), expression.Parameters), default(TKey)));

        private void Update() => Exclusions = Exclusions.Concat(exclusions).DistinctBy(e => e.ToString()).ToList();

        [JsonIgnore]
        internal List<(Expression<Func<T, object>> expression, object value)> Exclusions =
            new List<(Expression<Func<T, object>> expression, object value)>();
        
        [JsonIgnore]
        private readonly List<(Expression<Func<T, object>> expression, object value)> exclusions =
            new List<(Expression<Func<T, object>> expression, object value)>();
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
            var reduced = builder.Expressions.Aggregate(builder.Item.Copy(),
                (item, expressions) => item.Map(expressions.expression, expressions.value)
            );
            builder.Item.WithDefaultBehavior.Match(
                _ => builder.Item.Exclusions.ForEach(e => Exc.Create(__ => reduced.Map(e.expression, e.value)))
            );
            builder.Expressions.IsEmpty().And(_ => builder.Item.WithDefaultBehavior).Match(
                _ =>
                {
                    reduced = reduced.Copy();
                }
            );
            reduced.Exclusions = builder.Item.Exclusions;
            return reduced;
        }

        private static T Copy<T>(this Data<T> data) where T : Data<T> =>
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
}