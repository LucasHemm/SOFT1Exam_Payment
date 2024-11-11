using PaymentService.Models;

namespace PaymentService.DTOs;

public class PaymentDTO
{
    public int Id { get; set; }
    public double TotalPrice { get; set; }
    public DateTime Date { get; set; }
    public PaymentProcessInfoDTO PaymentProcessInfoDTO { get; set; }
    
    
    public PaymentDTO()
    {
    }

    public PaymentDTO(int id, double totalPrice, DateTime date, PaymentProcessInfoDTO paymentProcessInfoDTO)
    {
        Id = id;
        TotalPrice = totalPrice;
        Date = date;
        PaymentProcessInfoDTO = paymentProcessInfoDTO;
    }
    
    public PaymentDTO(Payment payment)
    {
        Id = payment.Id;
        TotalPrice = payment.TotalPrice;
        Date = payment.Date;
        PaymentProcessInfoDTO = new PaymentProcessInfoDTO(payment.PaymentProcessInfo);
    }
}