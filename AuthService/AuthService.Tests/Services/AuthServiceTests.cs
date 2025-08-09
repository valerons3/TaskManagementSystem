using AuthService.Application.Contracts.Auth;
using AuthService.Application.Exceptions;
using AuthService.Application.Interfaces;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.Entities;
using FluentAssertions;
using Moq;

namespace AuthService.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> userRepoMock = new();
    private readonly Mock<IJwtTokenGenerator> tokenGenMock = new();
    private readonly Mock<IPasswordHasher> passwordHasherMock = new();

    private readonly AuthService.Infrastructure.Services.AuthService authService;

    public AuthServiceTests()
    {
        authService = new AuthService.Infrastructure.Services.AuthService(
            userRepoMock.Object,
            tokenGenMock.Object,
            passwordHasherMock.Object);
    }
    
    [Fact]
    public async Task RegisterAsync_Should_Throw_When_EmailExists()
    {
        var request = new RegisterRequest("existing@mail.com", "password123", "user");
        userRepoMock.Setup(r => r.ExistsByIdentifierAsync(request.Email, request.Username)).ReturnsAsync(true);

        Func<Task> act = async () => await authService.RegisterAsync(request);

        await act.Should().ThrowAsync<UserAlreadyExistsException>();
    }
    
    
    [Fact]
    public async Task LoginAsync_Should_Throw_When_UserNotFound()
    {
        var request = new LoginRequest("notfound@mail.com", "pass");
        userRepoMock.Setup(r => r.GetByIdentifierAsync(request.Identifier)).ReturnsAsync((User?)null);

        Func<Task> act = async () => await authService.LoginAsync(request);

        await act.Should().ThrowAsync<InvalidCredentialsException>();
    }
    
    [Fact]
    public async Task LoginAsync_Should_Throw_When_PasswordInvalid()
    {
        var user = new User { Email = "mail@mail.com", PasswordHash = "hashed" };
        var request = new LoginRequest(user.Email, "wrongpass");

        userRepoMock.Setup(r => r.GetByIdentifierAsync(user.Email)).ReturnsAsync(user);
        passwordHasherMock.Setup(h => h.Verify(request.Password, user.PasswordHash)).Returns(false);

        Func<Task> act = async () => await authService.LoginAsync(request);

        await act.Should().ThrowAsync<InvalidCredentialsException>();
    }
    
    [Fact]
    public async Task LoginAsync_Should_ReturnToken_When_CredentialsAreValid()
    {
        var user = new User { Id = Guid.NewGuid(), Username = "User", Email = "mail@mail.com", PasswordHash = "hashed" };
        var request = new LoginRequest(user.Email, "pass");

        userRepoMock.Setup(r => r.GetByIdentifierAsync(user.Email)).ReturnsAsync(user);
        passwordHasherMock.Setup(h => h.Verify(request.Password, user.PasswordHash)).Returns(true);
        tokenGenMock.Setup(t => t.GenerateToken(user)).Returns("valid_token");

        var result = await authService.LoginAsync(request);

        result.Should().NotBeNull();
        result.Token.Should().Be("valid_token");
        result.Email.Should().Be(user.Email);
    }
}