using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApi.Application.DTOs;
using RealEstateApi.Application.Interfaces;

namespace RealEstateApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _service;

    public TransactionsController(ITransactionService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<TransactionDto>>> GetAll([FromQuery] int? clientId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var (items, total) = await _service.GetAllAsync(clientId, page, pageSize);
        return Ok(new { Items = items, TotalCount = total });
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<TransactionDto>> GetById(int id)
    {
        var t = await _service.GetByIdAsync(id);
        if (t == null) return NotFound();
        return Ok(t);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<TransactionDto>> Create(CreateTransactionDto dto)
    {
        var t = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = t.Id }, t);
    }

    [HttpPost("{id}/payments")]
    [Authorize]
    public async Task<IActionResult> AddPayment(int id, CreatePaymentDto dto)
    {
        await _service.AddPaymentAsync(id, dto);
        return NoContent();
    }
    
    [HttpGet("{id}/balance")]
    [Authorize]
    public async Task<ActionResult<decimal>> GetBalance(int id)
    {
        var balance = await _service.GetRemainingBalanceAsync(id);
        return Ok(new { RemainingBalance = balance });
    }
}
