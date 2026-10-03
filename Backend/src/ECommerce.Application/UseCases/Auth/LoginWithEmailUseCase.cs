
using ECommerce.Application.DTOs.Requests;
using ECommerce.Application.DTOs.Responses;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.ValueObjects;
using Tareino.Application.Interfaces;

namespace ECommerce.Application.UseCases.Auth;

public class LoginWithEmailUseCase(
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IUserRepository userRepository)
{
    public async Task<AuthResponse> Execute(LoginEmailRequest request)
    {
        var existingUser = await userRepository.GetByEmail(request.Email);
        
        if (existingUser == null) throw new UserNotFoundException(request.Email);
        
        if(existingUser.AuthProvider != AuthProvider.Local)
            #warning Implentar excepcion para auth provider distinto
            throw new InvalidCredentialsException(request.Email);

        if (existingUser.PasswordHash != null && !passwordHasher.Verify(request.Password, existingUser.PasswordHash))
            throw new InvalidCredentialsException(request.Email);

        var token = tokenService.Generate(existingUser);
        return new AuthResponse(token, existingUser.Id, existingUser.Name);
    }
}