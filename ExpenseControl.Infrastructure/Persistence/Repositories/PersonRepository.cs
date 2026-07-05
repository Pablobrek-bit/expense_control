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
