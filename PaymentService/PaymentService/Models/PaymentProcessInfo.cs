using PaymentService.DTOs;

namespace PaymentService.Models;

public class PaymentProcessInfo
{
    public int Id { get; set; }
    public double RestaurantEarnings { get; set; }
    public double AgentBonus { get; set; }
    public double MTOGOFee{ get; set; }

    public PaymentProcessInfo()
    {
    }

    public PaymentProcessInfo(int id, double restaurantEarnings, double agentBonus, double mtogoFee)
    {
        Id = id;
        RestaurantEarnings = restaurantEarnings;
        AgentBonus = agentBonus;
        MTOGOFee = mtogoFee;
    }
    
    public PaymentProcessInfo(PaymentProcessInfoDTO paymentProcessInfoDto)
    {
        Id = paymentProcessInfoDto.Id;
        RestaurantEarnings = paymentProcessInfoDto.RestaurantEarnings;
        AgentBonus = paymentProcessInfoDto.AgentBonus;
        MTOGOFee = paymentProcessInfoDto.MTOGOFee;
    }
    
    public PaymentProcessInfo(double restaurantEarnings, double agentBonus, double mtogoFee)
    {
        RestaurantEarnings = restaurantEarnings;
        AgentBonus = agentBonus;
        MTOGOFee = mtogoFee;
    }

    public PaymentProcessInfo(double totalPrice)
    {
        
    }
}