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


        [HttpGet("cancel")]
        public IActionResult CancelPayment(int orderId)
        {
            
            return Ok(new { Message = "Payment cancelled by user.", OrderId = orderId });
        }
    }
    
}
