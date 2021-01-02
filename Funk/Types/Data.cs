using System;
using System.Linq.Expressions;
using System.Reflection;
using Funk.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Funk
{
    /// <summary>
    /// Provides fluent way of creating new immutable objects with updated fields/properties.
    /// </summary>
    public abstract class Data<T> where T : Data<T>
    {
        /// <summary>
        /// Creates a new object with the modified field/property specified in the expression.
        /// Uses Copy method which performs deep copy by default.
        /// </summary>
        public T With<TKey>(Expression<Func<T, TKey>> expression, TKey value)
        {
            return Copy().Map(value, expression);
        }

        /// <summary>
        /// Performs deep copy.
        /// Override if desired copy behavior differs from this one.
        /// </summary>
        public virtual T Copy() =>
            JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(this), new JsonSerializerSettings
            {
                ContractResolver = new Writable()
            });
    }
    
    internal class Writable : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            property.Readable = true;
            property.Writable = true;
            return property;
        }
    }
}