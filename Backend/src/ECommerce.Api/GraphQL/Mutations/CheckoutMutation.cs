using ECommerce.Application.DTOs.Requests;
using ECommerce.Application.DTOs.Responses;
using ECommerce.Application.UseCases.Checkout;

namespace ECommerce.Api.GraphQL.Mutations;

[ExtendObjectType(OperationTypeNames.Mutation)]
public class CheckoutMutation
{
    public async Task<bool> UpdateCart(
        CartRequest request,
        UpdateCartUseCase useCase)
    {
        await useCase.Execute(request);

        return true;
    }

    public async Task<CreateOrderResponse> CreateOrder(
        CreateOrderRequest request,
        CreateOrderUseCase useCase)
    {
        return await useCase.Execute(request);
    }
}