using ExpenseControl.Domain.Enums;

namespace ExpenseControl.Application.DTOs;

/// <summary>
/// DTO de saída para retorno de dados de uma transação dentro do contexto dos detalhes de uma pessoa.
/// Não inclui PersonId e PersonName para evitar redundância.
/// </summary>
public class PersonTransactionResponse
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public TransactionType Type { get; set; }
}
