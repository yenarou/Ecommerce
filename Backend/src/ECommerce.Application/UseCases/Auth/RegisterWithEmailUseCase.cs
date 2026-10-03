using ECommerce.Application.DTOs.Requests;
using ECommerce.Application.DTOs.Responses;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Models;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.ValueObjects;
using Tareino.Application.Interfaces;

namespace ECommerce.Application.UseCases.Auth;

public class RegisterWithEmailUseCase(
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IUserRepository userRepository)
{
    public async Task<AuthResponse> Execute(RegisterWithEmailRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
            throw new ArgumentException("La contraseña debe tener al menos 6 caracteres.");
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException("El correo electrónico es obligatorio.");
        var existingUser = await userRepository.GetByEmail(request.Email);
        if (existingUser != null) throw new EmailAlreadyRegisteredException(request.Email);

        var hashedPassword = passwordHasher.Hash(request.Password);
        var email = Email.Create(request.Email);
        var user = User.CreateLocalUser(request.Username, email, hashedPassword);
        await userRepository.Save(user);

        var token = tokenService.Generate(user);
        return new AuthResponse(token, user.Id, user.Name);
    }
}