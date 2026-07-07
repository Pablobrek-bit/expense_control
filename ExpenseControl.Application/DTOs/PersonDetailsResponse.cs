namespace ExpenseControl.Application.DTOs;

/// <summary>
/// DTO de saída que representa os detalhes de uma pessoa, incluindo suas transações.
/// </summary>
public class PersonDetailsResponse
{
    /// <summary>
    /// Identificador da pessoa.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome da pessoa.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Idade da pessoa.
    /// </summary>
    public int Age { get; set; }

    /// <summary>
    /// Lista de transações (receitas e despesas) realizadas pela pessoa.
    /// </summary>
    public IEnumerable<PersonTransactionResponse> Transactions { get; set; } = [];
}
