using ExpenseControl.Application.DTOs;
using ExpenseControl.Application.DTOs.Common;
using ExpenseControl.Application.DTOs.Transaction;

namespace ExpenseControl.Application.Interfaces;

/// <summary>
/// Interface do caso de uso de Transação.
/// Define as operações disponíveis para o domínio de Transação.
/// </summary>
public interface ITransactionService
{
    /// <summary>
    /// Retorna uma lista paginada de transações, com opção de filtros.
    /// </summary>
    Task<PagedResult<TransactionResponse>> GetAllAsync(TransactionFilter filter);

    /// <summary>
    /// Cria uma nova transação.
    /// Valida se a pessoa existe e aplica regra de menor de idade.
    /// </summary>
    Task<TransactionResponse> CreateAsync(CreateTransactionRequest request);

    /// <summary>
    /// Retorna o resumo financeiro de todas as pessoas com totais gerais.
    /// </summary>
    Task<SummaryResponse> GetSummaryAsync();

    /// <summary>
    /// Atualiza uma transação existente.
    /// Valida regra de menor de idade ao trocar o tipo.
    /// </summary>
    Task<TransactionResponse> UpdateAsync(Guid id, UpdateTransactionRequest request);

    /// <summary>
    /// Deleta uma transação pelo seu ID.
    /// </summary>
    Task DeleteAsync(Guid id);
}
