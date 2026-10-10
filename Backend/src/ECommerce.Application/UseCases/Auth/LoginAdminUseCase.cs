using ECommerce.Application.DTOs.Responses;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Models;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Application.UseCases.Auth;

public class LoginAdminUseCase(IPasswordHasher passwordHasher, ITokenService tokenService, IUserRepository userRepository){
    public async Task<AuthResponse> Execute(string email, string password)
    {
        var user = await userRepository.GetByEmail(email);

        if(user is null || user.AuthProvider != AuthProvider.Local || 
        string.IsNullOrWhiteSpace(user.PasswordHash) || !passwordHasher.Verify(password, user.PasswordHash))
        {
            throw new InvalidCredentialsException(email);
        }

        if(user.Role != UserRole.Admin)
        {
            throw new UnauthorizedAccessException("El usuario no es administrador.");
        }

        var token = tokenService.Generate(user);

        return new AuthResponse(token, user.Id, user.Name, user.Role.ToString());
    }
}