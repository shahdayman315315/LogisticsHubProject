using AutoMapper;
using LogisticsHub.Application.DTOs;
using LogisticsHub.Application.Interfaces.Repositories;
using LogisticsHub.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LogisticsHub.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StoresController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public StoresController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }


        [HttpPost]
        [Authorize(Roles = "Admin,Merchant")]
        public async Task<IActionResult> Create(CreateStoreDto dto)
        {
            var existStore = await _unitOfWork.StoreRepository.GetFirstAsync(s => s.Name == dto.Name);

            if (existStore is not null)
            {
                return BadRequest("This Store Name already exists");
            }

            var newStore = _mapper.Map<Store>(dto);

            var IsMerchant = User.IsInRole("Merchant");

            if (IsMerchant)
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;

                var merchant = await _unitOfWork.MerchantRepository.GetFirstAsync(m => m.UserId == userId);

                if(merchant is null)
                {
                    return BadRequest("Merchant is Not found");
                }

                newStore.MerchantId = merchant.Id;

            }

            await _unitOfWork.StoreRepository.AddAsync(newStore);
            await _unitOfWork.CompleteAsync();

            return Ok(dto);
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<Store> stores;

            var IsAdmin = User.IsInRole("Admin");

            if (!IsAdmin)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var merchant = await _unitOfWork.MerchantRepository.GetFirstAsync(m => m.UserId == userId);

                if (merchant is null) 
                    return NotFound("Merchant is not found.");

                stores=await _unitOfWork.StoreRepository.GetAllAsync(s=>s.MerchantId==merchant.Id);
            }

            else
            {
                stores = await _unitOfWork.StoreRepository.GetAllAsync();

            }

            if (stores is null)
            {
                return NotFound("No Stores is found");
            }

            var storesDtos = _mapper.Map<IEnumerable<CreateStoreDto>>(stores);

            return Ok(storesDtos);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var existStore = await _unitOfWork.StoreRepository.GetByIdAsync(id);

            if (existStore is null)
            {
                return NotFound("Store is not found");
            }

            var storeDto = _mapper.Map<Store>(existStore);

            return Ok(storeDto);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreateStoreDto dto)
        {
            var existStore = await _unitOfWork.StoreRepository.GetByIdAsync(id);

            if (existStore is null)
            {
                return NotFound("Store is not found");
            }

            if (!User.IsInRole("Admin"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var merchant = await _unitOfWork.MerchantRepository.GetFirstAsync(m => m.UserId == userId);

                if (merchant is null || existStore.MerchantId != merchant.Id)
                    return Forbid("You don't have permission to update this store.");
            }

             _mapper.Map<CreateStoreDto,Store>(dto,existStore );
            _unitOfWork.StoreRepository.Update(existStore);
            await _unitOfWork.CompleteAsync();

            return Ok(existStore);
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var existStore = await _unitOfWork.StoreRepository.GetByIdAsync(id);

            if (existStore is null)
            {
                return NotFound("Store is already not found");
            }

             _unitOfWork.StoreRepository.Delete(existStore);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
    }
}
