using ECommerce.Domain.Exceptions;

namespace ECommerce.Api.Middleware;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (status, title, detail) = exception switch
        {
            // 404 Not Found
            ProductNotFoundException e =>
                (404, "Product Not Found", e.Message),

            UserNotFoundException e =>
                (404, "User Not Found", e.Message),

            // 400 Bad Request
            DifferentCurrenciesException e =>
                (400, "Different Currencies", e.Message),

            InsufficientQuantityException e =>
                (400, "Insufficient Quantity", e.Message),

            InvalidAddressException e =>
                (400, "Invalid Address", e.Message),

            InvalidCredentialsException e =>
                (400, "Invalid Credentials", e.Message),

            InvalidCurrencyException e =>
                (400, "Invalid Currency", e.Message),

            InvalidEmailException e =>
                (400, "Invalid Email", e.Message),

            // 409 Conflict
            EmailAlreadyRegisteredException e =>
                (409, "Email Already Registered", e.Message),

            InvalidOrderStatusTransition e =>
                (409, "Invalid Order Status Transition", e.Message),


            DomainException e =>
                (400, "Domain Error", e.Message),

            // 500 Internal Server Error
            _ => (
                500,
                "Internal Server Error",
                $"{exception.Message} | Inner: {exception.InnerException?.Message} | {exception}"
            )
        };

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json";

        var problem = new
        {
            title,
            status,
            detail
        };

        await context.Response.WriteAsJsonAsync(problem);
    }
}