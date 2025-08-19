using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HizmetPinari.Api.Data;
using HizmetPinari.Api.Dtos;
using HizmetPinari.Api.Models;
using Microsoft.AspNetCore.Authorization;

namespace HizmetPinari.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CategoryController(ApplicationDbContext context)
    {
        _context = context;
    }

    // --- Herkesin Erişebileceği Alanlar ---

    [HttpGet] // GET /api/category
    public async Task<IActionResult> GetAllCategories()
    {
        var categories = await _context.ServiceCategories
            .Where(c => c.IsActive)
            .OrderBy(c => c.CategoryName)
            .ToListAsync();
        return Ok(categories);
    }

    [HttpGet("{id}")] // GET /api/category/5
    public async Task<IActionResult> GetCategoryById(int id)
    {
        var category = await _context.ServiceCategories.FindAsync(id);
        if (category == null || !category.IsActive)
        {
            return NotFound();
        }
        return Ok(category);
    }

    // --- Sadece Adminlerin Erişebileceği Alanlar ---

    [HttpPost]
    [Authorize(Roles = "Admin")] // Bu endpoint'e sadece "Admin" rolündekiler erişebilir.
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto categoryDto)
    {
        var newCategory = new ServiceCategory
        {
            CategoryName = categoryDto.CategoryName,
            Description = categoryDto.Description,
            ParentCategoryID = categoryDto.ParentCategoryID
        };

        await _context.ServiceCategories.AddAsync(newCategory);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCategoryById), new { id = newCategory.CategoryID }, newCategory);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")] // Bu endpoint'e sadece "Admin" rolündekiler erişebilir.
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto categoryDto)
    {
        var categoryToUpdate = await _context.ServiceCategories.FindAsync(id);
        if (categoryToUpdate == null)
        {
            return NotFound();
        }

        categoryToUpdate.CategoryName = categoryDto.CategoryName;
        categoryToUpdate.Description = categoryDto.Description;
        categoryToUpdate.ParentCategoryID = categoryDto.ParentCategoryID;
        categoryToUpdate.IsActive = categoryDto.IsActive;

        await _context.SaveChangesAsync();
        return NoContent(); // Başarılı güncelleme sonrası 204 No Content döner.
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")] // Bu endpoint'e sadece "Admin" rolündekiler erişebilir.
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var categoryToDelete = await _context.ServiceCategories.FindAsync(id);
        if (categoryToDelete == null)
        {
            return NotFound();
        }

        // Kategoriyi veritabanından silmek yerine pasif hale getirmek daha güvenli bir yöntemdir.
        categoryToDelete.IsActive = false;
        await _context.SaveChangesAsync();

        return NoContent(); // Başarılı silme sonrası 204 No Content döner.
    }
}
