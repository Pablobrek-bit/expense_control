using ExpenseControl.Application.DTOs;
using ExpenseControl.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseControl.Infrastructure.Web.Controllers;

/// <summary>
/// Controller para gerenciamento de Pessoas.
/// Driving Adapter — recebe requisições HTTP e delega para o caso de uso.
/// </summary>
[ApiController]
[Route("api/persons")]
public class PersonController : ControllerBase
{
    private readonly IPersonService _personService;

    public PersonController(IPersonService personService)
    {
        _personService = personService;
    }

    /// <summary>
    /// Lista todas as pessoas cadastradas.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PersonResponse>>> GetAll()
    {
        var persons = await _personService.GetAllAsync();
        return Ok(persons);
    }

    /// <summary>
    /// Cria uma nova pessoa.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<PersonResponse>> Create([FromBody] CreatePersonRequest request)
    {
        var person = await _personService.CreateAsync(request);
        return CreatedAtAction(nameof(GetAll), new { id = person.Id }, person);
    }

    /// <summary>
    /// Deleta uma pessoa e todas as suas transações associadas.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _personService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
