using ExpenseControl.Application.DTOs;
using ExpenseControl.Application.DTOs.Common;
using ExpenseControl.Application.DTOs.Person;
using ExpenseControl.Application.Interfaces;
using ExpenseControl.Domain.Entities;
using ExpenseControl.Domain.Interfaces;

namespace ExpenseControl.Application.Services;

/// <summary>
/// Implementação do caso de uso de Pessoa.
/// Orquestra as operações usando os repositórios do Domain.
/// </summary>
public class PersonService : IPersonService
{
    private readonly IPersonRepository _personRepository;
    private readonly ITransactionRepository _transactionRepository;

    public PersonService(
        IPersonRepository personRepository,
        ITransactionRepository transactionRepository)
    {
        _personRepository = personRepository;
        _transactionRepository = transactionRepository;
    }

    /// <inheritdoc />
    public async Task<PagedResult<PersonResponse>> GetAllAsync(PersonFilter filter)
    {
        var (items, totalCount) = await _personRepository.GetAllAsync(filter.Page, filter.PageSize, filter.Name, filter.Age);

        var responses = items.Select(p => new PersonResponse
        {
            Id = p.Id,
            Name = p.Name,
            Age = p.Age
        });

        return new PagedResult<PersonResponse>(responses, totalCount, filter.Page, filter.PageSize);
    }

    /// <inheritdoc />
    public async Task<PersonDetailsResponse> GetDetailsAsync(Guid id)
    {
        var person = await _personRepository.GetByIdWithTransactionsAsync(id)
            ?? throw new KeyNotFoundException($"Pessoa com ID '{id}' não encontrada.");

        return new PersonDetailsResponse
        {
            Id = person.Id,
            Name = person.Name,
            Age = person.Age,
            Transactions = person.Transactions.Select(t => new PersonTransactionResponse
            {
                Id = t.Id,
                Description = t.Description,
                Value = t.Value,
                Type = t.Type
            }).ToList()
        };
    }

    /// <inheritdoc />
    public async Task<PersonResponse> CreateAsync(CreatePersonRequest request)
    {
        var person = new Person
        {
            Name = request.Name,
            Age = request.Age
        };

        await _personRepository.AddAsync(person);

        return new PersonResponse
        {
            Id = person.Id,
            Name = person.Name,
            Age = person.Age
        };
    }

    /// <inheritdoc />
    public async Task<PersonResponse> UpdateAsync(Guid id, UpdatePersonRequest request)
    {
        var person = await _personRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Pessoa com ID '{id}' não encontrada.");

        if (request.Age < 18 && person.Age >= 18)
        {
            var transactions = await _transactionRepository.GetByPersonIdAsync(id);
            if (transactions.Any(t => t.Type == Domain.Enums.TransactionType.Income))
            {
                throw new ArgumentException("Não é permitido alterar a idade para menor de 18 anos, pois esta pessoa já possui transações de Receita (Income). Remova as receitas primeiro.");
            }
        }

        person.Name = request.Name;
        person.Age = request.Age;

        await _personRepository.UpdateAsync(person);

        return new PersonResponse
        {
            Id = person.Id,
            Name = person.Name,
            Age = person.Age
        };
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id)
    {
        var person = await _personRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Pessoa com ID '{id}' não encontrada.");

        await _transactionRepository.DeleteByPersonIdAsync(id);
        await _personRepository.DeleteAsync(person);
    }
}
