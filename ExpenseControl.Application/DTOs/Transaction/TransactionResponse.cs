using ExpenseControl.Domain.Enums;

namespace ExpenseControl.Application.DTOs;

/// <summary>
/// DTO de saída para retorno de dados de uma transação.
/// </summary>
public class TransactionResponse
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public TransactionType Type { get; set; }
    public Guid PersonId { get; set; }
    public string PersonName { get; set; } = string.Empty;
}
