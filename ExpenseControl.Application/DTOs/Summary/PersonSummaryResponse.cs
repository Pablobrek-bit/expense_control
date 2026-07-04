namespace ExpenseControl.Application.DTOs;

/// <summary>
/// DTO de saída com o resumo financeiro de uma pessoa individual.
/// </summary>
public class PersonSummaryResponse
{
    public Guid PersonId { get; set; }
    public string PersonName { get; set; } = string.Empty;
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal Balance { get; set; }
}
