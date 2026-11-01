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
    public async Task<PayOrderResponse> PayOrder(
        PayOrderRequest request,
        PayOrderUseCase useCase)
    {
        return await useCase.Execute(request);
    }

    //PagoAmigo
    public async Task<string> CreatePayPalOrder(
        Guid orderId,
        CreatePayPalOrderUseCase useCase)
        {
            return await useCase.Execute(orderId);
        }

    public async Task<PayOrderResponse> CapturePayPalOrder(
        Guid orderId,
        string payPalOrderId,
        CapturePayPalOrderUseCase useCase)
        {
            return await useCase.Execute(orderId, payPalOrderId);
        }

    public async Task<CartResponse> GetCart(GetActiveCartUseCase useCase)
    {
        return await useCase.Execute();
    }
}