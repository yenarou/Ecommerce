using ECommerce.Application.DTOs.Responses;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Mappers;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Repositories;

namespace ECommerce.Application.UseCases.Checkout;

public class GetUserOrderHistoryUseCase(ICurrentUser currentUser, IOrderRepository orderRepository, IProductRepository productRepository)
{
    public async Task<ICollection<OrderResponse>> Execute()
    {
        var user = await currentUser.GetUserAsync();

        var orders = await orderRepository.GetByUserId(user.Id);

        foreach (var order in orders)
        {
            foreach (var item in order.Items)
            {
                if (item.ProductId  == Guid.Empty)
                    throw new InvalidOperationException(
                        $"Order item {item.Id} has no stored product ID.");
            }
        }

        var productImages = new Dictionary<Guid, Image?>();

        foreach (var item in orders.SelectMany(order => order.Items))
        {
            if (productImages.ContainsKey(item.ProductId))
                continue;

            var product = await productRepository.GetById(item.ProductId);
            var images = product?.Images;
            productImages[item.ProductId] =
                images?.FirstOrDefault(image => image.Id == item.ProductImageId)
                ?? images?.FirstOrDefault();
        }

        return OrderMapper.ToResponse(orders, productImages);
    }
}