namespace ECommerce.Application.DTOs.Requests;

public record RegisterWithEmailRequest(string Username, string Email, string Password);