using System;

namespace Funk
{
    public static class ApplicativeExt
    {
        public static Maybe<R> Apply<T1, R>(this Maybe<Func<T1, R>> function, Maybe<T1> maybe) => function.FlatMap(maybe.Map);
        public static Maybe<Unit> Apply<T1>(this Maybe<Action<T1>> function, Maybe<T1> maybe) =>
            function.FlatMap(f => maybe.Map(m =>
            {
                f(m);
                return Unit.Value;
            }));

        public static Maybe<Func<T2, R>> Apply<T1, T2, R>(this Maybe<Func<T1, T2, R>> function, Maybe<T1> maybe) => function.FlatMap(f => maybe.Map(f.Apply));
        
        public static Maybe<Action<T2>> Apply<T1, T2>(this Maybe<Action<T1, T2>> function, Maybe<T1> maybe) => function.FlatMap(f => maybe.Map(f.Apply));

        public static Maybe<Func<T2, T3, R>> Apply<T1, T2, T3, R>(this Maybe<Func<T1, T2, T3, R>> function, Maybe<T1> maybe) => function.FlatMap(f => maybe.Map(f.Apply));
        
        public static Maybe<Action<T2, T3>> Apply<T1, T2, T3>(this Maybe<Action<T1, T2, T3>> function, Maybe<T1> maybe) => function.FlatMap(f => maybe.Map(f.Apply));

        public static Maybe<Func<T2, T3, T4, R>> Apply<T1, T2, T3, T4, R>(this Maybe<Func<T1, T2, T3, T4, R>> function, Maybe<T1> maybe) => function.FlatMap(f => maybe.Map(f.Apply));
        
        public static Maybe<Action<T2, T3, T4>> Apply<T1, T2, T3, T4>(this Maybe<Action<T1, T2, T3, T4>> function, Maybe<T1> maybe) => function.FlatMap(f => maybe.Map(f.Apply));

        public static Maybe<Func<T2, T3, T4, T5, R>> Apply<T1, T2, T3, T4, T5, R>(this Maybe<Func<T1, T2, T3, T4, T5, R>> function, Maybe<T1> maybe) => function.FlatMap(f => maybe.Map(f.Apply));
        
        public static Maybe<Action<T2, T3, T4, T5>> Apply<T1, T2, T3, T4, T5>(this Maybe<Action<T1, T2, T3, T4, T5>> function, Maybe<T1> maybe) => function.FlatMap(f => maybe.Map(f.Apply));

        public static Exc<R, E> Apply<T1, R, E>(this Exc<Func<T1, R>, E> function, Exc<T1, E> exceptional) where E : Exception => function.FlatMap(exceptional.Map);

        public static Exc<Unit, E> Apply<T1, E>(this Exc<Action<T1>, E> function, Exc<T1, E> exceptional) where E : Exception =>
            function.FlatMap(f => exceptional.Map(m =>
            {
                f(m);
                return Unit.Value;
            }));

        /// <summary>
        /// Returns success if both are successful. Otherwise, returns error/s as part of EnumerableException.
        /// In case of continuation use Apply instead.
        /// </summary>
        public static Exc<R, E> Validate<T1, R, E>(this Exc<Func<T1, R>, E> function, Exc<T1, E> exceptional) where E : Exception
        {
            return function.Match(
                _ => Exc.Empty<R, E>(),
                f => exceptional.Match(
                    __ => Exc.Empty<R, E>(),
                    v => Exc.Create<R, E>(_ => f(v)),
                    Exc.Failure<R, E>
                ),
                e => exceptional.Match(
                    __ => Exc.Empty<R, E>(),
                    v => Exc.Failure<R, E>(e),
                    ee => Exc.Failure<R, E>(e.Merge(ee))
                )
            );
        }
        
