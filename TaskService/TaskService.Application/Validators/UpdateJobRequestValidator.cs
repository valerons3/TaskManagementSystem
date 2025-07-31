using FluentValidation;
using TaskService.Application.Contracts.Jobs;

namespace TaskService.Application.Validators;

public class UpdateJobRequestValidator : AbstractValidator<UpdateJobRequest>
{
    public UpdateJobRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200)
            .When(x => x.Title is not null);

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => x.Description is not null);
        
        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status is not null);

    }
}