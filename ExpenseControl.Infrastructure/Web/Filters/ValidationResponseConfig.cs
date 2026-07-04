using Microsoft.AspNetCore.Mvc;

namespace ExpenseControl.Infrastructure.Web.Filters;

/// <summary>
/// Configuração customizada para respostas de erro de validação.
/// Transforma as mensagens cruas do .NET em mensagens amigáveis em português.
/// </summary>
public static class ValidationResponseConfig
{
    public static IMvcBuilder ConfigureValidationResponse(this IMvcBuilder builder)
    {
        builder.ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .ToDictionary(
                        e => e.Key,
                        e => e.Value!.Errors.Select(err =>
                        {
                            if (err.ErrorMessage.Contains("could not be converted to"))
                            {
                                if (err.ErrorMessage.Contains("TransactionType"))
                                    return "O tipo deve ser 'Income' ou 'Expense'.";
                                if (err.ErrorMessage.Contains("Guid"))
                                    return "O campo deve ser um UUID válido.";
                                return "Valor inválido para este campo.";
                            }
                            return err.ErrorMessage;
                        }).ToArray()
                    );

                return new BadRequestObjectResult(new
                {
                    status = 400,
                    message = "Erro de validação.",
                    errors
                });
            };
        });

        return builder;
    }
}
