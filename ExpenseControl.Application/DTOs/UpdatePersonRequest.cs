using System.ComponentModel.DataAnnotations;

namespace ExpenseControl.Application.DTOs;

/// <summary>
/// DTO de entrada para atualização dos dados de uma pessoa.
/// </summary>
public class UpdatePersonRequest
{
    /// <summary>
    /// Nome da pessoa.
    /// </summary>
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [MinLength(2, ErrorMessage = "O nome deve ter no mínimo 2 caracteres.")]
    [MaxLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Idade da pessoa.
    /// </summary>
    [Required(ErrorMessage = "A idade é obrigatória.")]
    [Range(1, 150, ErrorMessage = "A idade deve ser entre 1 e 150 anos.")]
    public int Age { get; set; }
}
