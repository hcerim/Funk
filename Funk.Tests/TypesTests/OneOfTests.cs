using System;
using Funk.Exceptions;
using Xunit;
using static Funk.Prelude;

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

        private static OneOf<string, int> GetEmptyOneOf()
        {
            return Empty;
        }

        private static User GetEmptyUser()
        {
            return Empty;
        }
    }

    public class User : OneOf<BasicInfo, Biography>
    {
        private User()
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

        public static implicit operator User(Unit unit) => new User();
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
