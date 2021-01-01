using Xunit;
using static Funk.Prelude;

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
                    (1, _ => "Funk"),
                    (2, _ => "Harun"),
                    (3, _ => "Funky"),
                    (6, _ => "Fun"),
                    (7, _ => "Funny"),
                    (3, _ => "F"),
                    (9, _ => "FP")
                ),
                s => Assert.Equal("Funny", s)
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
    }
}