        /// <summary>
        /// Returns success if both are successful. Otherwise, returns error/s as part of EnumerableException.
        /// In case of continuation use Apply instead.
        /// </summary>
        public static Exc<Unit, E> Validate<T1, E>(this Exc<Action<T1>, E> function, Exc<T1, E> exceptional) where E : Exception
        {
            return function.Match(
                _ => Exc.Empty<Unit, E>(),
                f => exceptional.Match(
                    __ => Exc.Empty<Unit, E>(),
                    v => Exc.Create<Unit, E>(_ =>
                    {
                        f(v);
                        return Unit.Value;
                    }),
                    Exc.Failure<Unit, E>
                ),
                e => exceptional.Match(
                    __ => Exc.Empty<Unit, E>(),
                    v => Exc.Failure<Unit, E>(e),
                    ee => Exc.Failure<Unit, E>(e.Merge(ee))
                )
            );
        }

        public static Exc<Func<T2, R>, E> Apply<T1, T2, R, E>(this Exc<Func<T1, T2, R>, E> function, Exc<T1, E> exceptional) where E : Exception => function.FlatMap(f => exceptional.Map(f.Apply));
        
        public static Exc<Action<T2>, E> Apply<T1, T2, E>(this Exc<Action<T1, T2>, E> function, Exc<T1, E> exceptional) where E : Exception => function.FlatMap(f => exceptional.Map(f.Apply));

        /// <summary>
        /// Returns success if both are successful. Otherwise, returns error/s as part of EnumerableException.
        /// In case of continuation use Apply instead.
        /// </summary>
        public static Exc<Func<T2, R>, E> Validate<T1, T2, R, E>(this Exc<Func<T1, T2, R>, E> function, Exc<T1, E> exceptional) where E : Exception
        {
            return function.Match(
                _ => Exc.Empty<Func<T2, R>, E>(),
                f => exceptional.Match(
                    __ => Exc.Empty<Func<T2, R>, E>(),
                    v => Exc.Create<Func<T2, R>, E>(_ => f.Apply(v)),
                    Exc.Failure<Func<T2, R>, E>
                ),
                e => exceptional.Match(
                    __ => Exc.Empty<Func<T2, R>, E>(),
                    v => Exc.Failure<Func<T2, R>, E>(e),
                    ee => Exc.Failure<Func<T2, R>, E>(e.Merge(ee))
                )
            );
        }
        
        /// <summary>
        /// Returns success if both are successful. Otherwise, returns error/s as part of EnumerableException.
        /// In case of continuation use Apply instead.
        /// </summary>
        public static Exc<Action<T2>, E> Validate<T1, T2, E>(this Exc<Action<T1, T2>, E> function, Exc<T1, E> exceptional) where E : Exception
        {
            return function.Match(
                _ => Exc.Empty<Action<T2>, E>(),
                f => exceptional.Match(
                    __ => Exc.Empty<Action<T2>, E>(),
                    v => Exc.Create<Action<T2>, E>(_ => f.Apply(v)),
                    Exc.Failure<Action<T2>, E>
                ),
                e => exceptional.Match(
                    __ => Exc.Empty<Action<T2>, E>(),
                    v => Exc.Failure<Action<T2>, E>(e),
                    ee => Exc.Failure<Action<T2>, E>(e.Merge(ee))
                )
            );
        }

        public static Exc<Func<T2, T3, R>, E> Apply<T1, T2, T3, R, E>(this Exc<Func<T1, T2, T3, R>, E> function, Exc<T1, E> exceptional) where E : Exception => function.FlatMap(f => exceptional.Map(f.Apply));
        
        public static Exc<Action<T2, T3>, E> Apply<T1, T2, T3, E>(this Exc<Action<T1, T2, T3>, E> function, Exc<T1, E> exceptional) where E : Exception => function.FlatMap(f => exceptional.Map(f.Apply));

