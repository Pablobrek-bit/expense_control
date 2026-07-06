using ExpenseControl.Domain.Entities;

namespace ExpenseControl.Domain.Interfaces;

/// <summary>
/// Port (interface) para operações de persistência de Transação.
/// A implementação concreta fica na camada de Infrastructure.
/// </summary>
public interface ITransactionRepository
{
    /// <summary>
    /// Retorna uma página de transações cadastradas, com suporte a filtros.
    /// Retorna uma tupla contendo a lista de itens e o total de registros que satisfazem o filtro.
    /// </summary>
    Task<(IEnumerable<Transaction> Items, int TotalCount)> GetAllAsync(int page, int pageSize, Domain.Enums.TransactionType? type = null, Guid? personId = null);

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
