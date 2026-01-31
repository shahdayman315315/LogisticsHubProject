using LogisticsHub.Application.Helpers;
using LogisticsHub.Infrastructure.Repositories.RepositoriesInterfaces;
using LogisticsHub.Application.Services.ServicesInterfaces;
using LogisticsHub.Domain.Entities;
using Microsoft.AspNetCore.Http;
using LogisticsHub.Application.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Application.Services.ServicesImplementation
{
    public class CartService : ICartService
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly IUnitOfWork _unitOfWork;
        public CartService(IHttpContextAccessor accessor, IUnitOfWork unitOfWork)
        {
            _accessor = accessor;
            _unitOfWork = unitOfWork;
        }
        public async Task<ServiceResult<Cart>> AddToCart(int productId, int Quantity)
        {
            var cart = _accessor.HttpContext.Session.GetObjectFromJson<Cart>("Cart")??new Cart();

            var product = await _unitOfWork.ProductRepository.GetByIdAsync(productId);

            if (product is null)
            {
                return ServiceResult<Cart>.Failure("Product is not found", 404);
            }

            var cartItem = new CartItem
            {
                ProductId = productId,
                ProductName = product.Name,
                Quantity = Quantity,
                Price = product.Price
            };


            cart.AddItem(cartItem);

            _accessor.HttpContext.Session.SetObjectFromJson("Cart", cart);

            return ServiceResult<Cart>.Success(cart);

        }

        public async Task<ServiceResult<bool>> ClearCartAsync()
        {
            _accessor.HttpContext.Session.Remove("Cart");

            return await Task.FromResult(ServiceResult<bool>.Success(true));
        }

        public async Task<ServiceResult<Cart>> GetCartAsync()
        {
            var cart = _accessor.HttpContext.Session.GetObjectFromJson<Cart>("Cart") ?? new Cart();

            return await Task.FromResult(ServiceResult<Cart>.Success(cart));
        }

        public async Task<ServiceResult<int>> GetTotalCountAsync()
        {
            var cart = _accessor.HttpContext.Session.GetObjectFromJson<Cart>("Cart") ?? new Cart();

            return await Task.FromResult(ServiceResult<int>.Success(cart.TotalQuantity));
        }
        public async Task<ServiceResult<bool>> RemoveItemAsync(int productId)
        {
            var cart = _accessor.HttpContext.Session.GetObjectFromJson<Cart>("Cart");

            if(cart is null)
            {
                return ServiceResult<bool>.Failure("Cart is empty");
            }

            var existItem= cart.CartItems.FirstOrDefault(c=>c.ProductId==productId);

            if(existItem is null)
            {
                return ServiceResult<bool>.Failure("Product is not found",404);
            }

            cart.RemoveItem(existItem);

            _accessor.HttpContext.Session.SetObjectFromJson("Cart", cart);

            return await Task.FromResult(ServiceResult<bool>.Success(true));
        }

        public async Task<ServiceResult<bool>> UpdateQuantityAsync(int productId, int Quantity)
        {
            var cart = _accessor.HttpContext.Session.GetObjectFromJson<Cart>("Cart");

            if (cart is null)
            {
                return ServiceResult<bool>.Failure("Cart is empty");
            }

            var existItem = cart.CartItems.FirstOrDefault(c => c.ProductId == productId);

            if (existItem is null)
            {
                return ServiceResult<bool>.Failure("Product is not found", 404);
            }

            cart.UpdateItem(productId, Quantity);

            _accessor.HttpContext.Session.SetObjectFromJson("Cart", cart);

            return await Task.FromResult(ServiceResult<bool>.Success(true));

        }
    }
}
