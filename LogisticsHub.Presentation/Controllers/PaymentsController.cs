using LogisticsHub.Application.Interfaces.Repositories;
using LogisticsHub.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace LogisticsHub.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentsController : ControllerBase
    {

        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;
        public PaymentsController(IPaymentService paymentService, IUnitOfWork unitOfWork)
        {
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePaymentSession(int OrderId)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
            var order =await _unitOfWork.OrderRepository.GetOrderWithDetailsAsync(OrderId,userId.ToString());

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


    }
    
}
