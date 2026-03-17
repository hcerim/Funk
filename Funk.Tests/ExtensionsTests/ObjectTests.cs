using System;
using System.Threading.Tasks;
using Xunit;

namespace Funk.Tests
{
    public class ObjectTests : Test
    {
        [Fact]
        public void SafeCast_False()
        {
            UnitTest(
                _ => ("Harun", 24),
                r => r.SafeCast<Record<string, int>>(),
                m => Assert.True(m.IsEmpty)
            );
        }

        [Fact]
        public void SafeCast_True()
        {
            UnitTest(
                _ => (object)rec("Harun", 24),
                r => r.SafeCast<Record<string, int>>(),
                m => Assert.True(m.NotEmpty)
            );
        }

        [Fact]
        public void Match_On_Int_For_Result()
        {
            UnitTest(
                _ => 7,
                i => i.Match(
                    (list(1, 2, 3), _ => "Funk"),
                    (list(3, 4, 7), _ => "Funky"),
                    (list(5, 7, 3), _ => "Funny")
                ),
                s => Assert.Equal("Funky", s)
            );
        }

        [Fact]
        public void Match_On_String_For_Result()
        {
            UnitTest(
                _ => "Bosnia",
                i => i.Match(
                    (c => c == "Bosnia", _ => "Hello Funk"),
                    (c => c == "Hello", _ => "Funky"),
                    (c => c.StartsWith("B"), _ => "Funk")
                ),
                s => Assert.Equal("Hello Funk", s)
            );
        }

        [Fact]
        public void Match_On_String_For_Result_Throws()
        {
            UnitTest(
                _ => "Bosnia",
                i => act(() => i.Match(
                    "Harun", _ => "Funk",
                    "Funk", _ => "Harun"
                )),
                a => Assert.Throws<UnhandledValueException>(a)
            );
        }

        [Fact]
        public void Do_Action_Returns_Self()
        {
            UnitTest(
                _ => "Funk",
                s =>
                {
                    string captured = null;
                    var returned = s.Do(v => captured = v);
                    return (returned, captured);
                },
                r =>
                {
                    Assert.Equal("Funk", r.returned);
                    Assert.Equal("Funk", r.captured);
                }
            );
        }

        [Fact]
        public void Do_Function_Returns_Result()
        {
            UnitTest(
                _ => "Funk",
                s => s.Do(v => v.Length),
                r => Assert.Equal(4, r)
            );
        }

        [Fact]
        public async Task DoAsync_Action_Returns_Self()
        {
            await UnitTestAsync(
                _ => result("Funk"),
                async s =>
                {
                    var returned = await s.DoAsync(v => Task.CompletedTask);
                    return returned;
                },
                r => Assert.Equal("Funk", r)
            );
        }

        [Fact]
        public async Task DoAsync_Task_Action_Returns_Self()
        {
            await UnitTestAsync(
                _ => result("Funk"),
                s => s.DoAsync(v => Task.CompletedTask),
                r => Assert.Equal("Funk", r)
            );
        }

        [Fact]
        public async Task DoAsync_Function_Returns_Result()
        {
            await UnitTestAsync(
                _ => result("Funk"),
                s => s.DoAsync(v => result(v.Length)),
                r => Assert.Equal(4, r)
            );
        }

        [Fact]
        public async Task DoAsync_Task_Function_Returns_Result()
        {
            await UnitTestAsync(
                _ => result(5),
                s => s.DoAsync(v => result(v * 2)),
                r => Assert.Equal(10, r)
            );
        }

        [Fact]
        public void Match_Value_With_Otherwise()
        {
            UnitTest(
                _ => "Unknown",
                i => i.Match(
                    "Harun", _ => "Funk",
                    "Funk", _ => "Harun",
                    otherwise: _ => "Fallback"
                ),
                s => Assert.Equal("Fallback", s)
            );
        }

        [Fact]
        public void Match_Predicate_1_Hit()
        {
            UnitTest(
                _ => 10,
                i => i.Match(
                    v => v > 5, _ => "Big"
                ),
                s => Assert.Equal("Big", s)
            );
        }

