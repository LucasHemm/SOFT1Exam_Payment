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

    public PaymentProcessInfo CreatePaymentProcessInfo(double totalPrice, double agentRating)
    {
        if (agentRating < 0 || agentRating > 5)
            throw new ArgumentOutOfRangeException(nameof(agentRating), "Agent rating must be between 0 and 5 inclusive.");

        double agentBonus = totalPrice * (Math.Round(agentRating) / 100);

        double mtogoFee;
        if (totalPrice <= 100)
            mtogoFee = totalPrice * 0.06;
        else if (totalPrice <= 500)
            mtogoFee = totalPrice * 0.05;
        else if (totalPrice <= 1000)
            mtogoFee = totalPrice * 0.04;
        else
            mtogoFee = totalPrice * 0.03;

        double resFee = (totalPrice - mtogoFee) - agentBonus;

        return new PaymentProcessInfo(resFee, agentBonus, mtogoFee);
    }

}