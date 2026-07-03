namespace ExpenseControl.Domain.Enums;

/// <summary>
/// Representa o tipo de uma transação financeira.
/// </summary>
public enum TransactionType
{
    /// <summary>
    /// Receita — entrada de dinheiro.
    /// </summary>
    Income = 0,

    /// <summary>
    /// Despesa — saída de dinheiro.
    /// </summary>
    Expense = 1
}
