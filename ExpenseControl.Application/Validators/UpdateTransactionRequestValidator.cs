using ExpenseControl.Application.DTOs;
using FluentValidation;

namespace ExpenseControl.Application.Validators;

public class UpdateTransactionRequestValidator : AbstractValidator<UpdateTransactionRequest>
{
    public UpdateTransactionRequestValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("A descrição é obrigatória.")
            .MinimumLength(2).WithMessage("A descrição deve ter pelo menos 2 caracteres.")
            .MaximumLength(200).WithMessage("A descrição deve ter no máximo 200 caracteres.");

        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("O valor é obrigatório.")
            .GreaterThan(0).WithMessage("O valor deve ser maior que zero.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("O tipo deve ser 'Income' ou 'Expense'.");
    }
}
