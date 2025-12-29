using Microsoft.AspNetCore.Mvc;
using RentConnect.API.RentConnect.Domain.Models;
using RentConnect.API.RentConnect.Infrastructure.UnitOfwork;
using RentConnect.Presentation.UI.RentConnect.Presentation.DTOs;

namespace RentConnect.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // GET: api/category
    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        var categories = await _unitOfWork.CategoryRepository.GetAllAsync();
        return Ok(categories);
    }

    // GET: api/category/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryById(Guid id)
    {
        var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound($"Category with ID {id} not found.");
        }
        return Ok(category);
    }

    // GET: api/category/with-products
    [HttpGet("with-products")]
    public async Task<IActionResult> GetCategoriesWithProducts()
    {
        var categories = await _unitOfWork.CategoryRepository.GetCategoriesWithItemsAsync();
        return Ok(categories);
    }

    // POST: api/category
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] Category category)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _unitOfWork.CategoryRepository.AddAsync(category);
        await _unitOfWork.CompleteAsync();

        return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
    }

    // PUT: api/category/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] Category category)
    {
        if (id != category.Id)
        {
            return BadRequest("Category ID mismatch.");
        }

        var existingCategory = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
        if (existingCategory == null)
        {
            return NotFound($"Category with ID {id} not found.");
        }

        existingCategory.Name = category.Name;
        await _unitOfWork.CompleteAsync();

        return NoContent();
    }

    // DELETE: api/category/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound($"Category with ID {id} not found.");
        }

        await _unitOfWork.CategoryRepository.DeleteAsync(id);
        await _unitOfWork.CompleteAsync();

        return NoContent();
    }

    // POST: api/category/add-products
    [HttpPost("add-products")]
    public async Task<IActionResult> AddProductsToCategory([FromBody] CategoryItemDto categoryProductDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Validate if the category exists
        var category = await _unitOfWork.CategoryRepository.GetByIdAsync(categoryProductDto.CategoryId);
        if (category == null)
        {
            return NotFound($"Category with ID {categoryProductDto.CategoryId} not found.");
        }

        // Add each product to the category
        foreach (var itemId in categoryProductDto.ItemsIds)
        {
            var product = await _unitOfWork.ItemRepository.GetByIdAsync(itemId);
            if (product == null)
            {
                return NotFound($"Product with ID {itemId} not found.");
            }

            var categoryProduct = new CategoryItem
            {
                CategoryId = categoryProductDto.CategoryId,
                ItemId = itemId
            };

            await _unitOfWork.CategoryItemRepository.AddAsync(categoryProduct);
        }

        // Save changes
        await _unitOfWork.CompleteAsync();
        return Ok("Products successfully added to the category.");
    }




}




