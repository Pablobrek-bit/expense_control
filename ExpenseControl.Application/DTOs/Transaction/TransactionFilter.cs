using ExpenseControl.Application.DTOs.Common;
using ExpenseControl.Domain.Enums;

namespace ExpenseControl.Application.DTOs.Transaction;

/// <summary>
/// Parâmetros de filtro e paginação para listagem de transações.
/// </summary>
public class TransactionFilter : PaginationQuery
{
    /// <summary>
    /// Filtra pelo tipo da transação (Income ou Expense).
    /// </summary>
    public TransactionType? Type { get; set; }

    /// <summary>
    /// Filtra pelas transações de uma pessoa específica.
    /// </summary>
    public Guid? PersonId { get; set; }
}
