using ExpenseControl.Application.DTOs;
using ExpenseControl.Application.DTOs.Transaction;
using ExpenseControl.Application.Services;
using ExpenseControl.Domain.Entities;
using ExpenseControl.Domain.Enums;
using ExpenseControl.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ExpenseControl.Tests.Services;

public class TransactionServiceTests
{
    private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
    private readonly Mock<IPersonRepository> _personRepositoryMock;
    private readonly TransactionService _transactionService;

    public TransactionServiceTests()
    {
        _transactionRepositoryMock = new Mock<ITransactionRepository>();
        _personRepositoryMock = new Mock<IPersonRepository>();
        _transactionService = new TransactionService(_transactionRepositoryMock.Object, _personRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnPagedResult_WhenCalled()
    {
        // Arrange
        var filter = new TransactionFilter { Page = 1, PageSize = 10 };
        var person = new Person { Id = Guid.NewGuid(), Name = "João", Age = 30 };
        var transactions = new List<Transaction>
        {
            new Transaction { Id = Guid.NewGuid(), Description = "Salário", Value = 5000, Type = TransactionType.Income, PersonId = person.Id, Person = person },
            new Transaction { Id = Guid.NewGuid(), Description = "Aluguel", Value = 1500, Type = TransactionType.Expense, PersonId = person.Id, Person = person }
        };

        _transactionRepositoryMock
            .Setup(r => r.GetAllAsync(filter.Page, filter.PageSize, filter.Type, filter.PersonId))
            .ReturnsAsync((transactions, 2));

        // Act
        var result = await _transactionService.GetAllAsync(filter);

        // Assert
        result.Should().NotBeNull();
        result.TotalCount.Should().Be(2);
        result.Items.Should().HaveCount(2);
        result.Items.First().Description.Should().Be("Salário");
        result.Items.First().PersonName.Should().Be("João");
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowKeyNotFoundException_WhenPersonDoesNotExist()
    {
        // Arrange
        var request = new CreateTransactionRequest { PersonId = Guid.NewGuid() };
        
        _personRepositoryMock
            .Setup(r => r.GetByIdAsync(request.PersonId))
            .ReturnsAsync((Person?)null);

        // Act
        Func<Task> act = async () => await _transactionService.CreateAsync(request);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Pessoa com ID '{request.PersonId}' não encontrada.");
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowInvalidOperationException_WhenPersonIsMinorAndTypeIsIncome()
    {
        // Arrange
        var request = new CreateTransactionRequest { PersonId = Guid.NewGuid(), Type = TransactionType.Income };
        var minorPerson = new Person { Id = request.PersonId, Name = "Enzo", Age = 15 }; // Menor de idade
        
        _personRepositoryMock
            .Setup(r => r.GetByIdAsync(request.PersonId))
            .ReturnsAsync(minorPerson);

        // Act
        Func<Task> act = async () => await _transactionService.CreateAsync(request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Pessoas menores de 18 anos só podem ter transações do tipo Despesa.");
    }

    [Fact]
    public async Task CreateAsync_ShouldSaveTransaction_WhenValid()
    {
        // Arrange
        var request = new CreateTransactionRequest { PersonId = Guid.NewGuid(), Type = TransactionType.Income, Description = "Bônus", Value = 1000 };
        var adultPerson = new Person { Id = request.PersonId, Name = "Carlos", Age = 35 }; // Maior de idade
        
        _personRepositoryMock
            .Setup(r => r.GetByIdAsync(request.PersonId))
            .ReturnsAsync(adultPerson);

        _transactionRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Transaction>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _transactionService.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Description.Should().Be("Bônus");
        result.PersonName.Should().Be("Carlos");

        _transactionRepositoryMock.Verify(r => r.AddAsync(It.Is<Transaction>(t => t.Description == "Bônus" && t.Value == 1000)), Times.Once);
    }

    [Fact]
    public async Task GetSummaryAsync_ShouldCalculateCorrectly_WhenCalled()
    {
        // Arrange
        var person1 = new Person { Id = Guid.NewGuid(), Name = "Alice", Age = 30 };
        var person2 = new Person { Id = Guid.NewGuid(), Name = "Bob", Age = 25 };
        
        _personRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Person> { person1, person2 });

        var aliceTransactions = new List<Transaction>
        {
            new Transaction { Type = TransactionType.Income, Value = 3000 },
            new Transaction { Type = TransactionType.Expense, Value = 1000 }
        };
        
        var bobTransactions = new List<Transaction>
        {
            new Transaction { Type = TransactionType.Expense, Value = 500 }
        };

        _transactionRepositoryMock.Setup(r => r.GetByPersonIdAsync(person1.Id)).ReturnsAsync(aliceTransactions);
        _transactionRepositoryMock.Setup(r => r.GetByPersonIdAsync(person2.Id)).ReturnsAsync(bobTransactions);

        // Act
        var result = await _transactionService.GetSummaryAsync();

        // Assert
        result.Should().NotBeNull();
        result.TotalIncome.Should().Be(3000); // 3000 (Alice) + 0 (Bob)
        result.TotalExpenses.Should().Be(1500); // 1000 (Alice) + 500 (Bob)
        result.NetBalance.Should().Be(1500); // 3000 - 1500

        result.PersonSummaries.Should().HaveCount(2);
        
        var aliceSummary = result.PersonSummaries.First(p => p.PersonId == person1.Id);
        aliceSummary.TotalIncome.Should().Be(3000);
        aliceSummary.TotalExpenses.Should().Be(1000);
        aliceSummary.Balance.Should().Be(2000);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowKeyNotFoundException_WhenTransactionDoesNotExist()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var request = new UpdateTransactionRequest();
        
        _transactionRepositoryMock
            .Setup(r => r.GetByIdAsync(transactionId))
            .ReturnsAsync((Transaction?)null);

        // Act
        Func<Task> act = async () => await _transactionService.UpdateAsync(transactionId, request);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowInvalidOperationException_WhenPersonIsMinorAndTypeIsIncome()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var personId = Guid.NewGuid();
        
        var existingTransaction = new Transaction { Id = transactionId, PersonId = personId, Type = TransactionType.Expense };
        var updateRequest = new UpdateTransactionRequest { Type = TransactionType.Income }; // Tentando atualizar para Receita
        var minorPerson = new Person { Id = personId, Age = 17 }; // Menor de idade

        _transactionRepositoryMock.Setup(r => r.GetByIdAsync(transactionId)).ReturnsAsync(existingTransaction);
        _personRepositoryMock.Setup(r => r.GetByIdAsync(personId)).ReturnsAsync(minorPerson);

        // Act
        Func<Task> act = async () => await _transactionService.UpdateAsync(transactionId, updateRequest);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateTransaction_WhenValid()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var personId = Guid.NewGuid();
        
        var existingTransaction = new Transaction { Id = transactionId, PersonId = personId, Type = TransactionType.Expense, Description = "Antigo", Value = 100 };
        var updateRequest = new UpdateTransactionRequest { Type = TransactionType.Expense, Description = "Novo", Value = 150 }; 
        var adultPerson = new Person { Id = personId, Name = "Jorge", Age = 40 };

        _transactionRepositoryMock.Setup(r => r.GetByIdAsync(transactionId)).ReturnsAsync(existingTransaction);
        _personRepositoryMock.Setup(r => r.GetByIdAsync(personId)).ReturnsAsync(adultPerson);
        _transactionRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Transaction>())).Returns(Task.CompletedTask);

        // Act
        var result = await _transactionService.UpdateAsync(transactionId, updateRequest);

        // Assert
        result.Should().NotBeNull();
        result.Description.Should().Be("Novo");
        result.Value.Should().Be(150);

        _transactionRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Transaction>(t => t.Description == "Novo" && t.Value == 150)), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteTransaction_WhenTransactionExists()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var existingTransaction = new Transaction { Id = transactionId };

        _transactionRepositoryMock.Setup(r => r.GetByIdAsync(transactionId)).ReturnsAsync(existingTransaction);
        _transactionRepositoryMock.Setup(r => r.DeleteAsync(existingTransaction)).Returns(Task.CompletedTask);

        // Act
        await _transactionService.DeleteAsync(transactionId);

        // Assert
        _transactionRepositoryMock.Verify(r => r.DeleteAsync(existingTransaction), Times.Once);
    }
}
