using LogisticsHub.Application.DTOs;
using LogisticsHub.Application.Helpers;
using LogisticsHub.Infrastructure.Repositories.RepositoriesInterfaces;
using LogisticsHub.Application.Services.ServicesInterfaces;
using LogisticsHub.Domain.Entities;
using LogisticsHub.Domain.Enums;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Application.Services.ServicesImplementation
{
    public class PaymentService : IPaymentService
    {
        private readonly StripeSettings _stripeSettings;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IOptions<StripeSettings> stripeSettings, IUnitOfWork unitOfWork)
        {
            _stripeSettings = stripeSettings.Value;
            _unitOfWork = unitOfWork;
        }

       
        public async Task<ServiceResult<PaymentResultDto>> CreateCheckOutSessionAsync(Order order)
        {
            try
            {

                StripeConfiguration.ApiKey = _stripeSettings.SecretKey;

                var lineItems = new List<SessionLineItemOptions>();

                foreach (var item in order.OrderItems)
                {
                    lineItems.Add(new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.UnitPrice * 100),
                            Currency = "egp",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product!.Name
                            }

                        },

                        Quantity = item.Quantity

                    });
                }

                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = lineItems,
                    Mode = "payment",
                    SuccessUrl = "http://localhost:3000/payment-success?sessionId={CHECKOUT_SESSION_ID}",
                    CancelUrl = $"http://localhost:3000/payment-cancel?orderId={order.Id}",
                    Metadata = new Dictionary<string, string>
                    {
                      { "OrderId", order.Id.ToString() }
                    }
                };

                var service = new SessionService();
                var session = await service.CreateAsync(options);

                return ServiceResult<PaymentResultDto>.Success(new PaymentResultDto
                {
                    SessionId = session.Id,
                    Url = session.Url
                });
            }
            catch(StripeException e)
            {
                return ServiceResult<PaymentResultDto>.Failure($"Stripe Error: {e.Message}");
            }
            catch (Exception ex)
            {
                return ServiceResult<PaymentResultDto>.Failure($"Unexpected Error: {ex.Message}");

            }
        }

        public async Task<ServiceResult<Order>> CheckPaymentAsync(string sesstionId)
        {
            StripeConfiguration.ApiKey=_stripeSettings.SecretKey;

            var service = new SessionService();

            var session= await service.GetAsync(sesstionId);

            if (!(session.PaymentStatus == "paid"))
            {
                return ServiceResult<Order>.Failure("Payment not confirmed");
            }

            var orderId = int.Parse(session.Metadata["OrderId"]);
            var order=await _unitOfWork.OrderRepository.GetByIdAsync(orderId);

            if(order is null)
            {
                return  ServiceResult<Order>.Failure("Order is not found ");
            }

            order.Status = OrderStatus.Confirmed;
            order.StripeSessionId=sesstionId;

            _unitOfWork.OrderRepository.Update(order);
            await UpdateMerchantWalletAsync(order);

            await _unitOfWork.CompleteAsync();

            return ServiceResult<Order>.Success(order);
        }


        public async Task UpdateMerchantWalletAsync(Order order)
        {


            var storeId = order.OrderItems.FirstOrDefault()!.Product!.StoreId;

            var merchantId=(await _unitOfWork.StoreRepository.GetByIdAsync(storeId)).MerchantId;


            var userIdForMerchant= (await _unitOfWork.MerchantRepository.GetByIdAsync(merchantId)).UserId;

            var wallet=await _unitOfWork.WalletRepository.GetFirstAsync(w=>w.UserId== userIdForMerchant);

            var merchantShare = order.TotalAmount - order.PlatformCommission;

            wallet!.Balance += merchantShare;

            var transaction = new Transaction
            {
                Wallet = wallet,
                Amount = merchantShare,
                Description = $"New Deposite -> {merchantShare}",
                ExternalReferenceId = order.StripeSessionId,
                CreatedAt = DateTime.UtcNow,
                Type = TransactionType.Deposite
            };

            await _unitOfWork.TransactionRepository.AddAsync(transaction);
   

        }

    }
}
