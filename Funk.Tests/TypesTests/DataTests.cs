using System;
using Xunit;
using static Funk.Prelude;

namespace Funk.Tests
{
    public class DataTests : Test
    {
        [Fact]
        public void Create()
        {
            UnitTest(
                _ => Customer.New,
                c => c
                    .With(cc => cc.Name, "John")
                    .WithBuild(cc => cc.Age, 40),
                c =>
                {
                    Assert.Equal("John", c.Name);
                    Assert.Equal(40, c.Age);
                }
            );
        }

        [Fact]
        public void Create_Failure()
        {
            UnitTest(
                _ => Customer.New,
                c => act(() => c
                    .With(cc => cc.Name, "John")
                    .With(cc => cc.Account2.Number, 1234567890)
                    .Build()
                ),
                a => Assert.Throws<EmptyValueException>(a)
            );
        }

        [Fact]
        public void Update()
        {
            UnitTest(
                _ => Customer.New
                    .With(cc => cc.Name, "John")
                    .With(cc => cc.Age, 40)
                    .With(cc => cc.Account, new Account
                    {
                        Number = 1234567890,
                        CreditCard = new CreditCard
                        {
                            ExpirationDate = DateTime.Parse("12-12-2021")
                        }
                    })
                    .Build(),
                c =>
                {
                    var middle = c
                        .WithBuild(cc => cc.Age, 35);
                    var updated = middle    
                        .With(cc => cc.Surname, "Doe")
                        .WithBuild(cc => cc.Account.CreditCard.Contract, new Contract
                        {
                            Document = "Example"
                        })
                        .WithBuild(cc => cc.Account2, new Account
                        {
                            Number = 1234567891,
                            CreditCard = new CreditCard
                            {
                                ExpirationDate = DateTime.Parse("12-12-2022")
                            }
                        })
                        .WithBuild(u => u.Account2.Amount, 200);
                    return (c, updated);
                },
                c =>
                {
                    Assert.NotSame(c.c.Account, c.updated.Account);
                    Assert.Equal("Example", c.updated.Account.CreditCard.Contract.Document);
                    Assert.Null(c.c.Account.CreditCard.Contract);
                    Assert.Equal("Doe", c.updated.Surname);
                    Assert.Equal(35, c.updated.Age);
                }
            );
        }
    }

    public class Customer : Data<Customer>
    {
        private Customer()
        {
        }

        public Account Account { get; private set; }

        public readonly Account Account2;

        public string Name { get; private set; }

        public int Age { get; private set; }

        public readonly string Surname;

        public static Customer New => new Customer();
    }

    public class Account
    {
        public int Number { get; set; }

        public readonly int Amount;

        public CreditCard CreditCard { get; set; }
    }

    public class CreditCard
    {
        public DateTime ExpirationDate { get; set; }

        public Contract Contract { get; set; }
    }

    public class Contract
    {
        public string Document { get; set; }
    }
}