namespace ExpenseControl.Domain.Entities;

/// <summary>
/// Representa uma pessoa cadastrada no sistema de controle de gastos.
/// </summary>
public class Person
{
    /// <summary>
    /// Identificador único da pessoa (gerado automaticamente).
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Nome da pessoa.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Idade da pessoa.
    /// </summary>
    public int Age { get; set; }

    /// <summary>
    /// Transações associadas a esta pessoa.
    /// </summary>
    public ICollection<Transaction> Transactions { get; set; } = [];

    /// <summary>
    /// Verifica se a pessoa é menor de idade (menos de 18 anos).
    /// </summary>
    public bool IsMinor() => Age < 18;
}
