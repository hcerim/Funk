using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
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
        public void Create_Exceptional_Throws_Recover_Chain_Throws_Async()
        {
            UnitTest(
                _ => "Funk12",
                s =>
                {
                    return act(() =>
                    {
                        var result = Exc.CreateAsync<string, ArgumentException>(_ => GetNameByIdAsync(s));
                        result.OnFailureAsync(e => GetNameByIdAsync(null)).GetAwaiter().GetResult();
                    });
                },
                a => Assert.Throws<InvalidOperationException>(a)
            );
        }

        [Fact]
        public void Create_Exceptional_Throws_Recover_On_Empty()
        {
            UnitTest(
                _ => "Funk12",
                s =>
                {
                    return func(() =>
                    {
                        var result = Exc.CreateAsync<string, ArgumentException>(_ => GetNameByIdAsync(s)).GetAwaiter().GetResult();
                        return result.OnFailure(e => GetNullString()).OnEmpty(_ => GetNameById("Funk123"));
                    });
                },
                f =>
                {
                    var result = f.Invoke();
                    Assert.Equal("Harun", result.UnsafeGetFirst());
                }
            );
        }

        [Fact]
        public void Create_Exceptional_Throws_Recover_On_Empty_Ensure_Success()
        {
            UnitTest(
                _ => "Funk12",
                s =>
                {
                    return func(() =>
                    {
                        var result = Exc.CreateAsync<string, ArgumentException>(_ => GetNameByIdAsync(s)).GetAwaiter().GetResult();
                        return result.OnFailure(e => GetNullString()).OnEmpty(_ => GetNameById("Funk123"));
                    });
                },
                f =>
                {
                    var result = f.Invoke();
                    var maybe = result.AsSuccess();
                    Assert.True(maybe.NotEmpty);
                    Assert.Equal("Harun", maybe.UnsafeGet().UnsafeGetFirst());
                }
            );
        }

        [Fact]
        public void Create_Exceptional_Throws_Recover_On_Empty_Nothing()
        {
            UnitTest(
                _ => "Funk12",
                s =>
                {
                    return func(() =>
                    {
                        var result = Exc.CreateAsync<string, ArgumentException>(_ => GetNameByIdAsync(s)).GetAwaiter().GetResult();
                        return result.OnFailure(e => GetNameById("Funk12")).OnEmpty(_ => GetNameById("Funk123"));
                    });
                },
                f =>
                {
                    var result = f.Invoke();
                    Assert.IsType<EnumerableException<ArgumentException>>(result.Failure.UnsafeGet());
                }
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
        public void Create_Exceptional_Continue_When_Failure()
        {
            UnitTest(
                _ => "Funk12",
                s =>
                {
                    return Exc.CreateAsync<string, ArgumentException>(_ => GetNameByIdAsync(s))
                        .MapAsync(async ss => ss.Concat(await GetNameByIdAsync(s)))
                        .MapAsync(async ss => ss.Concat(await GetNameByIdAsync(s))).GetAwaiter().GetResult();
                },
                e =>
                {
                    var root = e.RootFailure.UnsafeGet();
                    Assert.Equal("Invalid id", root.Message);
                    Assert.IsType<EnumerableException<ArgumentException>>(e.UnsafeGetSecond());
                });
        }

        [Fact]
        public void Create_Exceptional_Recover_Continue_Empty()
        {
            UnitTest(
                _ => "Funk12",
                s =>
                {
                    return Exc.CreateAsync<string, ArgumentException>(_ => GetNameByIdAsync(s))
                        .OnFailureAsync(e => GetNullStringAsync())
                        .OnEmptyAsync(_ => GetNameByIdAsync("Funk123"))
                        .MapAsync(async ss => ss.Concat(await GetNameByIdAsync("Funk123"))).GetAwaiter().GetResult();
                },
                s => Assert.Equal("HarunHarun", s.UnsafeGetFirst())
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

        private static async Task<string> GetNullStringAsync()
        {
            return await Task.FromResult(GetNullString());
        }

        private static async Task<string> GetNameByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new InvalidOperationException("Id cannot be null.");
            }
            if (id.SafeEquals("Funk123"))
            {
                return await Task.FromResult("Harun");
            }
            throw new ArgumentException("Invalid id");
        }
    }

    public class InformationSource : OneOf<FirstSource, SecondSource>
    {
        private InformationSource()
        {
        }

        public InformationSource(FirstSource first)
            : base(first)
        {
        }

        public InformationSource(SecondSource second)
            : base(second)
        {
        }

        public static implicit operator InformationSource(Unit unit) => new InformationSource();
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
