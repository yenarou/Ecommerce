using ECommerce.Application.DTOs.Requests;
using ECommerce.Application.Interfaces;

namespace ECommerce.Application.UseCases.Checkout;

public class UpdateCartUseCase(ICurrentUser currentUser)
{
    public async Task<> Execute(UpdateCartRequest request)
    {
        var user = currentUser.User;
        
        
    }
}