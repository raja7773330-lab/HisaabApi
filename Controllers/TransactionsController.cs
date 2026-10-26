
using HisaabApi.Data;
using HisaabApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HisaabApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly HisaabDbContext _context;

    public TransactionsController(HisaabDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetTransactions()
    {
        var transactions = await _context.Transactions
            .Include(t => t.Category)
            .OrderByDescending(t => t.Date)
            .ToListAsync();

        return Ok(transactions);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetTransaction(int id)
    {
        var transaction = await _context.Transactions
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (transaction == null)
            return NotFound(new { message = "Transaction not found." });

        return Ok(transaction);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransaction(
        [FromBody] CreateTransactionRequest request)
    {
        if (request.Amount <= 0)
            return BadRequest(new
            {
                message = "Amount must be greater than zero."
            });

        if (request.Type != "Income" &&
            request.Type != "Expense")
        {
            return BadRequest(new
            {
                message = "Type must be Income or Expense."
            });
        }

        var category = await _context.Categories
            .FirstOrDefaultAsync(c =>
                c.Id == request.CategoryId &&
                c.IsActive &&
                c.Type == request.Type);

        if (category == null)
            return BadRequest(new
            {
                message = "Active category not found for this transaction type."
            });

        var transaction = new Transaction
        {
            Type = request.Type,
            Amount = request.Amount,
            CategoryId = request.CategoryId,
            Date = request.Date,
            Note = request.Note,
            CreatedAt = DateTime.Now
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        transaction.Category = category;

        return CreatedAtAction(
            nameof(GetTransaction),
            new { id = transaction.Id },
            transaction
        );
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTransaction(int id)
    {
        var transaction = await _context.Transactions
            .FindAsync(id);

        if (transaction == null)
            return NotFound(new
            {
                message = "Transaction not found."
            });

        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
