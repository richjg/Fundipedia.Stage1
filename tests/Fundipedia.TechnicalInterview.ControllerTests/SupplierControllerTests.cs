using Fundipedia.TechnicalInterview.Controllers;
using Fundipedia.TechnicalInterview.Data.Context;
using Fundipedia.TechnicalInterview.Domain;
using Fundipedia.TechnicalInterview.Model.Supplier;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;


namespace Fundipedia.TechincalInterview.ControllerTests
{
    public class SupplierControllerTests
    {
        private SupplierContext? SupplierContextForAssert;

        public SuppliersController NewSupplierController(Action<SupplierContext>? seedData = null)
        {
            var options = new DbContextOptionsBuilder<SupplierContext>()
                              .UseInMemoryDatabase(databaseName: "SupplierDatabase",(c) => { })
                              
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
            var supplierService = new SupplierService(supplierContext);
            var controller = new SuppliersController(supplierService);
            return controller;
        }

        private void SeedSuplierData(SupplierContext context)
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

        #region GetSuppliers

        [Test]
        public async Task GetSuppliers_ReturnsEmpty_WhenThereAreNoSuppliers()
        {
            //Arrange
            var controller = NewSupplierController();

            //Act
            var response = await controller.GetSuppliersAsync();

            //Assert
            var okResult = response.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            
            var suppliers = okResult.Value as List<Supplier>;
            Assert.That(suppliers, Is.Not.Null);
            Assert.That(suppliers.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetSuppliers_ReturnsSuppliers_WhenThereAreSuppliers()
        {
            //Arrange
            var controller = NewSupplierController(SeedSuplierData);

            //Act
            var response = await controller.GetSuppliersAsync();

            //Assert
            var okResult = response.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var suppliers = okResult.Value as List<Supplier>;
            Assert.That(suppliers, Is.Not.Null);
            Assert.That(suppliers.Count, Is.EqualTo(2));

            suppliers = suppliers.OrderBy(s => s.FirstName).ToList();
            Assert.That(suppliers[0].Id, Is.EqualTo(Guid.Parse("00000000-0000-0000-0000-000000000001")));
            Assert.That(suppliers[0].Title, Is.EqualTo("Mr"));
            Assert.That(suppliers[0].FirstName, Is.EqualTo("Patrick"));
            Assert.That(suppliers[0].LastName, Is.EqualTo("Star"));
            Assert.That(suppliers[0].Emails.Count, Is.EqualTo(1));
            Assert.That(suppliers[0].Emails.First().Id, Is.EqualTo(Guid.Parse("00000000-0000-0000-0000-000000000002")));
            Assert.That(suppliers[0].Emails.First().EmailAddress, Is.EqualTo("test2@test.com"));
            Assert.That(suppliers[0].Emails.First().IsPreferred, Is.EqualTo(false));
        }

        #endregion

        #region GetSupplier

        [Test]
        public async Task GetSupplier_ReturnsNotFound_WhenTheRequestedSupplierDoesNotExist()
        {
            //Arrange
            var controller = NewSupplierController();

            //Act
            var response = await controller.GetSupplierAsync(Guid.Empty);

            //Assert
            Assert.That(response.Result, Is.TypeOf<NotFoundResult>());
        }

        

        [Test]
        public async Task GetSupplier_ReturnsSupplier_WhenTheRequestedSupplierDoesExist()
        {
            //Arrange
            var controller = NewSupplierController(SeedSuplierData);

            //Act
            var response = await controller.GetSupplierAsync(Guid.Parse("00000000-0000-0000-0000-000000000001"));

            //Assert
            var okResult = response.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var supplier = (okResult.Value as Supplier)!;

            Assert.That(supplier.Id, Is.EqualTo(Guid.Parse("00000000-0000-0000-0000-000000000001")));
            Assert.That(supplier.Title, Is.EqualTo("Mr"));
            Assert.That(supplier.FirstName, Is.EqualTo("Patrick"));
            Assert.That(supplier.LastName, Is.EqualTo("Star"));
            Assert.That(supplier.Emails.Count, Is.EqualTo(1));
            Assert.That(supplier.Emails.First().Id, Is.EqualTo(Guid.Parse("00000000-0000-0000-0000-000000000002")));
            Assert.That(supplier.Emails.First().EmailAddress, Is.EqualTo("test2@test.com"));
            Assert.That(supplier.Emails.First().IsPreferred, Is.EqualTo(false));
        }

        #endregion

        #region Post Supplier

        [Test]
        public async Task PostSupplier_AddsSupplier_WhenSupplier()
        {
            //Arrange
            var controller = NewSupplierController();

            var supplier = new Supplier
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Title = "title",
                ActivationDate = DateTime.UtcNow.AddDays(1),
                FirstName = "FirstName",
                LastName = "LastName",
                Emails = { new Email { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), EmailAddress = "test@example.com",  } }
            };

            //Act
            var response = await controller.PostSupplierAsync(supplier);
            var result = response.Result as CreatedAtActionResult;

            //Assert
            Assert.That(result, Is.Not.Null);

            var supplierForAssert = (result.Value as Supplier)!;

            Assert.That(supplierForAssert.Id, Is.EqualTo(Guid.Parse("00000000-0000-0000-0000-000000000001")));
            Assert.That(supplierForAssert.Title, Is.EqualTo("title"));
            Assert.That(supplierForAssert.FirstName, Is.EqualTo("FirstName"));
            Assert.That(supplierForAssert.LastName, Is.EqualTo("LastName"));
            Assert.That(supplierForAssert.Emails.Count, Is.EqualTo(1));
            Assert.That(supplierForAssert.Emails.First().Id, Is.EqualTo(Guid.Parse("00000000-0000-0000-0000-000000000002")));
            Assert.That(supplierForAssert.Emails.First().EmailAddress, Is.EqualTo("test@example.com"));
            Assert.That(supplierForAssert.Emails.First().IsPreferred, Is.EqualTo(false));
        }

