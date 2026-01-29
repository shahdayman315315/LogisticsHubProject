using LogisticsHub.Application.DTOs;
using LogisticsHub.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Security.Claims;

namespace LogisticsHub.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WithdrawalRequestController : ControllerBase
    {
        private readonly IWithdrawalRequestService _withdrawalRequestService;
        public WithdrawalRequestController(IWithdrawalRequestService withdrawalRequestService)
        {
            _withdrawalRequestService = withdrawalRequestService;
        }


        [HttpPost("Request")]
        public async Task<IActionResult> CreateRequest(WithdrawaRequestDto requestDto,[FromBody]string regectionReason)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;

            var result = await _withdrawalRequestService.WithdrawalRequestAsync(userId.ToString(), requestDto);

            if (!result.IsSuccess)
            {
                if (result.StatusCode == 404)
                {
                    return NotFound(result.Message);
                }

                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }


        [HttpPost("{id}/Approve")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Approve(int requestId, [FromBody]string AdminComment, [FromBody]string externalReferenceId)
        {
            var result=await _withdrawalRequestService.ApproveWithdrawalRequest(requestId, AdminComment??null, externalReferenceId??null);
        
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }


        [HttpPost("{id}/Reject")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Reject(int requestId, [FromBody]string rejectionReason)
        {
            var result=await _withdrawalRequestService.RejectWithdrawalRequest(requestId, rejectionReason??null);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }


        [HttpGet]
        public async Task<IActionResult> GetPendingRequests()
        {
            var result=await _withdrawalRequestService.GetPendingRequestAsync();

            if (!result.IsSuccess)
            {
                if (result.StatusCode == 404)
                {
                    return NotFound(result.Message);    
                }

                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }
    }
}
