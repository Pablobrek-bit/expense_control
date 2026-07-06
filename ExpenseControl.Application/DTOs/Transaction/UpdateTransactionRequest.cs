using ExpenseControl.Domain.Enums;

namespace ExpenseControl.Application.DTOs;

/// <summary>
/// DTO de entrada para atualização de uma transação.
/// </summary>
public class UpdateTransactionRequest
{
    /// <summary>
    /// Descrição da transação.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Valor monetário da transação.
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// Tipo da transação: Income (Receita) ou Expense (Despesa).
    /// </summary>
    public TransactionType Type { get; set; }
}
