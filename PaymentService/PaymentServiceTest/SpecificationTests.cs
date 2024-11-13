using Microsoft.EntityFrameworkCore;
using PaymentService;
using PaymentService.Facades;

namespace PaymentServiceTest;

public class SpecificationTests
{
    private readonly PaymentFacade _paymentFacade;
    public SpecificationTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        var context = new ApplicationDbContext(options);
        _paymentFacade = new PaymentFacade(context);
    }
    
    
    [Theory]
    // AgentRating at exact boundaries
    [InlineData(500, 0)]
    [InlineData(500, 5)]
    // AgentRating just below rounding thresholds
    [InlineData(500, 0.49)]
    [InlineData(500, 1.49)]
    [InlineData(500, 2.49)]
    [InlineData(500, 3.49)]
    [InlineData(500, 4.49)]
    // AgentRating at rounding thresholds
    [InlineData(500, 0.5)]
    [InlineData(500, 1.5)]
    [InlineData(500, 2.5)]
    [InlineData(500, 3.5)]
    [InlineData(500, 4.5)]
    // AgentRating just above rounding thresholds
    [InlineData(500, 0.51)]
    [InlineData(500, 1.51)]
    [InlineData(500, 2.51)]
    [InlineData(500, 3.51)]
    [InlineData(500, 4.51)]
    public void Test_AgentRating_Boundaries(double totalPrice, double agentRating)
    {
        // Arrange
        double expectedMtogoFee = totalPrice * 0.05; // Since totalPrice is 500
        double roundedAgentRating = Math.Round(agentRating);
        double expectedAgentBonus = totalPrice * (roundedAgentRating / 100);
        double expectedResFee = (totalPrice - expectedMtogoFee) - expectedAgentBonus;

        // Act
        var paymentProcessInfo = _paymentFacade.CreatePaymentProcessInfo(totalPrice, agentRating);

        // Assert
        Assert.Equal(expectedAgentBonus, paymentProcessInfo.AgentBonus, 2);
        Assert.Equal(expectedMtogoFee, paymentProcessInfo.MTOGOFee, 2);
        Assert.Equal(expectedResFee, paymentProcessInfo.RestaurantEarnings, 2);
    }

    [Theory]
    // Testing invalid agent ratings (edge cases)
    [InlineData(500, -0.1)] // Below minimum
    [InlineData(500, 5.1)]  // Above maximum
    public void Test_Invalid_AgentRatings(double totalPrice, double agentRating)
    {
        // Act & Assert
        Assert.Throws<System.ArgumentOutOfRangeException>(() => _paymentFacade.CreatePaymentProcessInfo(totalPrice, agentRating));
    }
    
    [Theory]
    // Testing totalPrice around $100 threshold
    [InlineData(99.99)]
    [InlineData(100)]
    [InlineData(100.01)]
    // Testing totalPrice around $500 threshold
    [InlineData(499.99)]
    [InlineData(500)]
    [InlineData(500.01)]
    // Testing totalPrice around $1000 threshold
    [InlineData(999.99)]
    [InlineData(1000)]
    [InlineData(1000.01)]
    public void Test_MtogoFee_TotalPrice_Boundaries(double totalPrice)
    {
        // Arrange
        double agentRating = 3.3;
        double roundedAgentRating = Math.Round(agentRating); // Should be 3
        double expectedAgentBonus = totalPrice * (roundedAgentRating / 100);

        double expectedMtogoFee;
        if (totalPrice <= 100)
            expectedMtogoFee = totalPrice * 0.06;
        else if (totalPrice <= 500)
            expectedMtogoFee = totalPrice * 0.05;
        else if (totalPrice <= 1000)
            expectedMtogoFee = totalPrice * 0.04;
        else
            expectedMtogoFee = totalPrice * 0.03;

        double expectedResFee = (totalPrice - expectedMtogoFee) - expectedAgentBonus;

        // Act
        var paymentProcessInfo = _paymentFacade.CreatePaymentProcessInfo(totalPrice, agentRating);

        // Assert
        Assert.Equal(expectedMtogoFee, paymentProcessInfo.MTOGOFee, 2);
        Assert.Equal(expectedAgentBonus, paymentProcessInfo.AgentBonus, 2);
        Assert.Equal(expectedResFee, paymentProcessInfo.RestaurantEarnings, 2);
    }
}