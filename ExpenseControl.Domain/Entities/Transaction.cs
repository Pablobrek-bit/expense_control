using ExpenseControl.Domain.Enums;

namespace ExpenseControl.Domain.Entities;

/// <summary>
/// Representa uma transação financeira associada a uma pessoa.
/// </summary>
public class Transaction
{
    /// <summary>
    /// Identificador único da transação (gerado automaticamente).
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Descrição da transação.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Valor monetário da transação.
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// Tipo da transação (Receita ou Despesa).
    /// </summary>
    public TransactionType Type { get; set; }

    /// <summary>
    /// Identificador da pessoa associada a esta transação.
    /// </summary>
    public Guid PersonId { get; set; }

    /// <summary>
    /// Pessoa associada a esta transação (propriedade de navegação).
    /// </summary>
    public Person Person { get; set; } = null!;
}
