using LogisticsHub.Application.DTOs;
using LogisticsHub.Application.Helpers;
using LogisticsHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Application.Services.ServicesInterfaces
{
    public interface IWithdrawalRequestService
    {
        Task<ServiceResult<WithDrawalRequest>> WithdrawalRequestAsync(string userId, WithdrawaRequestDto request);
        Task<ServiceResult<WithDrawalRequest>> ApproveWithdrawalRequest(int requestId, string? adminComment = null, string? ExternalReferneceId = null);
        Task<ServiceResult<WithDrawalRequest>> RejectWithdrawalRequest(int requestId, string? rejectionReason = null);
        Task<ServiceResult<IEnumerable<WithDrawalRequest>>> GetPendingRequestAsync(int? walletId = null);

    }
}
