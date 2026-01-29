using LogisticsHub.Application.DTOs;
using LogisticsHub.Application.Helpers;
using LogisticsHub.Application.Interfaces.Repositories;
using LogisticsHub.Application.Interfaces.Services;
using LogisticsHub.Domain.Entities;
using LogisticsHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Infrastructure.ServicesImplementation
{
    public class WithdrawalRequestService:IWithdrawalRequestService
    {

        private readonly IUnitOfWork _unitOfWork;

        public WithdrawalRequestService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<WithDrawalRequest>> WithdrawalRequestAsync(string userId, WithdrawaRequestDto request)
        {
            var wallet = await _unitOfWork.WalletRepository.GetFirstAsync(w => w.UserId == userId);

            if (wallet is null)
            {
                return ServiceResult<WithDrawalRequest>.Failure("Wallet for this User is not exist", 404);
            }

            if (wallet.Balance < request.Amount)
            {
                return ServiceResult<WithDrawalRequest>.Failure("Balance is not Sufficient");
            }


            wallet.Balance -= request.Amount;
            _unitOfWork.WalletRepository.Update(wallet);

            var withdrawalRequest = new WithDrawalRequest
            {
                Amount = request.Amount,
                DestinationDetails = request.DestinationDetails,
                PaymentMethod = request.PaymentMethod,
                Wallet = wallet,
                Status = WithDrawalStatus.Pending,

            };

            await _unitOfWork.WithdrawalRequestsRepository.AddAsync(withdrawalRequest);
            await _unitOfWork.CompleteAsync();


            return ServiceResult<WithDrawalRequest>.Success(withdrawalRequest);
        }


        public async Task<ServiceResult<WithDrawalRequest>> RejectWithdrawalRequest(int requestId, string? rejectionReason = null)
        {
            var withdrawalRequest = await _unitOfWork.WithdrawalRequestsRepository.GetByIdAsync(requestId);

            if (withdrawalRequest is null || withdrawalRequest.Status != WithDrawalStatus.Pending)
            {
                return ServiceResult<WithDrawalRequest>.Failure("Request is not Found or invalid requeststatus update");
            }


            var wallet = await _unitOfWork.WalletRepository.GetByIdAsync(withdrawalRequest.WalletId);

            if (wallet is null)
            {
                return ServiceResult<WithDrawalRequest>.Failure("Wallet for this request is not Found");
            }

            wallet.Balance += withdrawalRequest.Amount;
            _unitOfWork.WalletRepository.Update(wallet);

            withdrawalRequest.Status = WithDrawalStatus.Rejected;
            withdrawalRequest.AdminComment = rejectionReason ?? "";
            _unitOfWork.WithdrawalRequestsRepository.Update(withdrawalRequest);

            var transaction = new Transaction
            {
                Amount = withdrawalRequest.Amount,
                CreatedAt = DateTime.UtcNow,
                Wallet = wallet,
                Description = "New Refund Process.",
                Type = TransactionType.Deposite

            };

            await _unitOfWork.TransactionRepository.AddAsync(transaction);
            await _unitOfWork.CompleteAsync();

            return ServiceResult<WithDrawalRequest>.Success(withdrawalRequest);
        }


        public async Task<ServiceResult<WithDrawalRequest>> ApproveWithdrawalRequest(int requestId, string? adminComment = null, string? ExternalReferenceId = null)
        {
            var withdrawalRequest = await _unitOfWork.WithdrawalRequestsRepository.GetByIdAsync(requestId);

            if (withdrawalRequest is null || withdrawalRequest.Status != WithDrawalStatus.Pending)
            {
                return ServiceResult<WithDrawalRequest>.Failure("Request is not Found or invalid requeststatus update");
            }

            var wallet = await _unitOfWork.WalletRepository.GetByIdAsync(withdrawalRequest.WalletId);

            if (wallet is null)
            {
                return ServiceResult<WithDrawalRequest>.Failure("Wallet for this request is not Found");
            }

            withdrawalRequest.Status = WithDrawalStatus.Approved;
            _unitOfWork.WithdrawalRequestsRepository.Update(withdrawalRequest);

            var transaction = new Transaction
            {
                Amount = withdrawalRequest.Amount,
                CreatedAt = DateTime.UtcNow,
                Wallet = wallet,
                Description = $"New Withdrawal Process with admin comment {adminComment ?? ""}",
                Type = TransactionType.Withdrawal,
                ExternalReferenceId = ExternalReferenceId ?? "",
            };

            await _unitOfWork.TransactionRepository.AddAsync(transaction);
            await _unitOfWork.CompleteAsync();

            return ServiceResult<WithDrawalRequest>.Success(withdrawalRequest);
        }

        public async Task<ServiceResult<IEnumerable<WithDrawalRequest>>> GetPendingRequestAsync(int? walletId = null)
        {
           
           var pendingRequests = await _unitOfWork.WithdrawalRequestsRepository.GetRequestsWithDetailsAsync(walletId: walletId??null,criteria:r=>r.Status==WithDrawalStatus.Pending);
            
           if (!pendingRequests.Any())
           {
                return ServiceResult<IEnumerable<WithDrawalRequest>>.Failure("No Pending WithdrawalRequests",404);
           }

            return ServiceResult<IEnumerable<WithDrawalRequest>>.Success(pendingRequests);
        }
    }
}
