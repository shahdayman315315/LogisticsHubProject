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
    public interface IPaymentService
    {
        Task<ServiceResult<PaymentResultDto>> CreateCheckOutSessionAsync(Order order);
        Task<ServiceResult<Order>> CheckPaymentAsync(string sesstionId);
    }
}
