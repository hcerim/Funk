using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using FsCheck.Xunit;
using Xunit;
using static Funk.Prelude;

namespace Funk.Tests
{
    public class ExcTests : Test
    {
        [Fact]
        public void Create_Exceptional()
        {
            UnitTest(
                _ => Exc.Create<string, ArgumentException>(__ => GetNameById("Funk12")),
                e => e.Failure,
                f =>
                {
                    Assert.True(f.NotEmpty);
                    Assert.NotEmpty(f.UnsafeGet());
                    var action = act(() => throw f.UnsafeGet());
                    Assert.Throws<EnumerableException<ArgumentException>>(action);
                }
            );
        }

        [Fact]
        public void Create_Exceptional_Throws()
        {
            UnitTest(
                _ => "Funk12",
                s => act(() => Exc.Create<string, EmptyValueException>(_ => GetNameById(s))),
                a => Assert.Throws<ArgumentException>(a)
            );
        }

        [Fact]
        public void Create_Exceptional_Throws_Recover()
        {
            UnitTest(
                _ => "Funk12",
                s =>
                {
                    return Exc.Create<string, ArgumentException>(_ => GetNameById(s))
                        .OnFailure(e => GetNameById("Funk123"));
                },
                e =>
                {
                    Assert.True(e.IsSuccess);
                    Assert.Equal("Harun", e.Success.UnsafeGet());
                }
            );
        }

        [Fact]
        public void Create_Exceptional_Throws_Recover_Chain()
        {
            UnitTest(
                _ => "Funk12",
                s =>
                {
                    return Exc.Create<string, ArgumentException>(_ => GetNameById(s))
                        .OnFailure(e => GetNameById("Funk1"))
                        .OnFailure(e => GetNameById("Funk123"));
                },
                e =>
                {
                    Assert.True(e.IsSuccess);
                    Assert.Equal("Harun", e.Success.UnsafeGet());
                }
            );
        }

        [Fact]
        public void Create_Exceptional_Throws_Recover_Chain_First()
        {
            UnitTest(
                _ => "Funk123",
                s =>
                {
                    return Exc.Create<string, ArgumentException>(_ => GetNameById(s))
                        .OnFailure(e => GetNameById("Funk1"))
                        .OnFailure(e => GetNameById("Funk12"));
                },
                e =>
                {
                    Assert.True(e.IsSuccess);
                    Assert.Equal("Harun", e.Success.UnsafeGet());
                }
            );
        }

        [Fact]
        public void Create_Exceptional_Throws_Recover_Chain_Throws()
        {
            UnitTest(
                _ => "Funk12",
                s =>
                {
                    return act(() => Exc.Create<string, ArgumentException>(_ => GetNameById(s))
                        .OnFailure(e => GetNameById(null)));
                },
                a => Assert.Throws<InvalidOperationException>(a)
            );
        }

        [Fact]
        public async Task Create_Exceptional_Throws_Recover_Chain_Throws_Async()
        {
            await UnitTestAsync(
                _ => result("Funk12"),
                s =>
                {
                    return result(act(async () =>
                    {
                        await Exc.CreateAsync<string, ArgumentException>(_ => GetNameByIdAsync(s))
                            .OnFailureAsync(e => GetNameByIdAsync(null));
                    }));
                },
                async a => Assert.Throws<InvalidOperationException>(await a)
            );
        }

        [Fact]
        public async Task Create_Exceptional_Throws_Recover_On_Empty()
        {
            await UnitTestAsync(
                _ => result("Funk12"),
                s =>
                {
                    return run(func(async () =>
                    {
                        var result = await Exc.CreateAsync<string, ArgumentException>(_ => GetNameByIdAsync(s));
                        return result.OnFailure(e => GetNullString()).OnEmpty(_ => GetNameById("Funk123"));
                    }));
                },
                e => Assert.Equal("Harun", e.UnsafeGetFirst())
            );
        }

        [Fact]
        public async Task Create_Exceptional_Throws_Recover_On_Empty_Ensure_Success()
        {
            await UnitTestAsync(
                _ => result("Funk12"),
                s =>
                {
                    return run(func(async () =>
                    {
                        var result = await Exc.CreateAsync<string, ArgumentException>(_ => GetNameByIdAsync(s));
                        return result.OnFailure(e => GetNullString()).OnEmpty(_ => GetNameById("Funk123"));
                    }));
                },
                e =>
                {
                    var maybe = e.AsSuccess();
                    Assert.True(maybe.NotEmpty);
                    Assert.Equal("Harun", maybe.UnsafeGet());
                }
            );
        }

