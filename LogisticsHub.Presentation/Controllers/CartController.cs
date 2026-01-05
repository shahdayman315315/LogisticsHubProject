using LogisticsHub.Application.DTOs;
using LogisticsHub.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LogisticsHub.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result=await _cartService.GetCartAsync();

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


        [HttpPost("Add")]
        public async Task<IActionResult> Add(CartItemDto cartDto)
        {
            var result = await _cartService.AddToCart(cartDto.ProductId, cartDto.Quantity);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return CreatedAtAction(nameof(Get),null,result.Data);
        }


        [HttpDelete("Remove{id}")]
        public async Task<IActionResult> RemoveItem(int id)
        {
            var result=await _cartService.RemoveItemAsync(id);

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


        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteCart()
        {
            var result=await _cartService.ClearCartAsync();

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update(CartItemDto cartDto)
        {
            var result=await _cartService.UpdateQuantityAsync(cartDto.ProductId,cartDto.Quantity);

            if(!result.IsSuccess)
            {
                if (result.StatusCode == 404)
                {
                    return NotFound(result.Message);    
                }

                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }

        [HttpGet("TotalItems")]
        public async Task<IActionResult> GetTotalItems()
        {
            var result = await _cartService.GetTotalCountAsync();

            if(!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }
    }
}
