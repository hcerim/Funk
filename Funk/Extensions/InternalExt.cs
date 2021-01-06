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

        internal static void Execute(this Action<Unit> otherwise) =>
            otherwise.AsMaybe().Match(
                _ => throw UnhandledException,
                e => e(Unit.Value)
            );

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
        
        internal static T Map<T, TKey>(this T data, Expression<Func<T, TKey>> expression, TKey value)
        {
            var memberExpression = expression.GetMemberExpression();
            new TypePattern<Unit>
            {
                (PropertyInfo p) =>
                {
                    p.GetSetMethod(true).AsMaybe().UnsafeGet(__ =>
                        new InvalidOperationException($"Set method for property '{p.Name}' not found.")
                    );
                    var target = GetTarget(ImmutableList<(MemberTypes, string)>.Empty, memberExpression);
                    var nested = target.Children.Take(target.Children.Count - 1).AsNotEmptyList();
                    return nested.Match(
                        _ =>
                        {
                            p.SetValue(data, value, null);
                            return Unit.Value;
                        },
                        l =>
                        {
                            var aggregate = l.Reduce(target.Parent, data);
                            p.SetValue(aggregate.data, value, null);
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
                            f.SetValue(data, value);
                            return Unit.Value;
                        },
                        l =>
                        {
                            var aggregate = l.Reduce(target.Parent, data);
                            f.SetValue(aggregate.data, value);
                            return Unit.Value;
                        }
                    );
                }
            }.Match(memberExpression.Member).UnsafeGet(_ =>
                new InvalidOperationException("Type member must be either a property or a field.")
            );

            return data;
        }
        
        internal static (Type type, string member) GetMember<T, TKey>(this Expression<Func<T, TKey>> expression)
        {
            var memberExpression = expression.GetMemberExpression();
            return new TypePattern<(Type, string)>
            {
                (PropertyInfo p) =>
                {
                    var target = GetTarget(ImmutableList<(MemberTypes, string)>.Empty, memberExpression);
                    var nested = target.Children.Take(target.Children.Count - 1).AsNotEmptyList();
                    return nested.Match(
                        _ => (target.Parent, p.Name),
                        l => (l.Reduce(target.Parent), p.Name)
                    );
                },
                (FieldInfo f) =>
                {
                    var target = GetTarget(ImmutableList<(MemberTypes, string)>.Empty, memberExpression);
                    var nested = target.Children.Take(target.Children.Count - 1).AsNotEmptyList();
                    return nested.Match(
                        _ => (target.Parent, f.Name),
                        l => (l.Reduce(target.Parent), f.Name)
                    );
                }
            }.Match(memberExpression.Member).UnsafeGet(_ =>
                new InvalidOperationException("Type member must be either a property or a field.")
            );
        }

        private static MemberExpression GetMemberExpression<T, TKey>(this Expression<Func<T, TKey>> expression) =>
            expression == null ? throw new ArgumentNullException(nameof(expression)) :
            expression.Body is UnaryExpression u && u.Operand is MemberExpression m1 ? m1 :
            expression.Body is MemberExpression m2 ? m2 :
            throw new ArgumentException("The expression does not indicate a valid property or a field.");

        private static (Type Parent, ImmutableList<(MemberTypes Type, string Name)> Children) GetTarget(this ImmutableList<(MemberTypes, string)> nested, MemberExpression expression) =>
            expression.Expression.NodeType.Match(
                ExpressionType.Parameter, _ => (expression.Expression.Type, nested.Add((expression.Member.MemberType, expression.Member.Name)).Reverse()),
                ExpressionType.MemberAccess, _ => (expression.Member is PropertyInfo).AsMaybe().Or(__ => (expression.Member is FieldInfo).AsMaybe()).Map(__ =>
                    GetTarget(nested.Add((expression.Member.MemberType, expression.Member.Name)), (MemberExpression) expression.Expression)
                ).UnsafeGet(__ => new ArgumentException("The expression does not indicate a valid property or a field.")),
                otherwiseThrow: _ => new InvalidOperationException("Type member must be either a property or a field.")
            );
        
        private static (Type _, object data) Reduce(this IImmutableList<(MemberTypes type, string child)> l, Type parent, object data) =>
            l.Aggregate((parent, data), (a, b) => b.type.Match(
                MemberTypes.Property, __ =>
                {
                    var obj = a.parent.GetProperty(b.child).GetValue(a.data, null).AsMaybe().UnsafeGet(_ =>
                        new EmptyValueException($"{b.child} is empty.")
                    );
                    return (obj.GetType(), obj);
                },
                MemberTypes.Field, __ =>
                {
                    var obj = a.parent.GetField(b.child).GetValue(a.data).AsMaybe().UnsafeGet(_ =>
                        new EmptyValueException($"{b.child} is empty.")
                    );
                    return (obj.GetType(), obj);
                }
            ));
        
        private static Type Reduce(this IImmutableList<(MemberTypes type, string child)> l, Type parent) =>
            l.Aggregate(parent, (a, b) => b.type.Match(
                MemberTypes.Property, __ => a.GetProperty(b.child).PropertyType,
                MemberTypes.Field, __ => a.GetField(b.child).FieldType
            ));
    }
}