        [Fact]
        public async Task Create_Exceptional_Throws_Recover_On_Empty_Nothing()
        {
            await UnitTestAsync(
                _ => result("Funk12"),
                s =>
                {
                    return run(func(async () =>
                    {
                        var result = await Exc.CreateAsync<string, ArgumentException>(_ => GetNameByIdAsync(s));
                        return result.OnFailure(e => GetNameById("Funk12")).OnEmpty(_ => GetNameById("Funk123"));
                    }));
                },
                e => Assert.IsType<EnumerableException<ArgumentException>>(e.Failure.UnsafeGet())
            );
        }

        [Fact]
        public void Create_Exceptional_Continue()
        {
            UnitTest(
                _ => "Funk123",
                s =>
                {
                    return Exc.Create<string, ArgumentException>(_ => GetNameById(s))
                        .Map(ss => ss.Concat(GetNameById(s)));
                },
                s => Assert.Equal("HarunHarun", s.UnsafeGetFirst())
            );
        }

        [Fact]
        public async Task Create_Exceptional_Continue_When_Failure()
        {
            await UnitTestAsync(
                _ => result("Funk12"),
                s =>
                {
                    return Exc.CreateAsync<string, ArgumentException>(_ => GetNameByIdAsync(s))
                        .MapAsync(async ss => ss.Concat(await GetNameByIdAsync(s)))
                        .MapAsync(async ss => ss.Concat(await GetNameByIdAsync(s)));
                },
                e =>
                {
                    var root = e.RootFailure.UnsafeGet();
                    Assert.Equal("Invalid id", root.Message);
                    Assert.IsType<EnumerableException<ArgumentException>>(e.UnsafeGetSecond());
                }
            );
        }

        [Fact]
        public async Task Create_Exceptional_Recover_Continue_Empty()
        {
            await UnitTestAsync(
                _ => result("Funk12"),
                s =>
                {
                    return Exc.CreateAsync<string, ArgumentException>(_ => GetNameByIdAsync(s))
                        .OnFailureAsync(e => GetNullStringAsync())
                        .OnEmptyAsync(_ => GetNameByIdAsync("Funk123"))
                        .MapAsync(async ss => ss.Concat(await GetNameByIdAsync("Funk123")));
                },
                s => Assert.Equal("HarunHarun", s.UnsafeGetFirst())
            );
        }

        [Fact]
        public async Task Create_Exceptional_Merge_With_Another()
        {
            await UnitTestAsync(
                _ => result("Funk12"),
                async s =>
                {
                    var first = await Exc.CreateAsync<string, ArgumentException>(_ => GetNameByIdAsync(s))
                        .OnFailureAsync(e => GetNullStringAsync())
                        .OnEmptyAsync(_ => GetNameByIdAsync("Funk123"))
                        .MapAsync(async ss => ss.Concat(await GetNameByIdAsync("Funk123")).ToString());

                    var second = Exc.Empty<string, ArgumentException>();
                    return first.Merge(second);
                },
                r => Assert.True(r.IsEmpty)
            );
        }

        [Fact]
        public async Task Create_Exceptional_Merge_With_Another_Map_Failure()
        {
            await UnitTestAsync(
                _ => result("Funk12"),
                async s =>
                {
                    var first = await Exc.CreateAsync<string, ArgumentException>(_ => GetNameByIdAsync(s))
                        .OnFailureAsync(e => GetNullStringAsync())
                        .OnEmptyAsync(_ => GetNameByIdAsync("Funk123"))
                        .MapAsync(async ss => ss.Concat(await GetNameByIdAsync("Funk123")).ToString());

                    return first.Merge(failure<string, ArgumentException>(new ArgumentException("Error occured")))
                        .MapFailure(e => new InvalidOperationException("New Exception type."));
                },
                r =>
                {
                    Assert.True(r.IsFailure);
                    Assert.IsType<EnumerableException<InvalidOperationException>>(r.Failure.UnsafeGet());
                    Assert.Equal("New Exception type.", r.RootFailure.UnsafeGet().Message);
                }
            );
        }

        [Fact]
        public async Task Create_Exceptional_Merge_With_More()
        {
            await UnitTestAsync(
                _ => result("Funk12"),
                async s =>
                {
                    var first = await Exc.CreateAsync<string, ArgumentException>(_ => GetNameByIdAsync(s))
                        .OnFailureAsync(e => GetNullStringAsync())
                        .OnEmptyAsync(_ => GetNameByIdAsync("Funk123"))
                        .MapAsync(async ss => ss.Concat(await GetNameByIdAsync("Funk123")).ToString());

                    var second = failure<string, ArgumentException>(new ArgumentException("Error occured")).ToImmutableList()
                        .SafeConcat(failure<string, ArgumentException>(new ArgumentException("Another occured")).ToImmutableList())
                        .SafeConcat(Exc.Empty<string, ArgumentException>().ToImmutableList());
                    return first.MergeRange(second);
                },
                r =>
                {
                    Assert.True(r.IsFailure);
                    Assert.IsType<EnumerableException<ArgumentException>>(r.Failure.UnsafeGet());
                    Assert.Equal(2, r.Failure.UnsafeGet().Count);
                    Assert.Equal("Error occured", r.RootFailure.UnsafeGet().Message);
                }
            );
        }

