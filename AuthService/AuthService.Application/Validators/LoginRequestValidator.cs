using AuthService.Application.Contracts.Auth;
using FluentValidation;

namespace AuthService.Application.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Identifier)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}