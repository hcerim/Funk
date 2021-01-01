using System;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Funk.Internal
{
    internal static class InternalExt
    {
        [Pure] 
        internal static UnhandledValueException UnhandledException => new UnhandledValueException("Expression did not cover all possible cases.");

        internal static R Otherwise<R>(Func<Unit, R> otherwise, Func<Unit, Exception> otherwiseThrow)
        {
            return otherwise.AsMaybe().Match(
                _ =>
                {
                    if (otherwiseThrow.IsNotNull())
                    {
                        throw otherwiseThrow(Unit.Value);
                    }
                    throw UnhandledException;
                },
                o => o(Unit.Value)
            );
        }

        internal static void Execute(this Action<Unit> otherwise)
        {
            otherwise.AsMaybe().Match(
                _ => throw UnhandledException,
                e => e(Unit.Value)
            );
        }

        internal static Exc<T, E> TryCatch<T, E>(this Func<Unit, T> operation) where E : Exception
        {
            try
            {
                return new Exc<T, E>(operation(Unit.Value));
            }
            catch (E e)
            {
                return new Exc<T, E>(e);
            }
        }

        internal static async Task<Exc<T, E>> TryCatchAsync<T, E>(this Func<Unit, Task<T>> operation) where E : Exception
        {
            try
            {
                return new Exc<T, E>(await operation(Unit.Value).ConfigureAwait(false));
            }
            catch (E e)
            {
                return new Exc<T, E>(e);
            }
        }

        internal static IImmutableList<T> GetOrEmpty<T>(this Maybe<IImmutableList<T>> maybe) => maybe.GetOr(_ => ImmutableList<T>.Empty.Map());
        
        internal static T Map<T, TKey>(this T data, TKey value, Expression<Func<T, TKey>> expression)
        {
            new TypePattern<Unit>
            {
                (PropertyInfo p) =>
                {
                    p.SetValue(data, Convert.ChangeType(value, p.PropertyType), null);
                    return Unit.Value;
                },
                (FieldInfo f) =>
                {
                    f.SetValue(data, Convert.ChangeType(value, f.FieldType));
                    return Unit.Value;
                }
            }.Match(expression.GetMemberInfo()).UnsafeGet(_ =>
                new InvalidOperationException("Type member must be either a property or a field.")
            );

            return data;
        }

        private static MemberInfo GetMemberInfo<T, TKey>(this Expression<Func<T, TKey>> expression)
        {
            return expression == null ? throw new ArgumentNullException(nameof(expression)) :
                expression.Body is UnaryExpression u && u.Operand is MemberExpression m1 ? m1.Member :
                expression.Body is MemberExpression m2 ? m2.Member :
                throw new ArgumentException("The expression does not indicate a valid property or a field.");
        }
    }
}
