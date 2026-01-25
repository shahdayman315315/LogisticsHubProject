using LogisticsHub.Application.Interfaces.Repositories;
using LogisticsHub.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.IO;
using System.Runtime.CompilerServices;

namespace LogisticsHub.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        

        //order stripe id ?
        //order services ??
        //url &controller ??

        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;
        public PaymentController(IPaymentService paymentService, IUnitOfWork unitOfWork)
        {
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePaymentSession(int OrderId)
        {
            var order=await _unitOfWork.OrderRepository.GetByIdAsync(OrderId);

            if(order is null)
            {
                return NotFound("Order is not found.");
            }

            var result= await _paymentService.CreateCheckOutSessionAsync(order);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }

        [HttpGet("checkpayment")]
        public async Task<IActionResult> CheckPayment(string sessionId)
        {
            var result=await _paymentService.CheckPaymentAsync(sessionId);

            if(!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebHook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            var stripeSignature = Request.Headers["Stripe-Signature"];

            var endpointSecret = "";

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, endpointSecret);

                if (stripeEvent.Type ==Events.CheckoutSessionCompleted)
                {
                    var session = stripeEvent.Data.Object as Stripe.Checkout.Session;

                    await _paymentService.CheckPaymentAsync(session.Id);
                }

                return Ok(); 
            }
            catch (StripeException e)
            {
                return BadRequest(); 
            }
        }
    }
    
}
