using AutoMapper;
using LogisticsHub.Application.DTOs;
using LogisticsHub.Application.Helpers;
using LogisticsHub.Application.Interfaces.Repositories;
using LogisticsHub.Application.Interfaces.Services;
using LogisticsHub.Domain.Entities;
using LogisticsHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Infrastructure.ServicesImplementation
{
    public class OrderService : IOrderService
    {
        private readonly ICartService _cartService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public OrderService(ICartService cartService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _cartService =cartService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ServiceResult<bool>> CancelOrderAsync(int orderId,string userId)
        {
            var existOrder = await _unitOfWork.OrderRepository.GetOrderWithDetailsAsync(orderId,userId);

            if (existOrder is null)
            {
                return ServiceResult<bool>.Failure("Order is not found",404);
            }

            if(existOrder.Status != OrderStatus.Pending)
            {
                return ServiceResult<bool>.Failure($"Order can't be cancelled as it was {existOrder.Status}");
            }

            var transaction= await _unitOfWork.BeginTransactionAsync();

            try
            {
                foreach (var item in existOrder.OrderItems)
                {
                    var product = await _unitOfWork.ProductRepository.GetByIdAsync(item.ProductId);
                    product!.StockQuantity += item.Quantity;
                    _unitOfWork.ProductRepository.Update(product);
                }

                existOrder.Status = OrderStatus.Cancelled;
                _unitOfWork.OrderRepository.Update(existOrder);

                await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync();

                return ServiceResult<bool>.Success(true);
            }

            catch (Exception ex)
            {
                await transaction.RollbackAsync();  
                return ServiceResult<bool>.Failure(ex.Message);
            }
           
        }

        
     
        public async Task<ServiceResult<Order>> CreateOrderAsync(string userId, OrderDetailsDto orderDto) //checkout order
        {
            //get user cart
            var result = await _cartService.GetCartAsync();
            var cart=result.Data;

            if (!cart!.CartItems.Any())
            {
                return ServiceResult<Order>.Failure("Cart is Empty !");
            }

            using var transaction=await _unitOfWork.BeginTransactionAsync();
            try
            {
                var order = new Order
                {
                    ShippingAddress = orderDto.ShippingAddress,
                    TotalAmount = cart.TotalItemsPrice,
                    CustomerId = userId,
                    CreatedAt = DateTime.UtcNow
                };

                decimal storeCommisionRate = 0;

                foreach (var item in cart.CartItems)
                {
                    var existproduct = await _unitOfWork.ProductRepository.GetByIdAsync(item.ProductId);

                    if (existproduct!.StockQuantity < item.Quantity)
                    {
                        return ServiceResult<Order>.Failure($"Unavaiable Stock Quantity for {existproduct.Name}.");
                    }

                    var orderItem = new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Price
                    };

                    //add orderitem to order
                    order.OrderItems.Add(orderItem);

                    //update product stockquantity
                    existproduct.StockQuantity -= item.Quantity;
                    _unitOfWork.ProductRepository.Update(existproduct);

                    //get the commissionRate for this store
                    var store = await _unitOfWork.StoreRepository.GetByIdAsync(existproduct!.StoreId);
                    storeCommisionRate = store!.CommissionRate;
                }

                order.PlatformCommission = storeCommisionRate * cart.TotalItemsPrice;

                //save to db
                await _unitOfWork.OrderRepository.AddAsync(order);
                await _unitOfWork.CompleteAsync();

                //clear tha cart after success
                await _cartService.ClearCartAsync();

                //successfull transaction 
                await transaction.CommitAsync();

                return ServiceResult<Order>.Success(order);
            }

            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                return ServiceResult<Order>.Failure(ex.Message);
            }
        }

        public async Task<ServiceResult<OrderDetailsDto>> GetOrderDetailsAsync(int orderId,string userId)
        {
            var order=await _unitOfWork.OrderRepository.GetOrderWithDetailsAsync(orderId,userId);

            if(order is null)
            {
                return ServiceResult<OrderDetailsDto>.Failure("Order is not found ");
            }

            var orderDetailsDto=_mapper.Map<OrderDetailsDto>(order);

            return ServiceResult<OrderDetailsDto>.Success(orderDetailsDto);
        }

        public async Task<ServiceResult<IEnumerable<OrderDto>>> GetUserOrdersAsync(string userId)
        {
            var orders=await _unitOfWork.OrderRepository.GetAllAsync(or=>or.CustomerId== userId);

            if (!orders.Any())
            {
                return ServiceResult<IEnumerable<OrderDto>>.Failure("No orders for this user");
            }

            var ordersDtos=_mapper.Map<IEnumerable<OrderDto>>(orders.OrderByDescending(o=>o.CreatedAt));

            return ServiceResult<IEnumerable<OrderDto>>.Success(ordersDtos);
        }

        public async Task<ServiceResult<bool>> UpdateOrderAsync(int orderId, OrderStatus newStatus,string userId)
        {
            var existOrder = await _unitOfWork.OrderRepository.GetOrderWithDetailsAsync(orderId,userId);

            if(existOrder is null)
            {
                return ServiceResult<bool>.Failure("Order is not found",404);
            }

            if((existOrder.Status==OrderStatus.Delivered)|| (existOrder.Status == OrderStatus.Cancelled))
            {
                return ServiceResult<bool>.Failure($"Invalid Status Update as the order was {existOrder.Status}");
            }

            if(newStatus==OrderStatus.Cancelled)
            {
                return await CancelOrderAsync(orderId,userId);
            }

            existOrder.Status = newStatus;
            _unitOfWork.OrderRepository.Update(existOrder);

            await _unitOfWork.CompleteAsync();

            return ServiceResult<bool>.Success(true);
        }
    }


}
