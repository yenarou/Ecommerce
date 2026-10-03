namespace ECommerce.Application.DTOs.Requests;

public record RegisterEmailRequest(string Email, string Password, string Username);