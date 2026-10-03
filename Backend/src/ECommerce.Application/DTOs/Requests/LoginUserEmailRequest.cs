namespace ECommerce.Application.DTOs.Requests;

public record RegisterUserEmailRequest(string Email, string Password, string Username);