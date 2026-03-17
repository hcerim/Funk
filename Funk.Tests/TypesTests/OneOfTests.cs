using System;
using Xunit;

namespace Funk.Tests
{
    public class OneOfTests : Test
    {
        [Fact]
        public void Create_OneOf_3_Type_Check()
        {
            UnitTest(
                _ => new OneOf<string, int, double>("Funk"),
                a => a,
                a => Assert.IsType<OneOf<string, int, double>>(a)
            );
        }

        [Fact]
        public void Create_OneOf_3_With_First()
        {
            UnitTest(
                _ => new OneOf<string, int, double>("Funk"),
                a => a.UnsafeGetFirst(),
                s => Assert.Equal("Funk", s)
            );
        }

        [Fact]
        public void Create_OneOf_3_With_First_Get_Second_Implicit()
        {
            UnitTest(
                _ => new OneOf<string, int, double>("Funk"),
                a => act(() => a.UnsafeGetSecond()),
                a => Assert.Throws<EmptyValueException>(a)
            );
        }

        [Fact]
        public void Create_OneOf_3_With_First_Get_Second_Explicit()
        {
            UnitTest(
                _ => new OneOf<string, int, double>("Funk"),
                a => act(() => a.UnsafeGetSecond(_ => new Exception("Funk"))),
                a => Assert.Throws<Exception>(a)
            );
        }

        [Fact]
        public void Create_OneOf_2_With_First_Get_Second_Implicit()
        {
            UnitTest(
                _ => GetEmptyOneOf(),
                a => act(() => a.UnsafeGetSecond()),
                a => Assert.Throws<EmptyValueException>(a)
            );
        }

        [Fact]
        public void Match_On_Non_Empty_OneOf_2_With_Result()
        {
            UnitTest(
                _ => new User(new BasicInfo
                {
                    Name = "John Doe",
                    Age = 30
                }), 
                u => u.Match(
                    _ => "John",
                    i => i.Name,
                    b => b.Nationality
                ),
                s => Assert.Equal("John Doe", s)
            );
        }

        [Fact]
        public void Match_On_Empty_OneOf_2_With_Result()
        {
            UnitTest(
                _ => new User(default(Biography)),
                u => u.Match(
                    _ => "John",
                    i => i.Name,
                    b => b.Nationality
                ),
                s => Assert.Equal("John", s)
            );
        }

        [Fact]
        public void Match_On_Empty_OneOf_2_With_Exception_Explicit()
        {
            UnitTest(
                _ => GetEmptyUser(),
                u => act(() => u.Match(
                    i => i.Name,
                    b => b.Nationality,
                    _ => new Exception("Funk")
                )),
                a => Assert.Throws<Exception>(a)
            );
        }

        [Fact]
        public void Match_On_Empty_OneOf_2_With_Exception_Implicit()
        {
            UnitTest(
                _ => GetEmptyUser(),
                u => act(() => u.Match(
                    i => i.Name,
                    b => b.Nationality
                )),
                a => Assert.Throws<EmptyValueException>(a)
            );
        }

        [Fact]
        public void Match_On_Empty_OneOf_2_With_Nullable_Value_Empty()
        {
            UnitTest(
                _ => new OneOf<int?, double>(null),
                a => a.Match(
                    _ => 1,
                    n=> n.Value,
                    n => 1.1
                ),
                n => Assert.Equal(1, n)
            );
        }

        [Fact]
        public void Match_On_Empty_OneOf_2_With_Nullable_Value()
        {
            UnitTest(
                _ =>
                {
                    int? number = 3;
                    return new OneOf<int?, double>(number);
                },
                a => a.Match(
                    _ => 1,
                    n => n.Value,
                    n => 1.1
                ),
                n => Assert.Equal(3, n)
            );
        }

        [Fact]
        public void Match_On_Empty_OneOf_2_With_Exception()
        {
            UnitTest(
                _ => GetEmptyUser(),
                u => act(() => u.Match(
                    _ => throw new Exception("Funk"),
                    i => { },
                    b => { }
                )),
                a => Assert.Throws<Exception>(a)
            );
        }

        [Fact]
        public void Match_On_Empty_OneOf_2_Empty_Check_Unsafe()
        {
            UnitTest(
                _ => new User(new Biography
                {
                    Nationality = "Bosnian"
                }),
                u => u.NotEmpty.Match(
                    _ => throw new Exception("Funk"),
                    _ => u.UnsafeGetSecond().Nationality
                ),
                s => Assert.Equal("Bosnian", s)
            );
        }

        [Fact]
        public void Match_On_Empty_OneOf_2_Empty_Check_Safe()
        {
            UnitTest(
                _ => new User(new Biography
                {
                    Nationality = "Bosnian"
                }),
                u => u.NotEmpty.Match(
                    _ => throw new Exception("Funk"),
                    _ => u.Match(
                        i => i.Name,
                        b => b.Nationality
                    )
                ),
                s => Assert.Equal("Bosnian", s)
            );
        }

