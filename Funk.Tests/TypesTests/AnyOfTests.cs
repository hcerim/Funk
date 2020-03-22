using System;
using Funk.Exceptions;
using Xunit;
using static Funk.Prelude;

namespace Funk.Tests
{
    public class AnyOfTests : Test
    {
        [Fact]
        public void Create_AnyOf_3_Type_Check()
        {
            UnitTest(
                _ => new AnyOf<string, int, double>("Funk"),
                a => a,
                a => Assert.IsType<AnyOf<string, int, double>>(a)
            );
        }

        [Fact]
        public void Create_AnyOf_3_With_First()
        {
            UnitTest(
                _ => new AnyOf<string, int, double>("Funk"),
                a => a.UnsafeGetFirst(),
                s => Assert.Equal("Funk", s)
            );
        }

        [Fact]
        public void Create_AnyOf_3_With_First_Get_Second_Implicit()
        {
            UnitTest(
                _ => new AnyOf<string, int, double>("Funk"),
                a => act(() => a.UnsafeGetSecond()),
                a => Assert.Throws<EmptyValueException>(a)
            );
        }

        [Fact]
        public void Create_AnyOf_3_With_First_Get_Second_Explicit()
        {
            UnitTest(
                _ => new AnyOf<string, int, double>("Funk"),
                a => act(() => a.UnsafeGetSecond(_ => new Exception("Funk"))),
                a => Assert.Throws<Exception>(a)
            );
        }

        [Fact]
        public void Create_AnyOf_2_With_First_Get_Second_Implicit()
        {
            UnitTest(
                _ => new AnyOf<string, int>(null),
                a => act(() => a.UnsafeGetSecond()),
                a => Assert.Throws<EmptyValueException>(a)
            );
        }

        [Fact]
        public void Match_On_Non_Empty_AnyOf_2_With_Result()
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
        public void Match_On_Empty_AnyOf_2_With_Result()
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
        public void Match_On_Empty_AnyOf_2_With_Exception_Explicit()
        {
            UnitTest(
                _ => new User(default(Biography)),
                u => act(() => u.Match(
                    i => i.Name,
                    b => b.Nationality,
                    _ => new Exception("Funk")
                )),
                a => Assert.Throws<Exception>(a)
            );
        }

        [Fact]
        public void Match_On_Empty_AnyOf_2_With_Exception_Implicit()
        {
            UnitTest(
                _ => new User(default(Biography)),
                u => act(() => u.Match(
                    i => i.Name,
                    b => b.Nationality
                )),
                a => Assert.Throws<EmptyValueException>(a)
            );
        }

        [Fact]
        public void Match_On_Empty_AnyOf_2_With_Exception()
        {
            UnitTest(
                _ => new User(default(Biography)),
                u => act(() => u.Match(
                    _ => throw new Exception("Funk"),
                    i => { },
                    b => { }
                )),
                a => Assert.Throws<Exception>(a)
            );
        }
    }

    public class User : AnyOf<BasicInfo, Biography>
    {
        public User(BasicInfo t1)
            : base(t1)
        {
        }

        public User(Biography t2)
            : base(t2)
        {
        }
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
