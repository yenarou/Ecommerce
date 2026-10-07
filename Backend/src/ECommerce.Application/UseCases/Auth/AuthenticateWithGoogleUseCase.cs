using ECommerce.Application.DTOs.Requests;
using ECommerce.Application.DTOs.Responses;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Models;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Application.UseCases.Auth;

public class AuthenticateWithGoogleUseCase(
    IGoogleAuthService googleAuthService,
    IUserRepository userRepository,
    ITokenService tokenService)
{
    public async Task<AuthResponse> Execute(AuthenticateWithGoogleRequest request)
    {
        var tokens =
            await googleAuthService.ExchangeAuthorizationCodeAsync(request.AuthorizationCode, request.RedirectUri);
        var userInfo = await googleAuthService.GetUserInfoAsync(tokens.AccessToken);
        var user = await userRepository.GetByGoogleId(userInfo.GoogleId);

        var email = Email.Create(userInfo.Email);

        if (user == null)
        {
            user = User.CreateGoogleUser(userInfo.Username, email, userInfo.GoogleId);
            await userRepository.Save(user);
        }

        var token = tokenService.Generate(user);
        return new AuthResponse(token, user.Id, user.Name);
    }
}