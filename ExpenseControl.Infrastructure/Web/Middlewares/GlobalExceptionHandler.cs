using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseControl.Infrastructure.Web.Middlewares;

/// <summary>
/// Tratamento Global de Exceções utilizando a interface IExceptionHandler do .NET 8+.
/// Retorna os erros no padrão RFC 7807 (Problem Details), que é o formato exigido
/// por integrações de APIs RESTful modernas.
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Ocorreu uma exceção não tratada: {Message}", exception.Message);

        var (statusCode, title, detail) = exception switch
        {
            KeyNotFoundException ex => (StatusCodes.Status404NotFound, "Recurso não encontrado", ex.Message),
            InvalidOperationException ex => (StatusCodes.Status400BadRequest, "Operação inválida", ex.Message),
            ArgumentException ex => (StatusCodes.Status400BadRequest, "Argumento inválido", ex.Message),
            _ => (StatusCodes.Status500InternalServerError, "Erro Interno do Servidor", "Ocorreu um erro inesperado. Tente novamente mais tarde.")
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
