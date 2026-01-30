using AutoMapper;
using LogisticsHub.Application.DTOs;
using LogisticsHub.Application.Interfaces.Services;
using LogisticsHub.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LogisticsHub.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public OrdersController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }


        [HttpPost]
        public async Task<IActionResult> Create(OrderDetailsDto orderDto)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;

            var result = await _orderService.CreateOrderAsync(userId!.ToString(), orderDto);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            var orderdto = _mapper.Map<OrderDetailsDto>(result.Data);
            return CreatedAtAction("GetById", new { id = result.Data!.Id }, orderdto);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;

            var result = await _orderService.GetUserOrdersAsync(userId!.ToString());

            if (!result.IsSuccess)
            {
                return NotFound(result.Message);
            }

            return Ok(result.Data);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;

            var result = await _orderService.GetOrderDetailsAsync(id, userId!.ToString());

            if (!result.IsSuccess)
            {
                return NotFound(result.Message);
            }

            return Ok(result.Data);
        }


        [HttpPost("{id}")]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;

            var result = await _orderService.CancelOrderAsync(id, userId!.ToString());

            if (!result.IsSuccess)
            {
                if (result.StatusCode == 404)
                {
                    return NotFound(result.Message);
                }

                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id,[FromBody] OrderStatus status)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;

            var result = await _orderService.UpdateOrderAsync(id, status, userId!.ToString());

            if (!result.IsSuccess)
            {
                if(result.StatusCode == 404)
                {
                    return NotFound(result.Message);
                }

                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }
    }
}
