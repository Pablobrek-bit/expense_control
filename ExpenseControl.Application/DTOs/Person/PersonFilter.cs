using ExpenseControl.Application.DTOs.Common;

namespace ExpenseControl.Application.DTOs.Person;

/// <summary>
/// Parâmetros de filtro e paginação para listagem de pessoas.
/// </summary>
public class PersonFilter : PaginationQuery
{
    /// <summary>
    /// Filtra pelo nome parcial da pessoa.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Filtra pela idade exata.
    /// </summary>
    public int? Age { get; set; }
}
