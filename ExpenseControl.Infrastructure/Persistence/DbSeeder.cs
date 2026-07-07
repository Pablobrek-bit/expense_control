using ExpenseControl.Domain.Entities;
using ExpenseControl.Domain.Enums;

namespace ExpenseControl.Infrastructure.Persistence;

public static class DbSeeder
{
    public static void Seed(AppDbContext context)
    {
        if (context.Persons.Any())
        {
            return;
        }

        var roberto = new Person { Name = "Roberto", Age = 45 };
        var ana = new Person { Name = "Ana", Age = 42 };
        var julia = new Person { Name = "Julia", Age = 20 };
        var lucas = new Person { Name = "Lucas", Age = 15 };

        context.Persons.AddRange(roberto, ana, julia, lucas);
        context.SaveChanges();
        var transactions = new List<Transaction>
        {
            new Transaction { Description = "Salário", Value = 8500.00m, Type = TransactionType.Income, PersonId = roberto.Id },
            new Transaction { Description = "Conta de Luz", Value = 350.00m, Type = TransactionType.Expense, PersonId = roberto.Id },
            new Transaction { Description = "Conta de Água", Value = 120.00m, Type = TransactionType.Expense, PersonId = roberto.Id },
            new Transaction { Description = "Financiamento Carro", Value = 1200.00m, Type = TransactionType.Expense, PersonId = roberto.Id },
            new Transaction { Description = "Gasolina", Value = 250.00m, Type = TransactionType.Expense, PersonId = roberto.Id },

            new Transaction { Description = "Salário", Value = 7200.00m, Type = TransactionType.Income, PersonId = ana.Id },
            new Transaction { Description = "Compra do Mês (Mercado)", Value = 1800.00m, Type = TransactionType.Expense, PersonId = ana.Id },
            new Transaction { Description = "Internet", Value = 150.00m, Type = TransactionType.Expense, PersonId = ana.Id },
            new Transaction { Description = "Netflix e Spotify", Value = 85.00m, Type = TransactionType.Expense, PersonId = ana.Id },
            new Transaction { Description = "Roupas", Value = 450.00m, Type = TransactionType.Expense, PersonId = ana.Id },

            new Transaction { Description = "Bolsa Estágio", Value = 1200.00m, Type = TransactionType.Income, PersonId = julia.Id },
            new Transaction { Description = "Mensalidade Faculdade", Value = 800.00m, Type = TransactionType.Expense, PersonId = julia.Id },
            new Transaction { Description = "Lanche Faculdade", Value = 150.00m, Type = TransactionType.Expense, PersonId = julia.Id },
            new Transaction { Description = "Cinema", Value = 60.00m, Type = TransactionType.Expense, PersonId = julia.Id },

            new Transaction { Description = "Lanche Escola", Value = 80.00m, Type = TransactionType.Expense, PersonId = lucas.Id },
            new Transaction { Description = "Jogo de Videogame", Value = 300.00m, Type = TransactionType.Expense, PersonId = lucas.Id },
            new Transaction { Description = "Figurinhas", Value = 45.00m, Type = TransactionType.Expense, PersonId = lucas.Id },
            new Transaction { Description = "Passeio do Colégio", Value = 120.00m, Type = TransactionType.Expense, PersonId = lucas.Id }
        };

        context.Transactions.AddRange(transactions);
        context.SaveChanges();
    }
}
