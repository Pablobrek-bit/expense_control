using ExpenseControl.Domain.Entities;
using ExpenseControl.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseControl.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementação concreta do repositório de Pessoa usando Entity Framework Core.
/// Este é o Driven Adapter que conecta o Domain ao banco de dados.
/// </summary>
public class PersonRepository : IPersonRepository
{
    private readonly AppDbContext _context;

    public PersonRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<(IEnumerable<Person> Items, int TotalCount)> GetAllAsync(int page, int pageSize, string? name = null, int? age = null)
    {
        var query = _context.Persons.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(p => p.Name.Contains(name));
        }

        if (age.HasValue)
        {
            query = query.Where(p => p.Age == age.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Person>> GetAllAsync()
    {
        return await _context.Persons
            .AsNoTracking()
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Person?> GetByIdAsync(Guid id)
    {
        return await _context.Persons.FindAsync(id);
    }

    /// <inheritdoc />
    public async Task<Person?> GetByIdWithTransactionsAsync(Guid id)
    {
        return await _context.Persons
            .Include(p => p.Transactions)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    /// <inheritdoc />
    public async Task AddAsync(Person person)
    {
        await _context.Persons.AddAsync(person);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Person person)
    {
        _context.Persons.Update(person);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Person person)
    {
        _context.Persons.Remove(person);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Persons.AnyAsync(p => p.Id == id);
    }
}
