using Microsoft.EntityFrameworkCore;
using PaymentService;
using PaymentService.DTOs;
using PaymentService.Facades;
using PaymentService.Models;
using Testcontainers.MsSql;

namespace PaymentServiceTest;

public class IntegrationTests: IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest") // Use the correct SQL Server image
        .WithPassword("YourStrong!Passw0rd") // Set a strong password
        .Build();

    private string _connectionString;

    public async Task InitializeAsync()
    {
        await _msSqlContainer.StartAsync();

        // Create the connection string for the database
        _connectionString = _msSqlContainer.GetConnectionString();

        // Initialize the database context and apply migrations
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(_connectionString)
            .Options;

        using (var context = new ApplicationDbContext(options))
        {
            context.Database.Migrate(); // Apply any pending migrations
        }
    }

    public async Task DisposeAsync()
    {
        await _msSqlContainer.DisposeAsync().AsTask();
    }

    [Fact]
    public void ShouldCreatePayment()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(_connectionString)
            .Options;
        
        using (var context = new ApplicationDbContext(options))
        {
            PaymentFacade paymentFacade = new PaymentFacade(context);
            Payment payment = paymentFacade.CreatePayment(750.75,3.35);
            Payment createdPayment = context.Payments.Find(payment.Id);
            
            Assert.Equal(750.75,createdPayment.TotalPrice);
            Assert.NotNull(createdPayment.Date);

            Assert.Equal(698.1975, createdPayment.PaymentProcessInfo.RestaurantEarnings);
            Assert.Equal(22.5225, createdPayment.PaymentProcessInfo.AgentBonus);
            Assert.Equal(30.03, createdPayment.PaymentProcessInfo.MTOGOFee);
        }
    }

    [Fact]
    public void ShouldGetPayment()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(_connectionString)
            .Options;
        
        using (var context = new ApplicationDbContext(options))
        {
            PaymentFacade paymentFacade = new PaymentFacade(context);
            Payment payment = paymentFacade.CreatePayment(750.75,3.35);

            Payment paymentFound = paymentFacade.GetPaymentById(payment.Id);
            
            Assert.Equal(payment.TotalPrice,paymentFound.TotalPrice);
            Assert.Equal(payment.Date,paymentFound.Date);
            Assert.Equal(payment.PaymentProcessInfo,paymentFound.PaymentProcessInfo);
        }
    }
}