        /// <summary>
        /// Returns success if both are successful. Otherwise, returns error/s as part of EnumerableException.
        /// In case of continuation use Apply instead.
        /// </summary>
        public static Exc<Func<T2, T3, R>, E> Validate<T1, T2, T3, R, E>(this Exc<Func<T1, T2, T3, R>, E> function, Exc<T1, E> exceptional) where E : Exception
        {
            return function.Match(
                _ => Exc.Empty<Func<T2, T3, R>, E>(),
                f => exceptional.Match(
                    __ => Exc.Empty<Func<T2, T3, R>, E>(),
                    v => Exc.Create<Func<T2, T3, R>, E>(_ => f.Apply(v)),
                    Exc.Failure<Func<T2, T3, R>, E>
                ),
                e => exceptional.Match(
                    __ => Exc.Empty<Func<T2, T3, R>, E>(),
                    v => Exc.Failure<Func<T2, T3, R>, E>(e),
                    ee => Exc.Failure<Func<T2, T3, R>, E>(e.Merge(ee))
                )
            );
        }
        
        /// <summary>
        /// Returns success if both are successful. Otherwise, returns error/s as part of EnumerableException.
        /// In case of continuation use Apply instead.
        /// </summary>
        public static Exc<Action<T2, T3>, E> Validate<T1, T2, T3, E>(this Exc<Action<T1, T2, T3>, E> function, Exc<T1, E> exceptional) where E : Exception
        {
            return function.Match(
                _ => Exc.Empty<Action<T2, T3>, E>(),
                f => exceptional.Match(
                    __ => Exc.Empty<Action<T2, T3>, E>(),
                    v => Exc.Create<Action<T2, T3>, E>(_ => f.Apply(v)),
                    Exc.Failure<Action<T2, T3>, E>
                ),
                e => exceptional.Match(
                    __ => Exc.Empty<Action<T2, T3>, E>(),
                    v => Exc.Failure<Action<T2, T3>, E>(e),
                    ee => Exc.Failure<Action<T2, T3>, E>(e.Merge(ee))
                )
            );
        }

        public static Exc<Func<T2, T3, T4, R>, E> Apply<T1, T2, T3, T4, R, E>(this Exc<Func<T1, T2, T3, T4, R>, E> function, Exc<T1, E> exceptional) where E : Exception => function.FlatMap(f => exceptional.Map(f.Apply));
        
        public static Exc<Action<T2, T3, T4>, E> Apply<T1, T2, T3, T4, E>(this Exc<Action<T1, T2, T3, T4>, E> function, Exc<T1, E> exceptional) where E : Exception => function.FlatMap(f => exceptional.Map(f.Apply));

        /// <summary>
        /// Returns success if both are successful. Otherwise, returns error/s as part of EnumerableException.
        /// In case of continuation use Apply instead.
        /// </summary>
        public static Exc<Func<T2, T3, T4, R>, E> Validate<T1, T2, T3, T4, R, E>(this Exc<Func<T1, T2, T3, T4, R>, E> function, Exc<T1, E> exceptional) where E : Exception
        {
            return function.Match(
                _ => Exc.Empty<Func<T2, T3, T4, R>, E>(),
                f => exceptional.Match(
                    __ => Exc.Empty<Func<T2, T3, T4, R>, E>(),
                    v => Exc.Create<Func<T2, T3, T4, R>, E>(_ => f.Apply(v)),
                    Exc.Failure<Func<T2, T3, T4, R>, E>
                ),
                e => exceptional.Match(
                    __ => Exc.Empty<Func<T2, T3, T4, R>, E>(),
                    v => Exc.Failure<Func<T2, T3, T4, R>, E>(e),
                    ee => Exc.Failure<Func<T2, T3, T4, R>, E>(e.Merge(ee))
                )
            );
        }
        
