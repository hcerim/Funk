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
                c =>
                {
                    return c
                        .With(
                            (cc => cc.Name, "John"),
                            (cc => cc.Age, 40)
                        )
                        .Build();
                },
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
                        Description = "Desc",
                        CreditCard = new CreditCard
                        {
                            ExpirationDate = DateTime.Parse("12-12-2021")
                        }
                    })
                    .Build().GetUpdated(),
                c =>
                {
                    var middle = GetMiddle(c).Build();
                    var updated = middle
                        .With(cc => cc.Surname, "Doe")
                        .With(cc => cc.Account.CreditCard.Contract, new Contract
                        {
                            Document = "Example"
                        })
                        .WithBuild(
                            (cc => cc.Account2, new Account
                            {
                                Description = "Desc",
                                Number = 1234567891,
                                CreditCard = new CreditCard
                                {
                                    ExpirationDate = DateTime.Parse("12-12-2022")
                                }
                            }),
                            (u => u.Account2.Amount, 200)
                        );
                    return (c, updated);
                },
                c =>
                {
                    Assert.NotSame(c.c.Account, c.updated.Account);
                    Assert.Null(c.updated.Account.Description);
                    Assert.Equal("Desc", c.updated.Account2.Description);
                    Assert.Null(c.c.Account.CreditCard.Contract);
                    Assert.Null(c.updated.Account.CreditCard.Contract.Document);
                    Assert.Equal("Doe", c.updated.Surname);
                    Assert.Equal(35, c.updated.Age);
                    Assert.True(c.c.PrivateEqual(c.updated));
                }
            );

            // implicit conversion
            static Builder<Customer> GetMiddle(Customer data) => data.WithBuild(cc => cc.Age, 35);
        }
    }

    public class Customer : Data<Customer>
    {
        public Customer GetUpdated() =>
            this.With(c => c.One, "One")
                .With(c => c.Two, "Two")
                .With(c => c.Three, "Three")
                .With(c => c.Four, "Four").Build();

        protected override void Configure()
        {
            Exclude(c => c.Account.Description);
            Exclude(c => c.Account.CreditCard.Contract.Document);
        }

        public bool PrivateEqual(Customer other) => One == other.One && Two == other.Two && Three == other.Three && Four == other.Four;

        public Account Account { get; private set; }

        public readonly Account Account2;

        private string One;

        protected string Two { private get; set; }
        
        internal string Three { get; private set; }

        protected string Four
        {
            get => One;
            private set => One = value;
        }

        public string Name { get; private set; }

        public int Age { get; private set; }

        public readonly string Surname;

        public static Customer New => new Customer();
    }

    public class Account
    {
        public int Number { get; set; }
        
        public string Description { get; set; }

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