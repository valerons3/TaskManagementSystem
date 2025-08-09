namespace AuthService.Application.Contracts.Auth;

public record LoginRequest(string Identifier, string Password);