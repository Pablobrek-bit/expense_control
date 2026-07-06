using ExpenseControl.Domain.Entities;

namespace ExpenseControl.Domain.Interfaces;

/// <summary>
/// Port (interface) para operações de persistência de Transação.
/// A implementação concreta fica na camada de Infrastructure.
/// </summary>
public interface ITransactionRepository
{
    /// <summary>
    /// Retorna todas as transações cadastradas.
    /// </summary>
    Task<IEnumerable<Transaction>> GetAllAsync();

    /// <summary>
    /// Retorna todas as transações de uma pessoa específica.
    /// </summary>
    Task<IEnumerable<Transaction>> GetByPersonIdAsync(Guid personId);

    /// <summary>
    /// Retorna uma transação pelo seu identificador único.
    /// </summary>
    Task<Transaction?> GetByIdAsync(Guid id);

    /// <summary>
    /// Adiciona uma nova transação ao repositório.
    /// </summary>
    Task AddAsync(Transaction transaction);

    /// <summary>
    /// Atualiza uma transação existente.
    /// </summary>
    Task UpdateAsync(Transaction transaction);

    /// <summary>
    /// Remove uma transação pelo seu identificador.
    /// </summary>
    Task DeleteAsync(Transaction transaction);

    /// <summary>
    /// Remove todas as transações de uma pessoa específica.
    /// </summary>
    Task DeleteByPersonIdAsync(Guid personId);
}
