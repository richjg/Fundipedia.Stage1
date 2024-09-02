using Fundipedia.TechnicalInterview.Data.Context;
using Fundipedia.TechnicalInterview.Domain;
using Fundipedia.TechnicalInterview.Model.Supplier;
using Microsoft.EntityFrameworkCore;

namespace Fundipedia.TechnicalInterview.DomainTests
{
    public class SupplierServicesTests
    {
        public SupplierService SupplierService(Action<SupplierContext>? seedData = null)
        {
            var options = new DbContextOptionsBuilder<SupplierContext>()
                              .UseInMemoryDatabase(databaseName: "SupplierDatabase", (c) => { })
                              .Options;

            using (var context = new SupplierContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                seedData?.Invoke(context);
                context.SaveChanges();
            }

            SupplierContextForAssert = new SupplierContext(options);
            var supplierContext = new SupplierContext(options);
            var supplierService = new SupplierService(SupplierContextForAssert);

            return supplierService;
        }
        private SupplierContext? SupplierContextForAssert;
        private void SeedSupplierData(SupplierContext context)
        {
            var suppliers = new List<Supplier>
                {
                    new ()
                    {
                        Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                        Title = "Mr",
                        FirstName = "Patrick",
                        LastName ="Star",
                        Emails =
                        {
                            new ()
                            {
                                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                                EmailAddress = "test2@test.com",
                                IsPreferred = false
                            }
                        },
                        Phones =
                        {
                            new ()
                            {
                                Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                                PhoneNumber = "09870987",
                                IsPreferred = false
                            }
                        }
                    },
                    new ()
                    {
                        Id = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                        Title = "Master",
                        FirstName = "Spongebob",
                        LastName ="Squarepants",
                        Emails =
                        {
                            new ()
                            {
                                Id = Guid.Parse("00000000-0000-0000-0000-000000000005"),
                                EmailAddress = "test1@test.com",
                                IsPreferred = true
                            },
                        },
                        Phones =
                        {
                            new ()
                            {
                                Id = Guid.Parse("00000000-0000-0000-0000-000000000006"),
                                PhoneNumber = "12341234",
                                IsPreferred = true
                            }
                        }
                    },
                };

            context.Suppliers.AddRange(suppliers);
        }

        #region Get

        [Test]
        public async Task GetSuppliersAsync_ReturnsEmpty_WhenNoSuppliers()
        {
            //Arrange
            var service = SupplierService();

            //Act
            var result = await service.GetSuppliersAsync();

            //Assert
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetSuppliersAsync_ReturnsSuppliers_WhenSuppliersExist()
        {
            //Arrange
            var service = SupplierService(SeedSupplierData);

            //Act
            var result = await service.GetSuppliersAsync();

            //Assert

            result = result.OrderBy(s => s.FirstName).ToList();
            Assert.That(result[0].Id, Is.EqualTo(Guid.Parse("00000000-0000-0000-0000-000000000001")));
            Assert.That(result[0].Title, Is.EqualTo("Mr"));
            Assert.That(result[0].FirstName, Is.EqualTo("Patrick"));
            Assert.That(result[0].LastName, Is.EqualTo("Star"));
            Assert.That(result[0].Emails.Count, Is.EqualTo(1));
            Assert.That(result[0].Emails.First().Id, Is.EqualTo(Guid.Parse("00000000-0000-0000-0000-000000000002")));
            Assert.That(result[0].Emails.First().EmailAddress, Is.EqualTo("test2@test.com"));
            Assert.That(result[0].Emails.First().IsPreferred, Is.EqualTo(false));
        }

        [Test]
        public async Task GetSupplierAsync_ReturnsNull_WhenSupplierNotFound()
        {
            //Arrange
            var service = SupplierService();

            //Act
            var result = await service.GetSupplierAsync(Guid.Empty);

            //Assert

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetSupplierAsync_ReturnsSupplier_WhenSupplierFound()
        {
            //Arrange
            var service = SupplierService(SeedSupplierData);

            //Act
            var result = await service.GetSupplierAsync(Guid.Parse("00000000-0000-0000-0000-000000000001"));

            //Assert
            Assert.That(result.Id, Is.EqualTo(Guid.Parse("00000000-0000-0000-0000-000000000001")));
            Assert.That(result.Title, Is.EqualTo("Mr"));
            Assert.That(result.FirstName, Is.EqualTo("Patrick"));
            Assert.That(result.LastName, Is.EqualTo("Star"));
            Assert.That(result.Emails.Count, Is.EqualTo(1));
            Assert.That(result.Emails.First().Id, Is.EqualTo(Guid.Parse("00000000-0000-0000-0000-000000000002")));
            Assert.That(result.Emails.First().EmailAddress, Is.EqualTo("test2@test.com"));
            Assert.That(result.Emails.First().IsPreferred, Is.EqualTo(false));
            Assert.That(result.Phones.First().Id, Is.EqualTo(Guid.Parse("00000000-0000-0000-0000-000000000003")));
            Assert.That(result.Phones.First().PhoneNumber, Is.EqualTo("09870987"));
        }

        #endregion

        #region Insert

        [Test]
        public async Task InsertSupplierAsync_InsertSupplierToDB()
        {
            //Arrange
            var service = SupplierService();

            var supplier = new Supplier
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Title = "title",
                ActivationDate = DateTime.UtcNow.AddDays(1),
                FirstName = "FirstName",
                LastName = "LastName",
                Emails = { new Email() { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), EmailAddress = "test@example.com", } },
                Phones = { new Phone { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), PhoneNumber = "123", } }
            };

