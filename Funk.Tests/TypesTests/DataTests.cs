using System;
using Xunit;

namespace Funk.Tests
{
    public class DataTests : Test
    {
        [Fact]
        public void CreateCustomer()
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
        public void UpdateCustomer()
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
                c => c
                    .With(cc => cc.Age, 35),
                c =>
                {
                    Assert.Equal("John", c.Name);
                    Assert.Equal(35, c.Age);
                    Assert.Equal(123456789, c.Account.Number);
                    Assert.Equal(DateTime.Parse("12-12-2021"), c.Account.CreditCard.ExpirationDate);
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
        
        public string Name { get; private set; }

        public int Age { get; private set; }
        
        public static Customer New => new Customer();
    }

    public class Account
    {
        public int Number { get; set; }
        
        public CreditCard CreditCard { get; set; }
    }

    public class CreditCard
    {
        public DateTime ExpirationDate { get; set; }
    }
}