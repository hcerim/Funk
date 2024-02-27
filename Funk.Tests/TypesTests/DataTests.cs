using System;
using Funk;
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
                        .With(cc => cc.FirstName, "Harun")
                        .With(cc => cc.LastName, "Cerim")
                        .Build();
                },
                c =>
                {
                    Assert.Equal("Harun", c.FirstName);
                    Assert.Equal("Cerim", c.LastName);
                }
            );
        }

        [Fact]
        public void Create_Failure()
        {
            UnitTest(
                _ => Customer.New,
                c => act(() => c
                    .With(cc => cc.FirstName, "Harun")
                    .With(cc => cc.Account.Number, 1234567890)
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
                    .With(cc => cc.FirstName, "John")
                    .With(cc => cc.LastName, "Doe")
                    .With(cc => cc.Account, new Account
                    {
                        Number = 1234567890,
                        Description = "Desc",
                        CreditCard = new CreditCard
                        {
                            ExpirationDate = DateTime.Parse("12-12-2021")
                        }
                    })
                    .Build(),
                c =>
                {
                    var middle = GetMiddle(c).Build();
                    middle.SetVersion(3);
                    var updated = middle
                        .With(cc => cc.LastName, "Doe")
                        .With(cc => cc.Account.CreditCard.Contract, new Contract
                        {
                            Document = "Example"
                        })
                        .With(
                            cc => cc.Account, new Account
                            {
                                Description = "Description",
                                Number = 1234567891,
                                CreditCard = new CreditCard
                                {
                                    ExpirationDate = DateTime.Parse("12-12-2022")
                                }
                            }
                        )
                        .Build();
                    return (c, updated);
                },
                c =>
                {
                    Assert.NotSame(c.c.Account, c.updated.Account);
                    Assert.Equal("Desc", c.c.Account.Description);
                    Assert.Equal("Description", c.updated.Account.Description);
                    Assert.Null(c.c.Account.CreditCard.Contract);
                    Assert.Equal(3, c.updated.Version);
                    Assert.Equal(DateTime.Parse("12-12-2022"), c.updated.Account.CreditCard.ExpirationDate);
                    Assert.Null(c.updated.Account.CreditCard.Contract);
                    Assert.Equal("Doe", c.updated.LastName);
                    Assert.Equal(1234567891, c.updated.Account.Number);
                    Assert.Equal(DateTime.Parse("12-12-2022"), c.updated.CreatedAt);
                }
            );
            return;
            
            static Builder<Customer> GetMiddle(Customer data) =>
                data.WithBuild(cc => cc.CreatedAt, (DateTime?)DateTime.Parse("12-12-2022"));
        }
    }
}

public sealed class Customer : Data<Customer>
{
    public Guid Id { get; private set; }
    public string EmailAddress { get; private set; }
    public bool EmailAddressVerified { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public Guid SubscriptionId { get; private set; }
    public Account Account { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime ModifiedAt { get; private set; }
    public Guid ModifiedBy { get; private set; }
    private int _version;

    public static Customer New => new();
    public void SetVersion(int version) => _version = version;
    public int Version => _version;
}

public class Account
{
    public int Number { get; set; }
        
    public string Description { get; set; }

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