        #endregion

        #region DeleteSupplier

        [Test]
        public async Task DeleteSupplier_ReturnsOK_WhenSupplierDoesNotExist_Idempotent()
        {
            //Arrange
            var controller = NewSupplierController();

            //Act
            var response = await controller.DeleteSupplierAsync(Guid.Parse("00000000-0000-0000-0000-000000000000"));
            var result = response.Result as OkObjectResult;

            //Assert
            Assert.That(result, Is.Not.Null);
            var supplier = result.Value as Supplier;
            Assert.That(supplier, Is.Null);
        }

        [Test]
        public async Task DeleteSupplier_ReturnsOK_WhenSupplierDoesExist()
        {
            //Arrange
            var controller = NewSupplierController(SeedSuplierData);

            //Act
            var response = await controller.DeleteSupplierAsync(Guid.Parse("00000000-0000-0000-0000-000000000001"));
            var result = response.Result as OkObjectResult;

            //Assert
            Assert.That(result, Is.Not.Null);
            var supplier = result.Value as Supplier;
            Assert.That(supplier!.Id, Is.EqualTo(Guid.Parse("00000000-0000-0000-0000-000000000001")));

            var suppliers = SupplierContextForAssert!.Suppliers.ToList();
            Assert.That(suppliers.Count, Is.EqualTo(1));
            Assert.That(suppliers[0].Id, Is.Not.EqualTo(Guid.Parse("00000000-0000-0000-0000-000000000001")));
        }

        [Test]
        public async Task DeleteSupplier_ReturnsBadArguments_WhenSupplierIsActive()
        {
            //Arrange
            var controller = NewSupplierController((context) =>
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
            var response = await controller.DeleteSupplierAsync(Guid.Parse("00000000-0000-0000-0000-000000000001"));
            var result = response.Result as BadRequestObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo("Supplier is active and cannot be deleted"));
        }

        #endregion

        #region ModelStateTest

        /// <summary>
        /// This is not exhaustive - does not cover all attributes 
        /// </summary>

