using AutoMapper;
using LogisticsHub.Application.DTOs;
using LogisticsHub.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LogisticsHub.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        public ProductsController(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _productService.GetProductByIdAsync(id);

            if (!result.IsSuccess)
            {
                return NotFound(result.Message);
            }

            return Ok(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {

            var result = await _productService.GetAllProductsAsync();

            return Ok(result.Data);
        }

        [Authorize(Roles ="Admin,Merchant")]
        [HttpPost("Add")]
        public async Task<IActionResult> Add(ProductAddDto productDto)
        {
            var result= await _productService.AddProductAsync(productDto);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            var productdto=_mapper.Map<ProductDto>(result.Data);
            return CreatedAtAction(nameof(GetById),new { id=result.Data!.Id },productdto);
        }


        [Authorize(Roles = "Admin,Merchant")]
        [HttpPut("Update")]
        public async Task<IActionResult> Update(ProductDto productDto)
        {
            var result= await _productService.UpdateProductAsync(productDto);

            if (!result.IsSuccess)
            {
                if (result.StatusCode == 404)
                {
                    return NotFound(result.Message);
                }

                else if(result.StatusCode == 409)
                {
                    return Conflict(result.Message);
                }

                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }


        [Authorize(Roles = "Admin,Merchant")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result=await _productService.DeleteProductAsync(id);

            if (!result.IsSuccess)
            {
                return NotFound(result.Message);
            }

            return Ok(result.Message);
        }
    }
}
