using ExpenseControl.Application.DTOs;
using ExpenseControl.Application.DTOs.Person;
using ExpenseControl.Application.Services;
using ExpenseControl.Domain.Entities;
using ExpenseControl.Domain.Enums;
using ExpenseControl.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ExpenseControl.Tests.Services;

public class PersonServiceTests
{
    private readonly Mock<IPersonRepository> _personRepositoryMock;
    private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
    private readonly PersonService _personService;

    public PersonServiceTests()
    {
        _personRepositoryMock = new Mock<IPersonRepository>();
        _transactionRepositoryMock = new Mock<ITransactionRepository>();
        _personService = new PersonService(_personRepositoryMock.Object, _transactionRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnPagedResult_WhenCalled()
    {
        // Arrange
        var filter = new PersonFilter { Page = 1, PageSize = 10 };
        var persons = new List<Person>
        {
            new Person { Id = Guid.NewGuid(), Name = "João", Age = 30 },
            new Person { Id = Guid.NewGuid(), Name = "Maria", Age = 25 }
        };

        _personRepositoryMock
            .Setup(r => r.GetAllAsync(filter.Page, filter.PageSize, filter.Name, filter.Age))
            .ReturnsAsync((persons, 2));

        // Act
        var result = await _personService.GetAllAsync(filter);

        // Assert
        result.Should().NotBeNull();
        result.TotalCount.Should().Be(2);
        result.Items.Should().HaveCount(2);
        result.Items.First().Name.Should().Be("João");
    }

    [Fact]
    public async Task GetDetailsAsync_ShouldReturnDetails_WhenPersonExists()
    {
        // Arrange
        var personId = Guid.NewGuid();
        var person = new Person
        {
            Id = personId,
            Name = "João",
            Age = 30,
            Transactions = new List<Transaction>
            {
                new Transaction { Id = Guid.NewGuid(), Description = "Salário", Value = 5000, Type = TransactionType.Income }
            }
        };

        _personRepositoryMock
            .Setup(r => r.GetByIdWithTransactionsAsync(personId))
            .ReturnsAsync(person);

        // Act
        var result = await _personService.GetDetailsAsync(personId);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("João");
        result.Transactions.Should().HaveCount(1);
        result.Transactions.First().Description.Should().Be("Salário");
    }

    [Fact]
    public async Task GetDetailsAsync_ShouldThrowKeyNotFoundException_WhenPersonDoesNotExist()
    {
        // Arrange
        var personId = Guid.NewGuid();
        _personRepositoryMock
            .Setup(r => r.GetByIdWithTransactionsAsync(personId))
            .ReturnsAsync((Person?)null);

        // Act
        Func<Task> act = async () => await _personService.GetDetailsAsync(personId);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Pessoa com ID '{personId}' não encontrada.");
    }

    [Fact]
    public async Task CreateAsync_ShouldSavePersonAndReturnResponse()
    {
        // Arrange
        var request = new CreatePersonRequest { Name = "Carlos", Age = 40 };

        _personRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Person>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _personService.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Carlos");
        result.Age.Should().Be(40);
        result.Id.Should().NotBeEmpty();

        _personRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Person>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateAndReturnResponse_WhenPersonExists()
    {
        // Arrange
        var personId = Guid.NewGuid();
        var existingPerson = new Person { Id = personId, Name = "Ana", Age = 28 };
        var updateRequest = new UpdatePersonRequest { Name = "Ana Silva", Age = 29 };

        _personRepositoryMock
            .Setup(r => r.GetByIdAsync(personId))
            .ReturnsAsync(existingPerson);

        _personRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Person>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _personService.UpdateAsync(personId, updateRequest);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Ana Silva");
        result.Age.Should().Be(29);

        _personRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Person>(p => p.Name == "Ana Silva" && p.Age == 29)), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowKeyNotFoundException_WhenPersonDoesNotExist()
    {
        // Arrange
        var personId = Guid.NewGuid();
        var updateRequest = new UpdatePersonRequest { Name = "Ana Silva", Age = 29 };

        _personRepositoryMock
            .Setup(r => r.GetByIdAsync(personId))
            .ReturnsAsync((Person?)null);

        // Act
        Func<Task> act = async () => await _personService.UpdateAsync(personId, updateRequest);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeletePersonAndTransactions_WhenPersonExists()
    {
        // Arrange
        var personId = Guid.NewGuid();
        var existingPerson = new Person { Id = personId, Name = "Pedro", Age = 35 };

        _personRepositoryMock
            .Setup(r => r.GetByIdAsync(personId))
            .ReturnsAsync(existingPerson);

        _transactionRepositoryMock
            .Setup(t => t.DeleteByPersonIdAsync(personId))
            .Returns(Task.CompletedTask);

        _personRepositoryMock
            .Setup(r => r.DeleteAsync(existingPerson))
            .Returns(Task.CompletedTask);

        // Act
        await _personService.DeleteAsync(personId);

        // Assert
        _transactionRepositoryMock.Verify(t => t.DeleteByPersonIdAsync(personId), Times.Once);
        _personRepositoryMock.Verify(r => r.DeleteAsync(existingPerson), Times.Once);
    }
}