        [Fact]
        public async Task Create_Exceptional_MapFailureAsync()
        {
            await UnitTestAsync(
                _ => result("Funk12"),
                async s =>
                {
                    return await Exc.CreateAsync<string, ArgumentException>(_ => GetNameByIdAsync(s))
                        .OnFailureAsync(e => GetNullStringAsync())
                        .OnEmptyAsync(_ => GetNameByIdAsync(s))
                        .MapAsync(async ss => ss.Concat(await GetNameByIdAsync("Funk123")).ToString())
                        .MapFailureAsync(e => new InvalidOperationException(e.Root.UnsafeGet().Message));
                },
                r =>
                {
                    Assert.True(r.IsFailure);
                    Assert.IsType<EnumerableException<InvalidOperationException>>(r.Failure.UnsafeGet());
                    Assert.Equal("Invalid id", r.RootFailure.UnsafeGet().Message);
                }
            );
        }

        [Fact]
        public void Create_Exceptional_Recover_Continue_Empty_Custom_Type()
        {
            UnitTest(
                _ => new InformationSource(new FirstSource("Third")), 
                s =>
                {
                    return Exc.Create<IImmutableList<string>, ArgumentException>(_ => s.UnsafeGetFirst().GetInformation())
                        .OnFailure(e =>
                        {
                            var information = new InformationSource(new SecondSource("First"));
                            return information.UnsafeGetSecond().GetInformation();
                        })
                        .OnFailure(e => default(IImmutableList<string>))
                        .OnEmpty(_ =>
                        {
                            var information = new InformationSource(new FirstSource("First"));
                            return information.UnsafeGetFirst().GetInformation();
                        });
                },
                s =>
                {
                    Assert.Equal(new List<string> { "1", "2", "3" }.Map(), s.UnsafeGetFirst());
                }
            );
        }

        [Property(Arbitrary = new[] { typeof(ArbitraryLifter) })]
        public void RightIdentity(Exc<object, Exception> maybe)
        {
            UnitTest(
                _ => maybe,
                e => e.SafeEquals(e.FlatMap(success<object, Exception>)),
                Assert.True
            );
        }

        [Property]
        public void LeftIdentity(object obj)
        {
            UnitTest(
                _ => func((object o) => o.AsMaybe().ToExc(__ => new Exception())),
                f => obj.AsMaybe().ToExc(_ => new Exception()).FlatMap(f).Success.SafeEquals(f(obj).Success),
                Assert.True
            );
        }

        [Property(Arbitrary = new[] { typeof(ArbitraryLifter) })]
        public void Associativity(Exc<int, Exception> maybe)
        {
            UnitTest(
                _ => rec(func((int o) => Exc.Create<int, Exception>(__ => o / 3)), func((int o) => success<int, Exception>(o * 4))),
                r => maybe.FlatMap(r.Item1).FlatMap(r.Item2).SafeEquals(maybe.FlatMap(v => r.Item1(v).FlatMap(r.Item2))),
                Assert.True
            );
        }

        private static string GetNameById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new InvalidOperationException("Id cannot be null.");
            }
            if (id.SafeEquals("Funk123"))
            {
                return "Harun";
            }
            throw new ArgumentException("Invalid id");
        }

        private static string GetNullString()
        {
            return null;
        }

        private static Task<string> GetNullStringAsync()
        {
            return result(GetNullString());
        }

        private static Task<string> GetNameByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new InvalidOperationException("Id cannot be null.");
            }
            if (id.SafeEquals("Funk123"))
            {
                return result("Harun");
            }
            throw new ArgumentException("Invalid id");
        }
    }

    public class InformationSource : OneOf<FirstSource, SecondSource>
    {
        public InformationSource(FirstSource first)
            : base(first)
        {
        }

        public InformationSource(SecondSource second)
            : base(second)
        {
        }
    }

    public class FirstSource
    {
        public FirstSource(string sourceName)
        {
            SourceName = sourceName;
        }

        private string SourceName { get; }

        private static IImmutableList<string> List => new List<string> {"1", "2", "3"}.Map();

        public IImmutableList<string> GetInformation()
        {
            if (SourceName.SafeEquals("First"))
            {
                return List;
            }
            throw new ArgumentException();
        }
    }

    public class SecondSource
    {
        public SecondSource(string sourceName)
        {
            SourceName = sourceName;
        }

        private string SourceName { get; }

        private static IImmutableList<string> List => new List<string> {"4", "5", "6"}.Map();

        public IImmutableList<string> GetInformation()
        {
            if (SourceName.SafeEquals("Second"))
            {
                return List;
            }
            throw new ArgumentException();
        }
    }
}
