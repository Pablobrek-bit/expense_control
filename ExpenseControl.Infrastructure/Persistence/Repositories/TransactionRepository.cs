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
    public async Task<(IEnumerable<Transaction> Items, int TotalCount)> GetAllAsync(int page, int pageSize, ExpenseControl.Domain.Enums.TransactionType? type = null, Guid? personId = null)
    {
        var query = _context.Transactions.AsNoTracking().Include(t => t.Person).AsQueryable();

        if (type.HasValue)
        {
            query = query.Where(t => t.Type == type.Value);
        }

        if (personId.HasValue)
        {
            query = query.Where(t => t.PersonId == personId.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(t => t.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
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
    public async Task<Transaction?> GetByIdAsync(Guid id)
    {
        return await _context.Transactions.FindAsync(id);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Transaction transaction)
    {
        _context.Transactions.Update(transaction);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Transaction transaction)
    {
        _context.Transactions.Remove(transaction);
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
