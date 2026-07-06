using ExpenseControl.Domain.Entities;

namespace ExpenseControl.Domain.Interfaces;

/// <summary>
/// Port (interface) para operações de persistência de Pessoa.
/// A implementação concreta fica na camada de Infrastructure.
/// </summary>
public interface IPersonRepository
{
    /// <summary>
    /// Retorna uma página de pessoas cadastradas, com suporte a filtros.
    /// Retorna uma tupla contendo a lista de itens e o total de registros que satisfazem o filtro.
    /// </summary>
    Task<(IEnumerable<Person> Items, int TotalCount)> GetAllAsync(int page, int pageSize, string? name = null, int? age = null);

    /// <summary>
    /// Retorna todas as pessoas cadastradas (sem paginação).
    /// </summary>
    Task<IEnumerable<Person>> GetAllAsync();

    /// <summary>
    /// Retorna uma pessoa pelo seu identificador único.
    /// </summary>
    Task<Person?> GetByIdAsync(Guid id);

    /// <summary>
    /// Retorna uma pessoa pelo seu identificador único, incluindo todas as suas transações.
    /// </summary>
    Task<Person?> GetByIdWithTransactionsAsync(Guid id);

    /// <summary>
    /// Adiciona uma nova pessoa ao repositório.
    /// </summary>
    Task AddAsync(Person person);

    /// <summary>
    /// Atualiza os dados de uma pessoa existente.
    /// </summary>
    Task UpdateAsync(Person person);

    /// <summary>
    /// Remove uma pessoa do repositório.
    /// </summary>
    Task DeleteAsync(Person person);

    /// <summary>
    /// Verifica se uma pessoa com o identificador informado existe.
    /// </summary>
    Task<bool> ExistsAsync(Guid id);
}
