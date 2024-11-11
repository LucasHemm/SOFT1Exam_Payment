using Microsoft.AspNetCore.Mvc;
using PaymentService.DTOs;
using PaymentService.Facades;

namespace PaymentService.Api;
[ApiController]
[Route("api/[controller]")]
public class PaymentApi: ControllerBase
{
    private readonly PaymentFacade _paymentFacade;

    public PaymentApi(PaymentFacade paymentFacade)
    {
        _paymentFacade = paymentFacade;
    }
    
    // POST: api/Payment
    [HttpPost]
    public IActionResult CreatePayment([FromBody] PaymentRequestDto request)
    {
        try
        {
            // Use the CreatePayment method to create a new payment.
            var payment = _paymentFacade.CreatePayment(request.TotalPrice, request.AgentRating);

            // Convert to a DTO if necessary for response (optional)
            var paymentDto = new PaymentDTO(payment);
        
            // Return a 201 Created response with the new payment data.
            return CreatedAtAction(nameof(GetPayment), new { id = payment.Id }, paymentDto);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    
    // GET: api/Payment/{id}
    [HttpGet("{id}")]
    public IActionResult GetPayment(int id)
    {
        try
        {
            var payment = _paymentFacade.GetPaymentById(id);
        
            if (payment == null)
            {
                return NotFound("Payment not found.");
            }

            var paymentDto = new PaymentDTO(payment);
            return Ok(paymentDto);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

}