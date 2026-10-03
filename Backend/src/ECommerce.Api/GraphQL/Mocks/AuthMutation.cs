using ECommerce.Application.DTOs.Requests;
using ECommerce.Application.DTOs.Responses;
using ECommerce.Application.UseCases.Auth;

namespace ECommerce.Api.GraphQL.Mocks;

public class AuthMutation
{
    public async Task<AuthResponse> Register(
        RegisterWithEmailRequest request,
        RegisterWithEmailUseCase useCase)
    {
        return await useCase.Execute(request);
    }

    public async Task<AuthResponse> Login(
        LoginEmailRequest request,
        LoginWithEmailUseCase useCase)
    {
        return await useCase.Execute(request);
    }

    public async Task<AuthResponse> Google(
        AuthenticateWithGoogleRequest request,
        AuthenticateWithGoogleUseCase useCase)
    {
        return await useCase.Execute(request);
    }
}