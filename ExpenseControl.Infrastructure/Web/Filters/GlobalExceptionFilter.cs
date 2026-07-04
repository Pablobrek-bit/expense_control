using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ExpenseControl.Infrastructure.Web.Filters;

/// <summary>
/// Filtro global de exceções — equivalente ao @ControllerAdvice do Java.
/// Intercepta todas as exceções não tratadas nos Controllers e retorna
/// respostas HTTP padronizadas com mensagens amigáveis.
/// </summary>
public class GlobalExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var (statusCode, message) = context.Exception switch
        {
            KeyNotFoundException ex => (StatusCodes.Status404NotFound, ex.Message),
            InvalidOperationException ex => (StatusCodes.Status400BadRequest, ex.Message),
            ArgumentException ex => (StatusCodes.Status400BadRequest, ex.Message),
            _ => (StatusCodes.Status500InternalServerError, "Ocorreu um erro interno no servidor.")
        };

        context.Result = new ObjectResult(new
        {
            status = statusCode,
            message
        })
        {
            StatusCode = statusCode
        };

        context.ExceptionHandled = true;
    }
}
