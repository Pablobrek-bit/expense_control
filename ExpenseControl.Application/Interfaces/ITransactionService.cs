using ExpenseControl.Application.DTOs;

namespace ExpenseControl.Application.Interfaces;

/// <summary>
/// Interface do caso de uso de Transação.
/// Define as operações disponíveis para o domínio de Transação.
/// </summary>
public interface ITransactionService
{
    /// <summary>
    /// Retorna todas as transações cadastradas.
    /// </summary>
    Task<IEnumerable<TransactionResponse>> GetAllAsync();

    /// <summary>
    /// Cria uma nova transação.
    /// Valida se a pessoa existe e aplica regra de menor de idade.
    /// </summary>
    Task<TransactionResponse> CreateAsync(CreateTransactionRequest request);

    /// <summary>
    /// Retorna o resumo financeiro de todas as pessoas com totais gerais.
    /// </summary>
    Task<SummaryResponse> GetSummaryAsync();
}
