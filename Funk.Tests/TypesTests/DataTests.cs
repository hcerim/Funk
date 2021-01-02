using System;
using Xunit;

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
                    .With(cc => cc.Age, 40),
                c =>
                {
                    Assert.Equal("John", c.Name);
                    Assert.Equal(40, c.Age);
                }
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
                        Number = 123456789,
                        CreditCard = new CreditCard
                        {
                            ExpirationDate = DateTime.Parse("12-12-2021")
                        }
                    }),
                c =>
                {
                    var updated = c
                        .With(cc => cc.Age, 35)
                        .With(cc => cc.Surname, "Doe")
                        .With(u => u.Account.CreditCard.Contract, new Contract
                        {
                            Document = "Example"
                        })
                        .With(u => u.Account2.CreditCard.Contract, new Contract
                        {
                            Document = "Example"
                        });
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

        public readonly DateTime Created;
        
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