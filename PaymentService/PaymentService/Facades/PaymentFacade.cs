using Microsoft.EntityFrameworkCore;
using PaymentService.DTOs;
using PaymentService.Models;

namespace PaymentService.Facades;

public class PaymentFacade
{
    private readonly ApplicationDbContext _context;

    public PaymentFacade(ApplicationDbContext context)
    {
        _context = context;
    }

    public Payment CreatePayment(double totalPrice, double agentRating)
    {
        Payment payment = new Payment(totalPrice, DateTime.Now, CreatePaymentProcessInfo(totalPrice, agentRating));
        _context.Payments.Add(payment);
        _context.SaveChanges();
        return payment;
    }

    public Payment GetPaymentById(int id)
    {
        Payment payment = _context.Payments
            .Include(p => p.PaymentProcessInfo)
            .FirstOrDefault(p => p.Id == id);
        if (payment == null)
        {
            throw new Exception("Restaurant not found");
        }

        return payment;
    }

    private PaymentProcessInfo CreatePaymentProcessInfo(double totalPrice, double agentRating)
    {
        double agentfee = totalPrice * (Math.Round(agentRating) / 100);
        
        double mtogofee;
        switch (totalPrice)
        {
            case <=100:
                mtogofee = totalPrice * 0.06;
                break;
            case <=500:
                mtogofee = totalPrice * 0.05;
                break;
            case <=1000:
                mtogofee = totalPrice * 0.04;
                break;
            default:
                mtogofee = totalPrice * 0.03;
                break;
        }
        
        return new PaymentProcessInfo(totalPrice - mtogofee - agentfee, agentfee, mtogofee);
    }
}