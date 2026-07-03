using ExpenseControl.Domain.Entities;

namespace ExpenseControl.Domain.Interfaces;

/// <summary>
/// Port (interface) para operações de persistência de Pessoa.
/// A implementação concreta fica na camada de Infrastructure.
/// </summary>
public interface IPersonRepository
{
    /// <summary>
    /// Retorna todas as pessoas cadastradas.
    /// </summary>
    Task<IEnumerable<Person>> GetAllAsync();

    /// <summary>
    /// Busca uma pessoa pelo seu identificador.
    /// </summary>
    Task<Person?> GetByIdAsync(Guid id);

    /// <summary>
    /// Adiciona uma nova pessoa ao repositório.
    /// </summary>
    Task AddAsync(Person person);

    /// <summary>
    /// Remove uma pessoa do repositório.
    /// </summary>
    Task DeleteAsync(Person person);

    /// <summary>
    /// Verifica se uma pessoa com o identificador informado existe.
    /// </summary>
    Task<bool> ExistsAsync(Guid id);
}
