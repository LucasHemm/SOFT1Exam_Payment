using Microsoft.EntityFrameworkCore;
using PaymentService;
using PaymentService.Facades;
using PaymentService.Models;

namespace PaymentServiceTest;

public class UnitTests
{

    private readonly PaymentFacade _paymentFacade;
    public UnitTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        var context = new ApplicationDbContext(options);
        _paymentFacade = new PaymentFacade(context);
    }

    [Fact]
    public void ShouldCreatePaymentProcessInfo()
    {
        PaymentProcessInfo info = _paymentFacade.CreatePaymentProcessInfo(489.5,3.2);
        Assert.Equal(489.5*0.05,info.MTOGOFee);
        Assert.Equal(489.5*0.03,info.AgentBonus);
        Assert.Equal((489.5-info.MTOGOFee)-info.AgentBonus,info.RestaurantEarnings);
    }

}