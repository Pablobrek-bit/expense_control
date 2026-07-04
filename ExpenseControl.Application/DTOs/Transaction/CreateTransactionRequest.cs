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
    [MinLength(2, ErrorMessage = "A descrição deve ter pelo menos 2 caracteres.")]
    [MaxLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres.")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Valor monetário da transação.
    /// </summary>
    [Required(ErrorMessage = "O valor é obrigatório.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
    public decimal Value { get; set; }

    /// <summary>
    /// Tipo da transação: Income (Receita) ou Expense (Despesa).
    /// </summary>
    [Required(ErrorMessage = "O tipo da transação é obrigatório.")]
    [EnumDataType(typeof(TransactionType), ErrorMessage = "O tipo deve ser 'Income' ou 'Expense'.")]
    public TransactionType Type { get; set; }

    /// <summary>
    /// Identificador (UUID) da pessoa associada à transação.
    /// </summary>
    [Required(ErrorMessage = "O identificador da pessoa é obrigatório.")]
    public Guid PersonId { get; set; }
}

