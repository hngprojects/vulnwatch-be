namespace Application.Features.Auth.DTOs;

public record RegisterRequest(string Email, string Password)
{
    public static RegisterRequest Create(string email, string password) => new(email, password);
}
