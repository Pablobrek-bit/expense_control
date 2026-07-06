using ExpenseControl.Application.DTOs;
using ExpenseControl.Application.DTOs.Common;
using ExpenseControl.Application.DTOs.Person;

namespace ExpenseControl.Application.Interfaces;

/// <summary>
/// Interface do caso de uso de Pessoa.
/// Define as operações disponíveis para o domínio de Pessoa.
/// </summary>
public interface IPersonService
{
    /// <summary>
    /// Retorna uma lista paginada de pessoas, com opção de filtros.
    /// </summary>
    Task<PagedResult<PersonResponse>> GetAllAsync(PersonFilter filter);

    /// <summary>
    /// Retorna os detalhes de uma pessoa pelo seu ID, incluindo suas transações.
    /// </summary>
    Task<PersonDetailsResponse> GetDetailsAsync(Guid id);

    /// <summary>
    /// Cria uma nova pessoa no sistema.
    /// </summary>
    Task<PersonResponse> CreateAsync(CreatePersonRequest request);

    /// <summary>
    /// Atualiza os dados de uma pessoa existente no sistema.
    /// </summary>
    Task<PersonResponse> UpdateAsync(Guid id, UpdatePersonRequest request);

    /// <summary>
    /// Deleta uma pessoa e todas as suas transações.
    /// </summary>
    Task DeleteAsync(Guid id);
}
