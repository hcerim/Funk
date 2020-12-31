using System;
using System.Linq.Expressions;
using Funk.Internal;

namespace Funk
{
    /// <summary>
    /// Provides fluent way of creating new immutable objects with updated fields/properties.
    /// </summary>
    public abstract class Data<T> where T : Data<T>
    {
        /// <summary>
        /// Creates a new object with the modified field/property specified in the expression.
        /// Uses Copy method which performs shallow copy by default.
        /// </summary>
        public T With<TKey>(Expression<Func<T, TKey>> expression, TKey value)
        {
            return Copy().Map(value, expression);
        }
        
        /// <summary>
        /// Performs shallow copy.
        /// For deep copy, override the method with desired implementation.
        /// </summary>
        public virtual T Copy() => (T) MemberwiseClone();
    }
}