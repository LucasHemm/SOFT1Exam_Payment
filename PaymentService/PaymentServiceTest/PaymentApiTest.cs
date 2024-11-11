using System.Net;
using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using PaymentService;
using PaymentService.DTOs;
using Testcontainers.MsSql;

namespace PaymentServiceTest
{
    public class PaymentApiTest : IAsyncLifetime
    {
        private readonly MsSqlContainer _msSqlContainer;
        private readonly WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        public PaymentApiTest()
        {
            _msSqlContainer = new MsSqlBuilder()
                .WithImage("mcr.microsoft.com/mssql/server:2022-latest") // Use the correct SQL Server image
                .WithPassword("YourStrong!Passw0rd") // Set a strong password
                .WithCleanUp(true)
                .Build();

            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        // Remove the existing ApplicationDbContext registration
                        var descriptor = services.SingleOrDefault(
                            d => d.ServiceType ==
                                typeof(DbContextOptions<ApplicationDbContext>));
                        if (descriptor != null)
                        {
                            services.Remove(descriptor);
                        }

                        // Add ApplicationDbContext using the test container's connection string
                        services.AddDbContext<ApplicationDbContext>(options =>
                        {
                            options.UseSqlServer(_msSqlContainer.GetConnectionString());
                        });

                        // Ensure the database is created and migrations are applied
                        var sp = services.BuildServiceProvider();
                        using (var scope = sp.CreateScope())
                        {
                            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                            db.Database.Migrate();
                        }
                    });
                });
        }

        public async Task InitializeAsync()
        {
            await _msSqlContainer.StartAsync();
            _client = _factory.CreateClient();
        }

        public async Task DisposeAsync()
        {
            _client.Dispose();
            await _msSqlContainer.DisposeAsync();
            _factory.Dispose();
        }

        private StringContent GetStringContent(object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
        
        [Fact]
        public async Task CreatePayment_ReturnsCreated_WhenValidRequest()
        {
            var createRequest = new PaymentRequestDto
            {
                TotalPrice = 100.0,
                AgentRating = 4.4
            };
            var response = await _client.PostAsJsonAsync("/api/PaymentApi", createRequest);

            // Assert: Verify response is 201 Created and contains the payment data
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            // Optionally verify the location header
            Assert.NotNull(response.Headers.Location);

            // Deserialize the response to check the content
            var paymentDto = await response.Content.ReadFromJsonAsync<PaymentDTO>();
            Assert.NotNull(paymentDto);
            Assert.Equal(createRequest.TotalPrice, paymentDto.TotalPrice);
            Assert.NotNull(paymentDto.Date);
            Assert.Equal(100-Math.Round(100*0.06)-(100*0.04),paymentDto.PaymentProcessInfoDTO.RestaurantEarnings);
            Assert.Equal(100*0.04,paymentDto.PaymentProcessInfoDTO.AgentBonus);
            Assert.Equal(100*0.06,paymentDto.PaymentProcessInfoDTO.MTOGOFee);
        }

        [Fact]
        public async Task GetPayment_ReturnsOk_WhenPaymentExists()
        {
            // Arrange: Create a payment to retrieve (using CreatePayment endpoint or a mock)
            var createRequest = new PaymentRequestDto
            {
                TotalPrice = 100.0,
                AgentRating = 4.4
            };
            var createResponse = await _client.PostAsJsonAsync("/api/PaymentApi", createRequest);
            var createdPayment = await createResponse.Content.ReadFromJsonAsync<PaymentDTO>();
            var paymentId = createdPayment?.Id;

            // Act: Send a GET request to retrieve the payment by ID
            var response = await _client.GetAsync($"/api/PaymentApi/{paymentId}");

            // Assert: Verify response is 200 OK and contains the payment data
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var paymentDto = await response.Content.ReadFromJsonAsync<PaymentDTO>();
            Assert.NotNull(paymentDto);
            Assert.Equal(createRequest.TotalPrice, paymentDto.TotalPrice);
            
        }

        [Fact]
        public async Task GetPayment_ReturnsNotFound_WhenPaymentDoesNotExist()
        {
            // Act: Try to retrieve a non-existent payment ID
            var response = await _client.GetAsync("/api/PaymentApi/9999");

            // Assert: Verify response is 404 Not Found
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            var errorMessage = await response.Content.ReadAsStringAsync();
            Assert.Equal("Payment not found.", errorMessage);
        }
    }
}
