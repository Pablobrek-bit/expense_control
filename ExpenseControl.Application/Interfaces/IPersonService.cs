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
    /// Retorna os detalhes de uma pessoa pelo seu ID, incluindo suas transações.
    /// </summary>
    Task<PersonDetailsResponse> GetDetailsAsync(Guid id);

    /// <summary>
    /// Cria uma nova pessoa no sistema.
    /// </summary>
    Task<PersonResponse> CreateAsync(CreatePersonRequest request);

    /// <summary>
    /// Deleta uma pessoa e todas as suas transações.
    /// </summary>
    Task DeleteAsync(Guid id);
}
