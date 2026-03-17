using System;
using Funk;
using Xunit;

namespace Funk.Tests
{
    public class DataTests : Test
    {
        [Fact]
        public void Create()
        {
            UnitTest(
                _ => NewCustomer.New,
                c =>
                {
                    return c
                        .With(cc => cc.FirstName, "Harun")
                        .With(cc => cc.MiddleName, "Ahmed")
                        .With(cc => cc.LastName, "Cerim")
                        .Build();
                },
                c =>
                {
                    Assert.Equal("Harun", c.FirstName);
                    Assert.Equal("Ahmed", c.MiddleName);
                    Assert.Equal("Cerim", c.LastName);
                }
            );
        }

        [Fact]
        public void Create_Failure()
        {
            UnitTest(
                _ => NewCustomer.New,
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
                _ => NewCustomer.New
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

            static Builder<NewCustomer> GetMiddle(NewCustomer data) =>
                data.WithBuild(cc => cc.CreatedAt, (DateTime?)DateTime.Parse("12-12-2022"));
        }

        [Fact]
        public void Deep_Copy_Preserves_Original()
        {
            UnitTest(
                _ => NewCustomer.New
                    .With(c => c.FirstName, "John")
                    .With(c => c.LastName, "Doe")
                    .With(c => c.Account, new Account
                    {
                        Number = 100,
                        Description = "Original",
                        CreditCard = new CreditCard
                        {
                            ExpirationDate = DateTime.Parse("01-01-2025")
                        }
                    })
                    .Build(),
                original =>
                {
                    var updated = original
                        .With(c => c.FirstName, "Jane")
                        .With(c => c.Account, new Account
                        {
                            Number = 200,
                            Description = "Updated",
                            CreditCard = new CreditCard
                            {
                                ExpirationDate = DateTime.Parse("01-01-2026")
                            }
                        })
                        .Build();
                    return (original, updated);
                },
                r =>
                {
                    Assert.Equal("John", r.original.FirstName);
                    Assert.Equal("Jane", r.updated.FirstName);
                    Assert.Equal(100, r.original.Account.Number);
                    Assert.Equal(200, r.updated.Account.Number);
                    Assert.NotSame(r.original, r.updated);
                    Assert.NotSame(r.original.Account, r.updated.Account);
                }
            );
        }

        [Fact]
        public void Multiple_Builds_From_Same_Source()
        {
            UnitTest(
                _ => NewCustomer.New
                    .With(c => c.FirstName, "Base")
                    .Build(),
                source =>
                {
                    var v1 = source.With(c => c.LastName, "Alpha").Build();
                    var v2 = source.With(c => c.LastName, "Beta").Build();
                    return (source, v1, v2);
                },
                r =>
                {
                    Assert.Equal("Base", r.source.FirstName);
                    Assert.Equal("Alpha", r.v1.LastName);
                    Assert.Equal("Beta", r.v2.LastName);
                    Assert.Equal("Base", r.v1.FirstName);
                    Assert.Equal("Base", r.v2.FirstName);
                }
            );
        }

        [Fact]
        public void Update_With_Null_Account()
        {
            UnitTest(
                _ => NewCustomer.New
                    .With(c => c.FirstName, "John")
                    .With(c => c.Account, new Account { Number = 1 })
                    .Build(),
                original =>
                {
                    var updated = original
                        .With(c => c.Account, (Account)null)
                        .Build();
                    return (original, updated);
                },
                r =>
                {
                    Assert.NotNull(r.original.Account);
                    Assert.Null(r.updated.Account);
                }
            );
        }

        [Fact]
        public void Build_With_Only_Defaults()
        {
            UnitTest(
                _ => NewCustomer.New
                    .With(c => c.FirstName, (string)null)
                    .Build(),
                c => c,
                c =>
                {
                    Assert.Null(c.FirstName);
                    Assert.Null(c.LastName);
                    Assert.Null(c.Account);
                    Assert.Equal(Guid.Empty, c.Id);
                }
            );
        }
    }
}

public class NewCustomer : Customer<NewCustomer>
{
    public string MiddleName { get; private set; }
    public new static NewCustomer New => new();
}

public class Customer<T> : Data<T> where T : Customer<T>
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

    public static Customer<T> New => new();
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