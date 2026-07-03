namespace ExpenseControl.Application.DTOs;

/// <summary>
/// DTO de saída com o resumo financeiro geral (todas as pessoas + totais).
/// </summary>
public class SummaryResponse
{
    /// <summary>
    /// Resumo individual de cada pessoa.
    /// </summary>
    public IEnumerable<PersonSummaryResponse> PersonSummaries { get; set; } = [];

    /// <summary>
    /// Total de receitas de todas as pessoas.
    /// </summary>
    public decimal TotalIncome { get; set; }

    /// <summary>
    /// Total de despesas de todas as pessoas.
    /// </summary>
    public decimal TotalExpenses { get; set; }

    /// <summary>
    /// Saldo líquido geral (receitas - despesas).
    /// </summary>
    public decimal NetBalance { get; set; }
}