        /// <summary>
        /// Returns success if both are successful. Otherwise, returns error/s as part of EnumerableException.
        /// In case of continuation use Apply instead.
        /// </summary>
        public static Exc<Action<T2, T3, T4>, E> Validate<T1, T2, T3, T4, E>(this Exc<Action<T1, T2, T3, T4>, E> function, Exc<T1, E> exceptional) where E : Exception
        {
            return function.Match(
                _ => Exc.Empty<Action<T2, T3, T4>, E>(),
                f => exceptional.Match(
                    __ => Exc.Empty<Action<T2, T3, T4>, E>(),
                    v => Exc.Create<Action<T2, T3, T4>, E>(_ => f.Apply(v)),
                    Exc.Failure<Action<T2, T3, T4>, E>
                ),
                e => exceptional.Match(
                    __ => Exc.Empty<Action<T2, T3, T4>, E>(),
                    v => Exc.Failure<Action<T2, T3, T4>, E>(e),
                    ee => Exc.Failure<Action<T2, T3, T4>, E>(e.Merge(ee))
                )
            );
        }

        public static Exc<Func<T2, T3, T4, T5, R>, E> Apply<T1, T2, T3, T4, T5, R, E>(this Exc<Func<T1, T2, T3, T4, T5, R>, E> function, Exc<T1, E> exceptional) where E : Exception => function.FlatMap(f => exceptional.Map(f.Apply));
        
        public static Exc<Action<T2, T3, T4, T5>, E> Apply<T1, T2, T3, T4, T5, E>(this Exc<Action<T1, T2, T3, T4, T5>, E> function, Exc<T1, E> exceptional) where E : Exception => function.FlatMap(f => exceptional.Map(f.Apply));

        /// <summary>
        /// Returns success if both are successful. Otherwise, returns error/s as part of EnumerableException.
        /// In case of continuation use Apply instead.
        /// </summary>
        public static Exc<Func<T2, T3, T4, T5, R>, E> Validate<T1, T2, T3, T4, T5, R, E>(this Exc<Func<T1, T2, T3, T4, T5, R>, E> function, Exc<T1, E> exceptional) where E : Exception
        {
            return function.Match(
                _ => Exc.Empty<Func<T2, T3, T4, T5, R>, E>(),
                f => exceptional.Match(
                    __ => Exc.Empty<Func<T2, T3, T4, T5, R>, E>(),
                    v => Exc.Create<Func<T2, T3, T4, T5, R>, E>(_ => f.Apply(v)),
                    Exc.Failure<Func<T2, T3, T4, T5, R>, E>
                ),
                e => exceptional.Match(
                    __ => Exc.Empty<Func<T2, T3, T4, T5, R>, E>(),
                    v => Exc.Failure<Func<T2, T3, T4, T5, R>, E>(e),
                    ee => Exc.Failure<Func<T2, T3, T4, T5, R>, E>(e.Merge(ee))
                )
            );
        }
        
        /// <summary>
        /// Returns success if both are successful. Otherwise, returns error/s as part of EnumerableException.
        /// In case of continuation use Apply instead.
        /// </summary>
        public static Exc<Action<T2, T3, T4, T5>, E> Validate<T1, T2, T3, T4, T5, E>(this Exc<Action<T1, T2, T3, T4, T5>, E> function, Exc<T1, E> exceptional) where E : Exception
        {
            return function.Match(
                _ => Exc.Empty<Action<T2, T3, T4, T5>, E>(),
                f => exceptional.Match(
                    __ => Exc.Empty<Action<T2, T3, T4, T5>, E>(),
                    v => Exc.Create<Action<T2, T3, T4, T5>, E>(_ => f.Apply(v)),
                    Exc.Failure<Action<T2, T3, T4, T5>, E>
                ),
                e => exceptional.Match(
                    __ => Exc.Empty<Action<T2, T3, T4, T5>, E>(),
                    v => Exc.Failure<Action<T2, T3, T4, T5>, E>(e),
                    ee => Exc.Failure<Action<T2, T3, T4, T5>, E>(e.Merge(ee))
                )
            );
        }
    }
}