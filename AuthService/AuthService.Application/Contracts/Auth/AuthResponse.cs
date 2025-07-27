namespace AuthService.Application.Contracts.Auth;

public record AuthResponse(Guid Id, string Username, string Email, string Token);