        [Test]
        public void TryValidateSupplierModel_ReturnsTrue_WhenSupplierRootIsValid()
        {
            //Arrange
            var supplier = new Supplier
            {
                Id = Guid.NewGuid(),
                Title = "title",
                ActivationDate = DateTime.UtcNow.AddDays(1),
                FirstName = "FirstName",
                LastName = "LastName",
            };

            //Act
            var result = TryValidateModel(supplier, out var validationsResults);

            //Assert
            Assert.That(result, Is.True);
            Assert.That(validationsResults.Count, Is.EqualTo(0));
        }

        [Test]
        public void TryValidateSupplierModel_ReturnsFalse_WhenActivationDateIsNotUtc()
        {
            //Arrange
            var supplier = new Supplier
            {
                Id = Guid.NewGuid(),
                Title = "title",
                ActivationDate = DateTime.Now.AddDays(1), //<-- Now will be local kind
                FirstName = "FirstName",
                LastName = "LastName",
            };

            //Act

            var result = TryValidateModel(supplier, out var validationsResults);

            //Assert
            Assert.That(result, Is.False);
            Assert.That(validationsResults.First().ErrorMessage, Is.EqualTo("ActivationDate must be sent as UTC"));
        }

        [Test]
        public void TryValidateSupplierModel_ReturnsFalse_WhenActivationDateIsEarlierThanTomorrow()
        {
            //Arrange
            var supplier = new Supplier
            {
                Id = Guid.NewGuid(),
                Title = "title",
                ActivationDate = DateTime.UtcNow.Date.AddDays(1).AddMilliseconds(-1),
                FirstName = "FirstName",
                LastName = "LastName",
            };

            //Act
            var result = TryValidateModel(supplier, out var validationsResults);

            //Assert
            Assert.That(result, Is.False);
            Assert.That(validationsResults.First().ErrorMessage, Is.EqualTo("ActivationDate must be tomorrow or later"));
        }

        [Test]
        public void TryValidateEmailModel_ReturnsTrue_WhenValid()
        {
            //Arrange

            var model = new Email
            {
                Id = Guid.NewGuid(),
                EmailAddress = "example@example.com",
                IsPreferred = true,
            };

            //Act
            var result = TryValidateModel(model, out var validationsResults);

            //Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void TryValidateEmailModel_ReturnsFalse_WhenEmailAddressIsInvalid()
        {
            //Arrange

            var model = new Email
            {
                Id = Guid.NewGuid(),
                EmailAddress = "this is not a valid email address",
                IsPreferred = true,
            };

            //Act
            var result = TryValidateModel(model, out var validationsResults);

            //Assert
            Assert.That(result, Is.False);
            Assert.That(validationsResults.First().ErrorMessage, Is.EqualTo("The EmailAddress field is not a valid e-mail address."));
        }

        [Test]
        public void TryValidatePhoneModel_ReturnsTrue_WhenValid()
        {
            //Arrange

            var model = new Phone
            {
                Id = Guid.NewGuid(),
                PhoneNumber = "1234567890"
            };

            //Act
            var result = TryValidateModel(model, out var validationsResults);

            //Assert
            Assert.That(result, Is.True);
        }

        [TestCase("a")]
        [TestCase("12345678901")] //<-- more then 10
        public void TryValidatePhoneModel_ReturnsFalse_WhenPhoneNumberIsInvalid(string invalidPhoneNumber)
        {
            //Arrange

            var model = new Phone
            {
                Id = Guid.NewGuid(),
                PhoneNumber = invalidPhoneNumber
            };

            //Act
            var result = TryValidateModel(model, out var validationsResults);

            //Assert
            Assert.That(result, Is.False);
            Assert.That(validationsResults.First().ErrorMessage, Is.EqualTo("Phone number is invalid. Must be only numbers and a max of 10 digits"));
        }

        private bool TryValidateModel(object model, out List<ValidationResult> results)
        {
            results = [];
            var validationContext = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, validationContext, results, true);

            return results.Count == 0;
        }
        #endregion
    }
}