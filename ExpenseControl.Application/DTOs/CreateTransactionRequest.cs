using System.ComponentModel.DataAnnotations;
using ExpenseControl.Domain.Enums;

namespace ExpenseControl.Application.DTOs;

/// <summary>
/// DTO de entrada para criação de uma transação.
/// </summary>
public class CreateTransactionRequest
{
    /// <summary>
    /// Descrição da transação.
    /// </summary>
    [Required(ErrorMessage = "A descrição é obrigatória.")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Valor monetário da transação.
    /// </summary>
    [Required(ErrorMessage = "O valor é obrigatório.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
    public decimal Value { get; set; }

    /// <summary>
    /// Tipo da transação: 0 = Receita (Income), 1 = Despesa (Expense).
    /// </summary>
    [Required(ErrorMessage = "O tipo da transação é obrigatório.")]
    public TransactionType Type { get; set; }

    /// <summary>
    /// Identificador da pessoa associada à transação.
    /// </summary>
    [Required(ErrorMessage = "O identificador da pessoa é obrigatório.")]
    public Guid PersonId { get; set; }
}
