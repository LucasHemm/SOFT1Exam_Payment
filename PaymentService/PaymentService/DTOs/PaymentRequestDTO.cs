namespace PaymentService.DTOs;

public class PaymentRequestDto
{
    public double TotalPrice { get; set; }
    public double AgentRating { get; set; }

    public PaymentRequestDto(double totalPrice, double agentRating)
    {
        TotalPrice = totalPrice;
        AgentRating = agentRating;
    }

    public PaymentRequestDto()
    {
    }
}