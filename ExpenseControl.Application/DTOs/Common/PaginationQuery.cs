using System.ComponentModel.DataAnnotations;

namespace ExpenseControl.Application.DTOs.Common;

/// <summary>
/// Parâmetros base para qualquer consulta paginada.
/// </summary>
public class PaginationQuery
{
    private const int MaxPageSize = 50;
    private int _pageSize = 10;

    /// <summary>
    /// Número da página atual (1-indexado). Padrão é 1.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "A página deve ser maior ou igual a 1.")]
    public int Page { get; set; } = 1;

    /// <summary>
    /// Quantidade de itens por página. Padrão é 10. O limite máximo é 50.
    /// </summary>
    [Range(1, 50, ErrorMessage = "O tamanho da página deve estar entre 1 e 50.")]
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }
}
