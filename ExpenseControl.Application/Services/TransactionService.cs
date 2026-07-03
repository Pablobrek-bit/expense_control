using ExpenseControl.Application.DTOs;
using ExpenseControl.Application.Interfaces;
using ExpenseControl.Domain.Entities;
using ExpenseControl.Domain.Enums;
using ExpenseControl.Domain.Interfaces;

namespace ExpenseControl.Application.Services;

/// <summary>
/// Implementação do caso de uso de Transação.
/// Contém as regras de negócio de criação e consulta de totais.
/// </summary>
public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IPersonRepository _personRepository;

    public TransactionService(
        ITransactionRepository transactionRepository,
        IPersonRepository personRepository)
    {
        _transactionRepository = transactionRepository;
        _personRepository = personRepository;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TransactionResponse>> GetAllAsync()
    {
        var transactions = await _transactionRepository.GetAllAsync();

        return transactions.Select(t => new TransactionResponse
        {
            Id = t.Id,
            Description = t.Description,
            Value = t.Value,
            Type = t.Type,
            PersonId = t.PersonId,
            PersonName = t.Person.Name
        });
    }

    /// <inheritdoc />
    public async Task<TransactionResponse> CreateAsync(CreateTransactionRequest request)
    {
        // Valida se a pessoa existe.
        var person = await _personRepository.GetByIdAsync(request.PersonId)
            ?? throw new KeyNotFoundException($"Pessoa com ID '{request.PersonId}' não encontrada.");

        // Regra de negócio: se a pessoa é menor de idade, apenas despesas podem ser cadastradas.
        if (person.IsMinor() && request.Type != TransactionType.Expense)
        {
            throw new InvalidOperationException(
                "Pessoas menores de 18 anos só podem ter transações do tipo Despesa.");
        }

        var transaction = new Transaction
        {
            Description = request.Description,
            Value = request.Value,
            Type = request.Type,
            PersonId = request.PersonId
        };

        await _transactionRepository.AddAsync(transaction);

        return new TransactionResponse
        {
            Id = transaction.Id,
            Description = transaction.Description,
            Value = transaction.Value,
            Type = transaction.Type,
            PersonId = transaction.PersonId,
            PersonName = person.Name
        };
    }

    /// <inheritdoc />
    public async Task<SummaryResponse> GetSummaryAsync()
    {
        var persons = await _personRepository.GetAllAsync();
        var personSummaries = new List<PersonSummaryResponse>();

        foreach (var person in persons)
        {
            var transactions = await _transactionRepository.GetByPersonIdAsync(person.Id);

            var totalIncome = transactions
                .Where(t => t.Type == TransactionType.Income)
                .Sum(t => t.Value);

            var totalExpenses = transactions
                .Where(t => t.Type == TransactionType.Expense)
                .Sum(t => t.Value);

            personSummaries.Add(new PersonSummaryResponse
            {
                PersonId = person.Id,
                PersonName = person.Name,
                TotalIncome = totalIncome,
                TotalExpenses = totalExpenses,
                Balance = totalIncome - totalExpenses
            });
        }

        return new SummaryResponse
        {
            PersonSummaries = personSummaries,
            TotalIncome = personSummaries.Sum(p => p.TotalIncome),
            TotalExpenses = personSummaries.Sum(p => p.TotalExpenses),
            NetBalance = personSummaries.Sum(p => p.Balance)
        };
    }
}