            //Act
            await service.InsertSupplier(supplier);

            //Assert
            var suppliers = SupplierContextForAssert!.Suppliers.ToList();

            Assert.That(suppliers.Count, Is.EqualTo(1));
            Assert.That(suppliers[0].Id, Is.EqualTo(Guid.Parse("00000000-0000-0000-0000-000000000001")));
            Assert.That(suppliers[0].Title, Is.EqualTo("title"));
            Assert.That(suppliers[0].FirstName, Is.EqualTo("FirstName"));
            Assert.That(suppliers[0].LastName, Is.EqualTo("LastName"));
            Assert.That(suppliers[0].Emails.Count, Is.EqualTo(1));
            Assert.That(suppliers[0].Emails.First().Id, Is.EqualTo(Guid.Parse("00000000-0000-0000-0000-000000000002")));
            Assert.That(suppliers[0].Emails.First().EmailAddress, Is.EqualTo("test@example.com"));
            Assert.That(suppliers[0].Emails.First().IsPreferred, Is.EqualTo(false));
            Assert.That(suppliers[0].Phones.First().Id, Is.EqualTo(Guid.Parse("00000000-0000-0000-0000-000000000003")));
            Assert.That(suppliers[0].Phones.First().PhoneNumber, Is.EqualTo("123"));
        }

        #endregion

        #region Delete

        [Test]
        public async Task DeleteSupplierAsync_IsIdempotent_WhenTheSupplierDontExist()
        {
            //Arrange
            var service = SupplierService();

            //Act
            var result = await service.DeleteSupplierAsync(Guid.Empty);

            //Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task DeleteSupplierAsync_Deletes_WhenTheSupplierExists()
        {
            //Arrange
            var service = SupplierService(SeedSupplierData);

            //Act
            var result = await service.DeleteSupplierAsync(Guid.Parse("00000000-0000-0000-0000-000000000001"));

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(Guid.Parse("00000000-0000-0000-0000-000000000001")));

            var suppliers = SupplierContextForAssert!.Suppliers.ToList();
            Assert.That(suppliers.Count, Is.EqualTo(1));
        }

        [Test]
        public void DeleteSupplierAsync_ThrowsException_WhenSupplierIsActive()
        {
            //Arrange
            var service = SupplierService((context) =>
            {
                context.Suppliers.Add(new Supplier()
                {
                    ActivationDate = DateTime.UtcNow.AddDays(1),
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    Title = "Mr",
                    FirstName = "Patrick",
                    LastName = "Star",
                    Emails =
                        {
                            new ()
                            {
                                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                                EmailAddress = "test2@test.com",
                                IsPreferred = false
                            }
                        },
                    Phones =
                        {
                            new ()
                            {
                                Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                                PhoneNumber = "09870987",
                                IsPreferred = false
                            }
                        }
                });
            });

            //Act
            async Task Act()
            {
                var result = await service.DeleteSupplierAsync(Guid.Parse("00000000-0000-0000-0000-000000000001"));
            }

            //Assert
            Assert.That(Act, Throws.Exception.TypeOf<Exception>().With.Message.EqualTo($"Supplier 00000000-0000-0000-0000-000000000001 is active, can't be deleted"));
        }

        #endregion
    }
}