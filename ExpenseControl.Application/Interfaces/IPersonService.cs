using ExpenseControl.Application.DTOs;

namespace ExpenseControl.Application.Interfaces;

/// <summary>
/// Interface do caso de uso de Pessoa.
/// Define as operações disponíveis para o domínio de Pessoa.
/// </summary>
public interface IPersonService
{
    /// <summary>
    /// Retorna todas as pessoas cadastradas.
    /// </summary>
    Task<IEnumerable<PersonResponse>> GetAllAsync();

    /// <summary>
    /// Cria uma nova pessoa.
    /// </summary>
    Task<PersonResponse> CreateAsync(CreatePersonRequest request);

    /// <summary>
    /// Deleta uma pessoa e todas as suas transações.
    /// </summary>
    Task DeleteAsync(Guid id);
}
