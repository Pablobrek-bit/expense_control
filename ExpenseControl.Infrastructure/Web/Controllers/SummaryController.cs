using ExpenseControl.Application.DTOs;
using ExpenseControl.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseControl.Infrastructure.Web.Controllers;

/// <summary>
/// Controller para consulta de totais financeiros.
/// Driving Adapter — retorna o resumo de receitas, despesas e saldo.
/// </summary>
[ApiController]
[Route("api/summary")]
public class SummaryController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public SummaryController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    /// <summary>
    /// Retorna o resumo financeiro de todas as pessoas,
    /// incluindo receitas, despesas e saldo individual e total geral.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<SummaryResponse>> GetSummary()
    {
        var summary = await _transactionService.GetSummaryAsync();
        return Ok(summary);
    }
}
