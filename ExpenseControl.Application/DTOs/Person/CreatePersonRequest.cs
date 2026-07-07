namespace ExpenseControl.Application.DTOs;

/// <summary>
/// DTO de entrada para criação de uma pessoa.
/// </summary>
public class CreatePersonRequest
{
    /// <summary>
    /// Nome da pessoa.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Idade da pessoa.
    /// </summary>
    public int Age { get; set; }
}
