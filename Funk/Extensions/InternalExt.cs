using System;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Funk.Internal
{
    internal static class InternalExt
    {
        private static UnhandledValueException UnhandledException => new UnhandledValueException("Expression did not cover all possible cases.");

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
            var memberExpression = expression.GetMemberExpression();
            new TypePattern<Unit>
            {
                (PropertyInfo p) =>
                {
                    var target = GetTarget(ImmutableList<(MemberTypes, string)>.Empty, memberExpression);
                    var nested = target.Children.Take(target.Children.Count - 1).AsNotEmptyList();
                    return nested.Match(
                        _ =>
                        {
                            p.SetValue(data, Convert.ChangeType(value, p.PropertyType), null);
                            return Unit.Value;
                        },
                        l =>
                        {
                            var aggregate = data.Reduce(target.Parent, l);
                            p.SetValue(aggregate.Item2, Convert.ChangeType(value, p.PropertyType), null);
                            return Unit.Value;
                        }
                    );
                },
                (FieldInfo f) =>
                {
                    var target = GetTarget(ImmutableList<(MemberTypes, string)>.Empty, memberExpression);
                    var nested = target.Children.Take(target.Children.Count - 1).AsNotEmptyList();
                    return nested.Match(
                        _ =>
                        {
                            f.SetValue(data, Convert.ChangeType(value, f.FieldType));
                            return Unit.Value;
                        },
                        l =>
                        {
                            var aggregate = data.Reduce(target.Parent, l);
                            f.SetValue(aggregate.Item2, Convert.ChangeType(value, f.FieldType));
                            return Unit.Value;
                        }
                    );
                }
            }.Match(memberExpression.Member).UnsafeGet(_ =>
                new InvalidOperationException("Type member must be either a property or a field.")
            );

            return data;
        }

        private static MemberExpression GetMemberExpression<T, TKey>(this Expression<Func<T, TKey>> expression) =>
            expression == null ? throw new ArgumentNullException(nameof(expression)) :
            expression.Body is UnaryExpression u && u.Operand is MemberExpression m1 ? m1 :
            expression.Body is MemberExpression m2 ? m2 :
            throw new ArgumentException("The expression does not indicate a valid property or a field.");

        private static (Type Parent, ImmutableList<(MemberTypes Type, string Name)> Children) GetTarget(this ImmutableList<(MemberTypes, string)> nested, MemberExpression expression) =>
            expression.Expression.NodeType.Match(
                ExpressionType.Parameter, _ => (expression.Expression.Type, nested.Add((expression.Member.MemberType, expression.Member.Name)).Reverse()),
                ExpressionType.MemberAccess, _ => new TypePattern<(Type, ImmutableList<(MemberTypes, string)>)>
                {
                    (PropertyInfo p) => GetTarget(
                        nested.Add((expression.Member.MemberType, expression.Member.Name)),
                        (MemberExpression)expression.Expression
                    ),
                    (FieldInfo f) => GetTarget(
                        nested.Add((expression.Member.MemberType, expression.Member.Name)),
                        (MemberExpression)expression.Expression
                    )
                }.Match(expression.Member).UnsafeGet(__ =>
                    new ArgumentException("The expression does not indicate a valid property or a field.")
                ),
                otherwiseThrow: _ => new InvalidOperationException("Type member must be either a property or a field.")
            );
        
        private static (Type, object) Reduce(this object data, Type parent, IImmutableList<(MemberTypes, string)> l) =>
            l.Aggregate((parent, data), (a, b) =>
            {
                return b.Item1.Match(
                    MemberTypes.Property, __ =>
                    {
                        var obj = a.parent.GetProperty(b.Item2)?.GetValue(a.Item2, null);
                        return (obj?.GetType(), obj);
                    },
                    MemberTypes.Field, __ =>
                    {
                        var info = a.parent.GetField(b.Item2);
                        return (info?.GetType(), info?.GetValue(a.Item2));
                    }
                );
            });
    }
}
