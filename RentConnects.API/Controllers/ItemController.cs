using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentConnect.API.RentConnect.Domain.Models;
using RentConnect.API.RentConnect.Infrastructure.UnitOfwork;
using RentConnect.Presentation.UI.RentConnect.Presentation.DTOs;

namespace RentConnect.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public ItemController(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        // GET: api/Items
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetAllItems()
        {
            var items = await _unitOfWork.ItemRepository.GetAllAsync();
            if (!items.Any())
            {
                return NotFound("No items found.");
            }

            var itemDtos = items.Select(item => new ItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                PricePerDay = item.PricePerDay,
                OwnerId = item.OwnerId,
                OwnerUserName = item.Owner?.UserName,
                OwnerFullName = item.Owner?.FullName,
                OwnerEmail = item.Owner?.Email,
                OwnerPhone = item.Owner?.PhoneNumber,
                CategoryId = item.CategoryId,
                CategoryName = item.Category?.Name,
                City = item.City,
                IsShippable = item.IsShippable,
                Notes = item.Notes,
                ImageUrls = item.Images?.Select(img => img.ImageUrl).ToList() ?? new List<string>()
            }).ToList();

            return Ok(itemDtos);
        }

        // GET: api/Items/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetItemById(Guid id)
        {
            var item = await _unitOfWork.ItemRepository.GetByIdAsync(id);
            if (item == null)
                return NotFound($"Item with ID {id} not found.");

            var itemDto = new ItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                PricePerDay = item.PricePerDay,
                OwnerId = item.OwnerId,
                OwnerUserName = item.Owner?.UserName,
                OwnerFullName = item.Owner?.FullName,
                OwnerEmail = item.Owner?.Email,
                OwnerPhone = item.Owner?.PhoneNumber,
                CategoryId = item.CategoryId,
                CategoryName = item.Category?.Name,
                City = item.City,
                IsShippable = item.IsShippable,
                Notes = item.Notes,
                ImageUrls = item.Images?.Select(img => img.ImageUrl).ToList() ?? new List<string>()
            };

            return Ok(itemDto);
        }

        // POST: api/Items
        [HttpPost]
        public async Task<IActionResult> CreateItem([FromBody] ItemDto itemDto)
        {
            if (itemDto == null)
                return BadRequest("Item data is required.");

            var newItem = new Item
            {
                Name = itemDto.Name,
                Description = itemDto.Description,
                PricePerDay = itemDto.PricePerDay,
                OwnerId = itemDto.OwnerId,
                CategoryId = itemDto.CategoryId,
                City = itemDto.City,
                IsShippable = itemDto.IsShippable,
                Notes = itemDto.Notes
            };

            await _unitOfWork.ItemRepository.AddAsync(newItem);
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(GetItemById), new { id = newItem.Id }, null);
        }

        // POST: api/Items/create-with-images
        [HttpPost("create-with-images")]
        public async Task<IActionResult> CreateItemWithImages([FromForm] ItemWithImagesDto itemDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newItem = new Item
            {
                Name = itemDto.Name,
                Description = itemDto.Description,
                PricePerDay = itemDto.PricePerDay,
                OwnerId = itemDto.OwnerId,
                CategoryId = itemDto.CategoryId,
                City = itemDto.City,
                IsShippable = itemDto.IsShippable,
                Notes = itemDto.Notes,
                Images = new List<ItemImage>() // نجهز القائمة عشان الصور
            };

            await _unitOfWork.ItemRepository.AddAsync(newItem);
            await _unitOfWork.CompleteAsync();

            if (itemDto.Images != null && itemDto.Images.Count > 0)
            {
                var uploadsFolder = _configuration["UploadSettings:ItemsImagesPath"];

                // نتأكد ان المسار موجود
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                foreach (var file in itemDto.Images)
                {
                    if (file.Length > 0)
                    {
                        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var image = new ItemImage
                        {
                            ItemId = newItem.Id,
                            ImageUrl = $"/uploads/items/{fileName}"
                        };

                        await _unitOfWork.ItemImageRepository.AddAsync(image);
                        newItem.Images.Add(image);
                    }
                }

                await _unitOfWork.CompleteAsync();
            }

            return CreatedAtAction(nameof(GetItemById), new { id = newItem.Id }, null);
        }


        // PATCH: api/Items/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateItem(Guid id, [FromBody] ItemDto itemDto)
        {
            if (itemDto == null)
                return BadRequest("Updated item data is required.");

            if (id != itemDto.Id)
                return BadRequest("Item ID mismatch.");

            var existingItem = await _unitOfWork.ItemRepository.GetByIdAsync(id);
            if (existingItem == null)
                return NotFound($"Item with ID {id} not found.");

            existingItem.Name = !string.IsNullOrWhiteSpace(itemDto.Name) ? itemDto.Name : existingItem.Name;
            existingItem.Description = !string.IsNullOrWhiteSpace(itemDto.Description) ? itemDto.Description : existingItem.Description;
            existingItem.PricePerDay = itemDto.PricePerDay >= 0 ? itemDto.PricePerDay : existingItem.PricePerDay;
            existingItem.OwnerId = !string.IsNullOrWhiteSpace(itemDto.OwnerId) ? itemDto.OwnerId : existingItem.OwnerId;
            existingItem.City = !string.IsNullOrWhiteSpace(itemDto.City) ? itemDto.City : existingItem.City;
            existingItem.Notes = !string.IsNullOrWhiteSpace(itemDto.Notes) ? itemDto.Notes : existingItem.Notes;
            existingItem.IsShippable = itemDto.IsShippable;

            if (itemDto.CategoryId != Guid.Empty && itemDto.CategoryId != existingItem.CategoryId)
            {
                existingItem.CategoryId = itemDto.CategoryId;
            }

            await _unitOfWork.ItemRepository.UpdateAsync(existingItem);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        // DELETE: api/Items/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            var item = await _unitOfWork.ItemRepository.GetByIdAsync(id);
            if (item == null)
                return NotFound($"Item with ID {id} not found.");

            await _unitOfWork.ItemRepository.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
    }
}
