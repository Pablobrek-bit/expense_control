using System.ComponentModel.DataAnnotations;

namespace ExpenseControl.Application.DTOs;

/// <summary>
/// DTO de entrada para criação de uma pessoa.
/// </summary>
public class CreatePersonRequest
{
    /// <summary>
    /// Nome da pessoa.
    /// </summary>
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [MinLength(2, ErrorMessage = "O nome deve ter pelo menos 2 caracteres.")]
    [MaxLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Idade da pessoa.
    /// </summary>
    [Required(ErrorMessage = "A idade é obrigatória.")]
    [Range(1, 150, ErrorMessage = "A idade deve ser entre 1 e 150.")]
    public int Age { get; set; }
}

