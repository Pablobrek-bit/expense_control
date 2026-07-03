using ExpenseControl.Domain.Entities;
using ExpenseControl.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseControl.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementação concreta do repositório de Transação usando Entity Framework Core.
/// Este é o Driven Adapter que conecta o Domain ao banco de dados.
/// </summary>
public class TransactionRepository : ITransactionRepository
{
    private readonly AppDbContext _context;

    public TransactionRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Transaction>> GetAllAsync()
    {
        return await _context.Transactions
            .AsNoTracking()
            .Include(t => t.Person)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Transaction>> GetByPersonIdAsync(Guid personId)
    {
        return await _context.Transactions
            .AsNoTracking()
            .Where(t => t.PersonId == personId)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task AddAsync(Transaction transaction)
    {
        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task DeleteByPersonIdAsync(Guid personId)
    {
        var transactions = await _context.Transactions
            .Where(t => t.PersonId == personId)
            .ToListAsync();

        _context.Transactions.RemoveRange(transactions);
        await _context.SaveChangesAsync();
    }
}