        [Fact]
        public void Match_On_Empty_OneOf_2_Check_Empty_First_And_Second()
        {
            UnitTest(
                _ => new User(default(BasicInfo)),
                u => u.Info.IsEmpty && u.Bio.IsEmpty,
                Assert.True
            );
        }

        [Fact]
        public void Match_On_Empty_OneOf_2_Get_Second_Empty()
        {
            UnitTest(
                _ => new OneOf<int, int>(first:2), 
                u => act(() => u.Second.UnsafeGet()),
                a => Assert.Throws<EmptyValueException>(a)
            );
        }

        [Fact]
        public void Match_On_Empty_OneOf_2_Get_Second()
        {
            UnitTest(
                _ => new OneOf<int, int>(second: 2),
                u => u.Second.UnsafeGet(),
                i => Assert.Equal(2, i)
            );
        }

        [Fact]
        public void Match_On_Empty_OneOf_2_Not_Check_Empty_Second()
        {
            UnitTest(
                _ => new User(new Biography
                {
                    Nationality = "Bosnian"
                }),
                u => u.Bio.UnsafeGet().Nationality,
                s => Assert.Equal("Bosnian", s)
            );
        }

        // OneOf<T1,T2,T3,T4> tests

        [Fact]
        public void Create_OneOf_4_With_First()
        {
            UnitTest(
                _ => new OneOf<string, int, double, bool>("Funk"),
                a => a,
                a =>
                {
                    Assert.True(a.IsFirst);
                    Assert.False(a.IsSecond);
                    Assert.False(a.IsThird);
                    Assert.False(a.IsFourth);
                    Assert.Equal("Funk", a.UnsafeGetFirst());
                }
            );
        }

        [Fact]
        public void Create_OneOf_4_With_Fourth()
        {
            UnitTest(
                _ => new OneOf<string, int, double, bool>(true),
                a => a,
                a =>
                {
                    Assert.False(a.IsFirst);
                    Assert.True(a.IsFourth);
                    Assert.True(a.UnsafeGetFourth());
                }
            );
        }

        [Fact]
        public void Match_On_OneOf_4_All_Cases()
        {
            UnitTest(
                _ => new OneOf<string, int, double, bool>(3.14),
                a => a.Match(
                    _ => 0.0,
                    s => 0.0,
                    i => (double)i,
                    d => d,
                    b => 0.0
                ),
                d => Assert.Equal(3.14, d)
            );
        }

        [Fact]
        public void Match_On_Empty_OneOf_4_Throws()
        {
            UnitTest(
                _ => new OneOf<string, int, double, bool>(),
                a => act(() => a.Match(
                    s => s,
                    i => i.ToString(),
                    d => d.ToString(),
                    b => b.ToString()
                )),
                a => Assert.Throws<EmptyValueException>(a)
            );
        }

        [Fact]
        public void OneOf_4_Equality()
        {
            UnitTest(
                _ => new OneOf<string, int, double, bool>(42),
                a => a == new OneOf<string, int, double, bool>(42),
                Assert.True
            );
        }

        [Fact]
        public void OneOf_4_Inequality()
        {
            UnitTest(
                _ => new OneOf<string, int, double, bool>(42),
                a => a != new OneOf<string, int, double, bool>(43),
                Assert.True
            );
        }

        [Fact]
        public void OneOf_4_UnsafeGet_Wrong_State_Throws()
        {
            UnitTest(
                _ => new OneOf<string, int, double, bool>("Funk"),
                a => act(() => a.UnsafeGetFourth()),
                a => Assert.Throws<EmptyValueException>(a)
            );
        }

        [Fact]
        public void OneOf_4_Deconstruct()
        {
            UnitTest(
                _ => new OneOf<string, int, double, bool>(42),
                a =>
                {
                    var (first, second, third, fourth) = a;
                    return (first, second, third, fourth);
                },
                r =>
                {
                    Assert.True(r.first.IsEmpty);
                    Assert.True(r.second.NotEmpty);
                    Assert.Equal(42, r.second.UnsafeGet());
                    Assert.True(r.third.IsEmpty);
                    Assert.True(r.fourth.IsEmpty);
                }
            );
        }

        [Fact]
        public void OneOf_4_Match_Action()
        {
            UnitTest(
                _ => new OneOf<string, int, double, bool>(true),
                a =>
                {
                    var captured = false;
                    a.Match(ifFourth: b => captured = b);
                    return captured;
                },
                Assert.True
            );
        }

        [Fact]
        public void OneOf_4_ToString()
        {
            UnitTest(
                _ => new OneOf<string, int, double, bool>("Funk"),
                a => a.ToString(),
                s => Assert.Equal("Funk", s)
            );
        }

        // OneOf<T1,T2,T3,T4,T5> tests

