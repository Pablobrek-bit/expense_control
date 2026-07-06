using ExpenseControl.Application.DTOs;
using ExpenseControl.Application.DTOs.Transaction;
using ExpenseControl.Application.DTOs.Common;
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
    /// Retorna uma lista paginada de transações, com suporte a filtros.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<TransactionResponse>>> GetAll([FromQuery] TransactionFilter filter)
    {
        var transactions = await _transactionService.GetAllAsync(filter);
        return Ok(transactions);
    }

    /// <summary>
    /// Cria uma nova transação.
    /// Valida se a pessoa existe e se a regra de menor de idade é respeitada.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TransactionResponse>> Create([FromBody] CreateTransactionRequest request)
    {
        var transaction = await _transactionService.CreateAsync(request);
        return CreatedAtAction(nameof(GetAll), new { id = transaction.Id }, transaction);
    }

    /// <summary>
    /// Atualiza uma transação existente.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TransactionResponse>> Update(Guid id, [FromBody] UpdateTransactionRequest request)
    {
        var updatedTransaction = await _transactionService.UpdateAsync(id, request);
        return Ok(updatedTransaction);
    }

    /// <summary>
    /// Deleta uma transação pelo seu ID.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _transactionService.DeleteAsync(id);
        return NoContent();
    }
}
