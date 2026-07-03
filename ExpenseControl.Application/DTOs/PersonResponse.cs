namespace ExpenseControl.Application.DTOs;

/// <summary>
/// DTO de saída para retorno de dados de uma pessoa.
/// </summary>
public class PersonResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
}
