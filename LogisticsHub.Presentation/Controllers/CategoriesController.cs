using LogisticsHub.Infrastructure.Repositories.RepositoriesInterfaces;
using LogisticsHub.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace LogisticsHub.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache; 
        private const string CategoriesCacheKey = "AllCategories"; 

        public CategoriesController(IUnitOfWork unitOfWork, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }


        [HttpPost]
        [Authorize(Roles = "Admin,Merchant")]
        public async Task<IActionResult> Add(string Name)
        {
            var existCategory=await _unitOfWork.CategoryRepository.GetFirstAsync(c=>c.Name==Name);

            if(existCategory is not null)
            {
                return BadRequest("this Category Name already exists");
            }

            var newCategory = new Category
            {
                Name = Name
            };

            await _unitOfWork.CategoryRepository.AddAsync(newCategory);
            await _unitOfWork.CompleteAsync();

            _cache.Remove(CategoriesCacheKey); 

            return Ok(newCategory);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);

            if(category is null)
            {
                return NotFound("Category is not found");
            }

            return Ok(category);
        }



        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (!_cache.TryGetValue(CategoriesCacheKey, out IEnumerable<Category> categories))
            {
                categories = await _unitOfWork.CategoryRepository.GetAllAsync();

                if (!categories.Any())
                {
                    return NotFound("No Categories were found");
                }

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1)) 
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10)); 

                _cache.Set(CategoriesCacheKey, categories, cacheOptions);
            }

            return Ok(categories);
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Merchant")]

        public async Task<IActionResult> Update(int id, string newName)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);

            if (category is null)
            {
                return NotFound("Category is not found");
            }

            category.Name = newName;

            _unitOfWork.CategoryRepository.Update(category);
            await _unitOfWork.CompleteAsync();

            _cache.Remove(CategoriesCacheKey);

            return Ok(category);
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Merchant")]

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);

            if (category is null)
            {
                return NotFound("Category is already not found");
            }

            _unitOfWork.CategoryRepository.Delete(category);
            await _unitOfWork.CompleteAsync();

            _cache.Remove(CategoriesCacheKey);

            return NoContent();
        }
        
    }
}
