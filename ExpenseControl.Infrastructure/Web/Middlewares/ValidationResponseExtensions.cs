using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseControl.Infrastructure.Web.Middlewares;

public static class ValidationResponseExtensions
{
    public static IMvcBuilder ConfigureValidationResponse(this IMvcBuilder builder)
    {
        return builder.ConfigureApiBehaviorOptions(options =>
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

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Erro de Validação",
                    Detail = "Um ou mais erros de validação ocorreram.",
                    Instance = context.HttpContext.Request.Path
                };

                problemDetails.Extensions.Add("errors", errors);

                return new BadRequestObjectResult(problemDetails);
            };
        });
    }
}
