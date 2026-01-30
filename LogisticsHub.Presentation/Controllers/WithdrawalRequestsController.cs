using AutoMapper;
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
    public class WithdrawalRequestsController : ControllerBase
    {
        private readonly IWithdrawalRequestService _withdrawalRequestService;
        private readonly IMapper _mapper;
        public WithdrawalRequestsController(IWithdrawalRequestService withdrawalRequestService, IMapper mapper)
        {
            _withdrawalRequestService = withdrawalRequestService;
            _mapper = mapper;
        }


        [HttpPost("Request")]
        public async Task<IActionResult> CreateRequest(WithdrawaRequestDto requestDto,[FromQuery] string regectionReason)
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

            var dtos=_mapper.Map<WithdrawaRequestDto>(result.Data);
            return Ok(dtos);
        }


        [HttpPost("{id}/Approve")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Approve(int id, [FromQuery]string AdminComment, [FromQuery]string externalReferenceId)
        {
            var result=await _withdrawalRequestService.ApproveWithdrawalRequest(id, AdminComment??null, externalReferenceId??null);
        
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            var dtos = _mapper.Map<WithdrawaRequestDto>(result.Data);
            return Ok(dtos);
        }


        [HttpPost("{id}/Reject")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Reject(int id, [FromBody]string rejectionReason)
        {
            var result=await _withdrawalRequestService.RejectWithdrawalRequest(id, rejectionReason??null);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            var dtos = _mapper.Map<WithdrawaRequestDto>(result.Data);
            return Ok(dtos);
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

            var dtos = _mapper.Map<WithdrawaRequestDto>(result.Data);
            return Ok(dtos);
        }
    }
}
