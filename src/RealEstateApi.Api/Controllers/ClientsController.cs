using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApi.Application.DTOs;
using RealEstateApi.Application.Interfaces;

namespace RealEstateApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _service;
    private readonly ITransactionService _transactionService;

    public ClientsController(IClientService service, ITransactionService transactionService)
    {
        _service = service;
        _transactionService = transactionService;
    }

    [HttpGet]
    [Authorize] 
    public async Task<ActionResult<IEnumerable<ClientDto>>> GetAll([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var (items, total) = await _service.GetAllAsync(search, page, pageSize);
        return Ok(new { Items = items, TotalCount = total });
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<ClientDto>> GetById(int id)
    {
        var client = await _service.GetByIdAsync(id);
        if (client == null) return NotFound();
        return Ok(client);
    }

    [HttpGet("{id}/transactions")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactions(int id)
    {
        var transactions = await _transactionService.GetByClientIdAsync(id);
        return Ok(transactions);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ClientDto>> Create(CreateClientDto dto)
    {
        var client = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, UpdateClientDto dto)
    {
        await _service.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