        [Fact]
        public void Create_OneOf_5_With_Fifth()
        {
            UnitTest(
                _ => new OneOf<string, int, double, bool, char>('F'),
                a => a,
                a =>
                {
                    Assert.False(a.IsFirst);
                    Assert.False(a.IsSecond);
                    Assert.False(a.IsThird);
                    Assert.False(a.IsFourth);
                    Assert.True(a.IsFifth);
                    Assert.Equal('F', a.UnsafeGetFifth());
                }
            );
        }

        [Fact]
        public void Create_OneOf_5_With_Third()
        {
            UnitTest(
                _ => new OneOf<string, int, double, bool, char>(2.71),
                a => a,
                a =>
                {
                    Assert.True(a.IsThird);
                    Assert.Equal(2.71, a.UnsafeGetThird());
                }
            );
        }

        [Fact]
        public void Match_On_OneOf_5_All_Cases()
        {
            UnitTest(
                _ => new OneOf<string, int, double, bool, char>('X'),
                a => a.Match(
                    _ => "empty",
                    s => s,
                    i => i.ToString(),
                    d => d.ToString(),
                    b => b.ToString(),
                    c => c.ToString()
                ),
                s => Assert.Equal("X", s)
            );
        }

        [Fact]
        public void Match_On_Empty_OneOf_5_Throws()
        {
            UnitTest(
                _ => new OneOf<string, int, double, bool, char>(),
                a => act(() => a.Match(
                    s => s,
                    i => i.ToString(),
                    d => d.ToString(),
                    b => b.ToString(),
                    c => c.ToString()
                )),
                a => Assert.Throws<EmptyValueException>(a)
            );
        }

        [Fact]
        public void OneOf_5_Equality()
        {
            UnitTest(
                _ => new OneOf<string, int, double, bool, char>('A'),
                a => a == new OneOf<string, int, double, bool, char>('A'),
                Assert.True
            );
        }

        [Fact]
        public void OneOf_5_Empty_Equality()
        {
            UnitTest(
                _ => new OneOf<string, int, double, bool, char>(),
                a => a == new OneOf<string, int, double, bool, char>(),
                Assert.True
            );
        }

        [Fact]
        public void OneOf_5_Deconstruct()
        {
            UnitTest(
                _ => new OneOf<string, int, double, bool, char>('Z'),
                a =>
                {
                    var (first, second, third, fourth, fifth) = a;
                    return (first, second, third, fourth, fifth);
                },
                r =>
                {
                    Assert.True(r.first.IsEmpty);
                    Assert.True(r.second.IsEmpty);
                    Assert.True(r.third.IsEmpty);
                    Assert.True(r.fourth.IsEmpty);
                    Assert.True(r.fifth.NotEmpty);
                    Assert.Equal('Z', r.fifth.UnsafeGet());
                }
            );
        }

        [Fact]
        public void OneOf_5_Match_Action()
        {
            UnitTest(
                _ => new OneOf<string, int, double, bool, char>('A'),
                a =>
                {
                    var captured = ' ';
                    a.Match(ifFifth: c => captured = c);
                    return captured;
                },
                c => Assert.Equal('A', c)
            );
        }

        [Fact]
        public void OneOf_5_UnsafeGet_Wrong_State_Throws()
        {
            UnitTest(
                _ => new OneOf<string, int, double, bool, char>("Funk"),
                a => act(() => a.UnsafeGetFifth()),
                a => Assert.Throws<EmptyValueException>(a)
            );
        }

        [Fact]
        public void OneOf_5_ToString()
        {
            UnitTest(
                _ => new OneOf<string, int, double, bool, char>(42),
                a => a.ToString(),
                s => Assert.Equal("42", s)
            );
        }

        [Fact]
        public void OneOf_5_Implicit_Conversion()
        {
            UnitTest(
                _ =>
                {
                    OneOf<string, int, double, bool, char> oneOf = "implicit";
                    return oneOf;
                },
                a => a.UnsafeGetFirst(),
                s => Assert.Equal("implicit", s)
            );
        }

        [Fact]
        public void OneOf_4_Implicit_Conversion()
        {
            UnitTest(
                _ =>
                {
                    OneOf<string, int, double, bool> oneOf = 3.14;
                    return oneOf;
                },
                a => a.UnsafeGetThird(),
                d => Assert.Equal(3.14, d)
            );
        }

        private static OneOf<string, int> GetEmptyOneOf()
        {
            return empty;
        }

        private static User GetEmptyUser()
        {
            return new User();
        }
    }

    public class User : OneOf<BasicInfo, Biography>
    {
        public User()
        {
        }

        public User(BasicInfo info)
            : base(info)
        {
        }

        public User(Biography bio)
            : base(bio)
        {
        }

        public Maybe<BasicInfo> Info => First;
        public Maybe<Biography> Bio => Second;
    }

    public class BasicInfo
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class Biography
    {
        public string Nationality { get; set; }
    }
}
