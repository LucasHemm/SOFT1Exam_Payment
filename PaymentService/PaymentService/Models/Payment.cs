using PaymentService.DTOs;

namespace PaymentService.Models;

public class Payment
{
    public int Id { get; set; }
    public double TotalPrice { get; set; }
    public DateTime Date { get; set; }
    public PaymentProcessInfo PaymentProcessInfo { get; set; }

    public Payment()
    {
    }

    public Payment(int id, double totalPrice, DateTime date, PaymentProcessInfo paymentProcessInfo)
    {
        Id = id;
        TotalPrice = totalPrice;
        Date = date;
        PaymentProcessInfo = paymentProcessInfo;
    }
    
    public Payment( double totalPrice, DateTime date, PaymentProcessInfo paymentProcessInfo)
    {
        TotalPrice = totalPrice;
        Date = date;
        PaymentProcessInfo = paymentProcessInfo;
    }

    
    public Payment(PaymentDTO paymentDto)
    {
        Id = paymentDto.Id;
        TotalPrice = paymentDto.TotalPrice;
        Date = paymentDto.Date;
        PaymentProcessInfo = new PaymentProcessInfo(paymentDto.TotalPrice);
    }
}