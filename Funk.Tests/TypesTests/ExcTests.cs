using System;
using System.Threading.Tasks;
using Funk.Exceptions;
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
                    Assert.Empty(f.UnsafeGet());
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
                        .RecoverOnFailure(e => GetNameById("Funk123"));
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
                        .RecoverOnFailure(e => GetNameById("Funk1"))
                        .RecoverOnFailure(e => GetNameById("Funk123"));
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
                        .RecoverOnFailure(e => GetNameById("Funk1"))
                        .RecoverOnFailure(e => GetNameById("Funk12"));
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
                        .RecoverOnFailure(e => GetNameById(null)));
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
                        var result = Exc.Create<string, ArgumentException>(_ => GetNameByIdAsync(s));
                        result.RecoverOnFailure(e => GetNameByIdAsync(null)).GetAwaiter().GetResult();
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
                    return fun(() =>
                    {
                        var result = Exc.Create<string, ArgumentException>(_ => GetNameByIdAsync(s)).GetAwaiter().GetResult();
                        return result.RecoverOnFailure(e => GetNullString()).RecoverOnEmpty(_ => GetNameById("Funk123"));
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
        public void Create_Exceptional_Throws_Recover_On_Empty_Nothing()
        {
            UnitTest(
                _ => "Funk12",
                s =>
                {
                    return fun(() =>
                    {
                        var result = Exc.Create<string, ArgumentException>(_ => GetNameByIdAsync(s)).GetAwaiter().GetResult();
                        return result.RecoverOnFailure(e => GetNameById("Funk12")).RecoverOnEmpty(_ => GetNameById("Funk123"));
                    });
                },
                f =>
                {
                    var result = f.Invoke();
                    Assert.IsType<EnumerableException<ArgumentException>>(result.Failure.UnsafeGet());
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

        private static async Task<string> GetNameByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new InvalidOperationException("Id cannot be null.");
            }
            if (id.SafeEquals("Funk123"))
            {
                return await Task.Run(() => "Harun");
            }
            throw new ArgumentException("Invalid id");
        }
    }
}
