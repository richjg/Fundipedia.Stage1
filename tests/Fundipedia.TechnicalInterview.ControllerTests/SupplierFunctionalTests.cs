using Fundipedia.TechnicalInterview;
using Fundipedia.TechnicalInterview.Data.Context;
using Fundipedia.TechnicalInterview.Model.Supplier;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Fundipedia.TechnicalInterview.ControllerTests
{
    public class SupplierApplication : WebApplicationFactory<Startup>
    {
        private readonly Action<SupplierContext>? _seedDataAction;

        public SupplierApplication(Action<SupplierContext>? seedDataAction = null)
        {
            _seedDataAction = seedDataAction;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<SupplierContext>));
                services.Remove(dbContextDescriptor!);

                services.AddDbContext<SupplierContext>(options =>
                {
                    options.UseInMemoryDatabase(databaseName: "SupplierDatabase");
                });

                using (var scope = services.BuildServiceProvider().CreateScope())
                {
                    using (var context = scope.ServiceProvider.GetRequiredService<SupplierContext>())
                    {
                        _seedDataAction?.Invoke(context);
                        context.SaveChanges();
                    }
                }
            });
        }
    }

    public class SupplierIFunctionalTests
    {
        /// <summary>
        /// Have not written all the tests :(
        /// </summary>
        /// <returns></returns>

        [Test]
        public async Task PostSupplier_ReturnsCreated_WhenModelIsValid()
        {
            //Arrange
            var app = new SupplierApplication();
            var httpClient = app.CreateClient();

            //Act
            var supplier = new Supplier
            {
                Id = Guid.NewGuid(),
                Title = "title",
                ActivationDate = DateTime.UtcNow.AddDays(1),
                FirstName = "FirstName",
                LastName = "LastName",
                Emails = { new Email() { Id = Guid.NewGuid(), EmailAddress = "test@example.com", } },
                Phones = { new Phone { Id = Guid.NewGuid(), PhoneNumber = "123", } }
            };

            var json = JsonConvert.SerializeObject(supplier);
            var responseMessage = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, "api/suppliers")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });

            //Assert
            Assert.That(responseMessage.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        }

        [Test]
        public async Task PostSupplier_ReturnsBadArgument_WhenPhoneNumberIsInvalid()
        {
            //Arrange
            var app = new SupplierApplication();
            var httpClient = app.CreateClient();

            //Act
            var supplier = new Supplier
            {
                Id = Guid.NewGuid(),
                Title = "title",
                ActivationDate = DateTime.UtcNow.AddDays(1),
                FirstName = "FirstName",
                LastName = "LastName",
                Phones = { new Phone { Id = Guid.NewGuid(), PhoneNumber = "not a valid phone number", } }
            };

            var json = JsonConvert.SerializeObject(supplier);
            var responseMessage = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, "api/suppliers")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });

            //Assert
            Assert.That(responseMessage.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            var jsonContent = await responseMessage.Content.ReadAsStringAsync();
            var errors = JsonConvert.DeserializeAnonymousType(jsonContent, new { Errors = new Dictionary<string, string[]>() });

            Assert.That(errors!.Errors.Keys.First(), Is.EqualTo("Phones[0].PhoneNumber"));
            Assert.That(errors!.Errors.Values.First().First(), Is.EqualTo("Phone number is invalid. Must be only numbers and a max of 10 digits"));
        }

        [Test]
        public async Task PostSupplier_ReturnsBadArgument_WhenEmailAddressIsInvalid()
        {
            //Arrange
            var app = new SupplierApplication();
            var httpClient = app.CreateClient();

            //Act
            var supplier = new Supplier
            {
                Id = Guid.NewGuid(),
                Title = "title",
                ActivationDate = DateTime.UtcNow.AddDays(1),
                FirstName = "FirstName",
                LastName = "LastName",
                Emails = { new Email { Id = Guid.NewGuid(), EmailAddress = "not a valid email address", } }
            };

            var json = JsonConvert.SerializeObject(supplier);
            var responseMessage = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, "api/suppliers")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });

            //Assert
            Assert.That(responseMessage.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            var jsonContent = await responseMessage.Content.ReadAsStringAsync();
            var errors = JsonConvert.DeserializeAnonymousType(jsonContent, new { Errors = new Dictionary<string, string[]>() });

            Assert.That(errors!.Errors.Keys.First(), Is.EqualTo("Emails[0].EmailAddress"));
            Assert.That(errors!.Errors.Values.First().First(), Is.EqualTo("The EmailAddress field is not a valid e-mail address."));
        }

        [Test]
        public async Task PostSupplier_ReturnsBadArgument_WhenActivationDateIsNotUtc()
        {
            //Arrange
            var app = new SupplierApplication();
            var httpClient = app.CreateClient();

            //Act
            var supplier = new Supplier
            {
                Id = Guid.NewGuid(),
                Title = "title",
                ActivationDate = DateTime.Now.AddDays(10),
                FirstName = "FirstName",
                LastName = "LastName",
                Phones = { new Phone { Id = Guid.NewGuid(), PhoneNumber = "123", } }
            };

            var json = JsonConvert.SerializeObject(supplier);
            var responseMessage = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, "api/suppliers")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });

            //Assert
            Assert.That(responseMessage.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            var jsonContent = await responseMessage.Content.ReadAsStringAsync();
            var errors = JsonConvert.DeserializeAnonymousType(jsonContent, new { Errors = new Dictionary<string, string[]>() });

            Assert.That(errors!.Errors.Keys.First(), Is.EqualTo("ActivationDate"));
            Assert.That(errors!.Errors.Values.First().First(), Is.EqualTo("ActivationDate must be sent as UTC"));
        }

        [Test]
        public async Task PostSupplier_ReturnsBadArgument_WhenActivationDateIsInvalid()
        {
            //Arrange
            var app = new SupplierApplication();
            var httpClient = app.CreateClient();

            //Act
            var supplier = new Supplier
            {
                Id = Guid.NewGuid(),
                Title = "title",
                ActivationDate = DateTime.UtcNow.AddDays(-1),
                FirstName = "FirstName",
                LastName = "LastName",
                Phones = { new Phone { Id = Guid.NewGuid(), PhoneNumber = "123", } }
            };

            var json = JsonConvert.SerializeObject(supplier);
            var responseMessage = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, "api/suppliers")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });

            //Assert
            Assert.That(responseMessage.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            var jsonContent = await responseMessage.Content.ReadAsStringAsync();
            var errors = JsonConvert.DeserializeAnonymousType(jsonContent, new { Errors = new Dictionary<string, string[]>() });

            Assert.That(errors!.Errors.Keys.First(), Is.EqualTo("ActivationDate"));
            Assert.That(errors!.Errors.Values.First().First(), Is.EqualTo("ActivationDate must be tomorrow or later"));
        }
    }
}