        [Fact]
        public void Match_Predicate_No_Hit_With_Otherwise()
        {
            UnitTest(
                _ => 3,
                i => i.Match(
                    v => v > 5, _ => "Big",
                    otherwise: _ => "Small"
                ),
                s => Assert.Equal("Small", s)
            );
        }

        [Fact]
        public void Match_Value_Action_Executes()
        {
            UnitTest(
                _ => "Funk",
                i =>
                {
                    string captured = null;
                    i.Match("Funk", v => captured = v);
                    return captured;
                },
                s => Assert.Equal("Funk", s)
            );
        }

        [Fact]
        public void Match_Value_Action_Otherwise_Executes()
        {
            UnitTest(
                _ => "Unknown",
                i =>
                {
                    var hit = false;
                    i.Match("Funk", _ => { }, otherwise: _ => hit = true);
                    return hit;
                },
                Assert.True
            );
        }

        [Fact]
        public void Match_Predicate_Action_Executes()
        {
            UnitTest(
                _ => 42,
                i =>
                {
                    int captured = 0;
                    i.Match(v => v > 40, v => captured = v);
                    return captured;
                },
                c => Assert.Equal(42, c)
            );
        }

        [Fact]
        public void Match_Collection_No_Match_Returns_Empty_Maybe()
        {
            UnitTest(
                _ => 99,
                i => i.Match(
                    (list(1, 2, 3), _ => "Found")
                ),
                m => Assert.True(m.IsEmpty)
            );
        }

        [Fact]
        public void Match_Value_Params_Returns_Maybe()
        {
            UnitTest(
                _ => "Funk",
                i => i.Match(
                    ("Funk", _ => 42)
                ),
                m =>
                {
                    Assert.True(m.NotEmpty);
                    Assert.Equal(42, m.UnsafeGet());
                }
            );
        }

        [Fact]
        public void Match_Value_Params_No_Hit_Returns_Empty_Maybe()
        {
            UnitTest(
                _ => "Other",
                i => i.Match(
                    ("Funk", _ => 42)
                ),
                m => Assert.True(m.IsEmpty)
            );
        }

        [Fact]
        public void Match_Predicate_Params_Returns_Maybe()
        {
            UnitTest(
                _ => 10,
                i => i.Match(
                    ((Func<int, bool>)(v => v < 5), _ => "Small"),
                    ((Func<int, bool>)(v => v >= 5), _ => "Big")
                ),
                m =>
                {
                    Assert.True(m.NotEmpty);
                    Assert.Equal("Big", m.UnsafeGet());
                }
            );
        }

        [Fact]
        public void Match_OtherwiseThrow_Custom_Exception()
        {
            UnitTest(
                _ => "Bosnia",
                i => act(() => i.Match(
                    "X", _ => "Funk",
                    otherwiseThrow: _ => new InvalidOperationException("custom")
                )),
                a => Assert.Throws<InvalidOperationException>(a)
            );
        }

        [Fact]
        public void Match_3_Values()
        {
            UnitTest(
                _ => "C",
                i => i.Match(
                    "A", _ => 1,
                    "B", _ => 2,
                    "C", _ => 3
                ),
                r => Assert.Equal(3, r)
            );
        }

        [Fact]
        public void SafeCast_Null_Returns_Empty()
        {
            UnitTest(object (_) => null,
                r => r.SafeCast<string>(),
                m => Assert.True(m.IsEmpty)
            );
        }

        [Fact]
        public void SafeCast_Base_To_Derived_Fails()
        {
            UnitTest(object (_) => new Exception("base"),
                r => r.SafeCast<InvalidOperationException>(),
                m => Assert.True(m.IsEmpty)
            );
        }

        [Fact]
        public void SafeCast_Derived_To_Base_Succeeds()
        {
            UnitTest(object (_) => new InvalidOperationException("derived"),
                r => r.SafeCast<Exception>(),
                m =>
                {
                    Assert.True(m.NotEmpty);
                    Assert.Equal("derived", m.UnsafeGet().Message);
                }
            );
        }

        [Fact]
        public void Match_Collection_Action_Executes()
        {
            UnitTest(
                _ => 3,
                i =>
                {
                    var result = i.Match(
                        (list(1, 2, 3), v => { })
                    );
                    return result;
                },
                m => Assert.True(m.NotEmpty)
            );
        }
    }
}
