using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentConnect.API.RentConnect.Domain.Models;
using RentConnect.API.RentConnect.Infrastructure.UnitOfwork;
using RentConnect.API.RentConnect.Presentation.DTOs;
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
                ImageUrls = item.Images?.Select(img => img.ImageUrl).ToList() ?? new List<string>(),
                Images = item.Images?.Select(img => new ItemImageDto
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl
                }).ToList() ?? new List<ItemImageDto>()
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
                ImageUrls = item.Images?.Select(img => img.ImageUrl).ToList() ?? new List<string>(),
                Images = item.Images?.Select(img => new ItemImageDto
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl
                }).ToList() ?? new List<ItemImageDto>()
            };

            return Ok(itemDto);
        }

        [HttpGet("cities")]
        public async Task<ActionResult<List<string>>> GetCities()
        {
            var items = await _unitOfWork.ItemRepository.GetAllAsync();
            var cities = items
                .Select(i => i.City)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(c => c.Trim())
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            return Ok(cities);
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

        // ✅ POST: api/Items/create-with-images  (مع فلتر للصور فقط)
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
                Images = new List<ItemImage>()
            };

            await _unitOfWork.ItemRepository.AddAsync(newItem);
            await _unitOfWork.CompleteAsync();

            // لو فيه صور مرفوعة
            if (itemDto.Images != null && itemDto.Images.Count > 0)
            {
                var uploadsFolder = _configuration["UploadSettings:ItemsImagesPath"];

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // ✅ أنواع الـ Content-Type المسموحة
                var allowedTypes = new[]
                {
                    "image/jpeg",
                    "image/jpg",
                    "image/png",
                    "image/webp",
                    "image/gif"
                };

                // ✅ الإمتدادات المسموحة
                var allowedExtensions = new[]
                {
                    ".jpg", ".jpeg", ".png", ".webp", ".gif"
                };

                // حجم الصورة الأقصى (مثال: 20MB)
                long maxFileSize = 20 * 1024 * 1024;

                foreach (var file in itemDto.Images)
                {
                    if (file == null || file.Length == 0)
                        continue;

                    // 1) حجم الملف
                    if (file.Length > maxFileSize)
                    {
                        return BadRequest($"File '{file.FileName}' is too large. Max size is 20MB.");
                    }

                    // 2) نوع الـ ContentType
                    var contentType = file.ContentType?.ToLowerInvariant() ?? "";
                    if (!allowedTypes.Contains(contentType))
                    {
                        return BadRequest($"Only image files are allowed. '{file.FileName}' is not a valid image type.");
                    }

                    // 3) الإمتداد
                    var ext = Path.GetExtension(file.FileName)?.ToLowerInvariant() ?? "";
                    if (!allowedExtensions.Contains(ext))
                    {
                        return BadRequest(
                            $"Only these image types are allowed: {string.Join(", ", allowedExtensions)}. Invalid file: '{file.FileName}'.");
                    }

                    // 4) حفظ الملف
                    var safeName = Path.GetFileNameWithoutExtension(file.FileName);
                    var fileName = $"{Guid.NewGuid()}_{safeName}{ext}";
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

                await _unitOfWork.CompleteAsync();
            }

            return CreatedAtAction(nameof(GetItemById), new { id = newItem.Id }, null);
        }

        [HttpPut("{id:guid}/update-with-images")]
        public async Task<IActionResult> UpdateWithImages(Guid id, [FromForm] ItemUpdateWithImagesDto dto)
        {
            if (id != dto.Id) return BadRequest("Item ID mismatch.");

            var item = await _unitOfWork.ItemRepository.GetByIdAsync(id);
            if (item == null) return NotFound("Item not found.");

            // ✅ تحديث البيانات الأساسية
            item.Name = dto.Name.Trim();
            item.Description = dto.Description.Trim();
            item.PricePerDay = (double)dto.PricePerDay;
            item.CategoryId = dto.CategoryId;
            item.City = dto.City.Trim();
            item.IsShippable = dto.IsShippable;
            item.Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim();

            // ✅ حذف صور قديمة (من DB + حذف الملف إن تبي)
            if (dto.DeletedImageIds != null && dto.DeletedImageIds.Count > 0)
            {
                foreach (var imgId in dto.DeletedImageIds.Distinct())
                {
                    await _unitOfWork.ItemImageRepository.DeleteAsync(imgId);
                }
            }

            // ✅ إضافة صور جديدة
            if (dto.NewImages != null && dto.NewImages.Count > 0)
            {
                var uploadsFolder = _configuration["UploadSettings:ItemsImagesPath"];
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp", "image/gif" };
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
                long maxFileSize = 10 * 1024 * 1024; // 10MB

                foreach (var file in dto.NewImages)
                {
                    if (file == null || file.Length == 0) continue;

                    if (file.Length > maxFileSize)
                        return BadRequest($"File '{file.FileName}' is too large. Max 10MB.");

                    var contentType = file.ContentType?.ToLowerInvariant() ?? "";
                    if (!allowedTypes.Contains(contentType))
                        return BadRequest($"Only image files are allowed. '{file.FileName}' is invalid.");

                    var ext = Path.GetExtension(file.FileName)?.ToLowerInvariant() ?? "";
                    if (!allowedExtensions.Contains(ext))
                        return BadRequest($"Invalid file extension: '{file.FileName}'.");

                    var safeName = Path.GetFileNameWithoutExtension(file.FileName);
                    var fileName = $"{Guid.NewGuid()}_{safeName}{ext}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(stream);

                    var image = new ItemImage
                    {
                        ItemId = item.Id,
                        ImageUrl = $"/uploads/items/{fileName}"
                    };

                    await _unitOfWork.ItemImageRepository.AddAsync(image);
                }
            }

            await _unitOfWork.CompleteAsync();
            return NoContent();
        }


        // PATCH: api/Items/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateItem(Guid id, [FromBody] ItemUpdateDto dto)
        {
            if (dto == null) return BadRequest("Updated item data is required.");
            if (id != dto.Id) return BadRequest("Item ID mismatch.");

            var existingItem = await _unitOfWork.ItemRepository.GetByIdAsync(id);
            if (existingItem == null) return NotFound($"Item with ID {id} not found.");

            existingItem.Name = dto.Name;
            existingItem.Description = dto.Description;
            existingItem.PricePerDay = dto.PricePerDay;
            existingItem.City = dto.City;
            existingItem.Notes = dto.Notes;
            existingItem.IsShippable = dto.IsShippable;
            existingItem.CategoryId = dto.CategoryId;

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

            var bookings = await _unitOfWork.BookingRepository.GetByItemIdAsync(id);
            if (bookings.Any())
                return BadRequest("You cannot delete this item because there are bookings associated with it.");

            await _unitOfWork.ItemRepository.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
    }
}