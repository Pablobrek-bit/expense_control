using ExpenseControl.Application.DTOs;
using FluentValidation;

namespace ExpenseControl.Application.Validators;

public class CreatePersonRequestValidator : AbstractValidator<CreatePersonRequest>
{
    public CreatePersonRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MinimumLength(2).WithMessage("O nome deve ter pelo menos 2 caracteres.")
            .MaximumLength(100).WithMessage("O nome deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Age)
            .NotEmpty().WithMessage("A idade é obrigatória.")
            .InclusiveBetween(1, 150).WithMessage("A idade deve ser entre 1 e 150.");
    }
}
