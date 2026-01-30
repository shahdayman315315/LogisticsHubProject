using AutoMapper;
using LogisticsHub.Application.DTOs;
using LogisticsHub.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LogisticsHub.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WalletsController : ControllerBase
    {

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public WalletsController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserWallet()
        {
            var userId =  User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;

            var wallet=await _unitOfWork.WalletRepository.GetWalletWithTransactionsAsync(userId);

            if(wallet is null)
            {
                return NotFound("Wallet is not found");
            }

            var walletDto=_mapper.Map<WalletDto>(wallet);

            return Ok(walletDto);
        }


        [HttpGet("Transactions")]
        public async Task<IActionResult> GetWalletTransactions([FromQuery]int pageNumber,[FromQuery]int pageSize)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;

            var wallet = await _unitOfWork.WalletRepository.GetWalletWithTransactionsAsync(userId);

            if (wallet is null)
            {
                return NotFound("Wallet is not found");
            }

            var transactions=await _unitOfWork.WalletRepository.GetWalletTransactionsAsync(wallet.Id,pageNumber,pageSize);

            if (!transactions.Any())
            {
                return NotFound("No Transactions was found for this wallet ");
            }

            var transactionDtos=_mapper.Map<TransactionDto>(transactions);

            return Ok(transactionDtos);
        }
    }
}
