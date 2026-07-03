using ExpenseControl.Application.DTOs;
using ExpenseControl.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseControl.Infrastructure.Web.Controllers;

/// <summary>
/// Controller para gerenciamento de Transações.
/// Driving Adapter — recebe requisições HTTP e delega para o caso de uso.
/// </summary>
[ApiController]
[Route("api/transactions")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    /// <summary>
    /// Lista todas as transações cadastradas.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransactionResponse>>> GetAll()
    {
        var transactions = await _transactionService.GetAllAsync();
        return Ok(transactions);
    }

    /// <summary>
    /// Cria uma nova transação.
    /// Valida se a pessoa existe e se a regra de menor de idade é respeitada.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TransactionResponse>> Create([FromBody] CreateTransactionRequest request)
    {
        try
        {
            var transaction = await _transactionService.CreateAsync(request);
            return CreatedAtAction(nameof(GetAll), new { id = transaction.Id }, transaction);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
