
using HisaabApi.Data;
using HisaabApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HisaabApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly HisaabDbContext _context;

    public CategoriesController(HisaabDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _context.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();

        return Ok(categories);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory(Category category)
    {
        category.Id = 0;

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return Ok(category);
    }
}
