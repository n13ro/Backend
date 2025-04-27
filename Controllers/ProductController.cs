
using Backend.Database;
using Backend.Models;
using Backend.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowedHosts")]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly MinioService _minioService;

        public ProductController(AppDbContext dbContext, MinioService minioService)
        {
            _dbContext = dbContext;
            _minioService = minioService;
        }

        [HttpGet("getAll")]
        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            var allProd = await _dbContext.Products.ToListAsync();

            GC.Collect();
            return allProd;
        }

        [HttpGet("getById")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var product = await _dbContext.Products
                .FirstAsync(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound(new { mess = "Такого товара нет" });
            }
            GC.Collect();
            return Ok(product);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("createProd")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateProduct([FromForm] ProdDto prodDto)
        {
            try
            {
                // Проверка наличия файла
                if (prodDto.images == null || !prodDto.images.Any())
                {
                    return BadRequest(new { message = "Изображение обязательно" });
                }
                var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
                // Проверка типа файла
                foreach (var image in prodDto.images)
                {
                    if (!allowedTypes.Contains(image.ContentType.ToLower()))
                    {
                        return BadRequest(new { message = "Допустимы только изображения форматов: JPEG, PNG, GIF, WEBP" });
                    }

                    if (image.Length > 10 * 1024 * 1024) // 10MB
                    {
                        return BadRequest(new { message = $"Размер файла {image.FileName} не должен превышать 5 МБ" });
                    }
                }

                var imageUrls = new List<string>();

                foreach (var image in prodDto.images)
                {
                    try
                    {
                        string imageUrl = await _minioService.UploadFileAsync(image);
                        imageUrls.Add(imageUrl);
                    }
                    catch (Exception ex)
                    {
                        BadRequest(new { mess = $"Ошибка при загрузке изображения {image.FileName}" });

                        // Удаляем уже загруженные изображения в случае ошибки
                        foreach (var url in imageUrls)
                        {
                            try
                            {
                                // Извлекаем имя файла из URL
                                Uri uri = new Uri(url);
                                string fileName = uri.Segments.Last();
                                await _minioService.DeleteFileAsync(fileName);
                            }
                            catch { /* Игнорируем ошибки при удалении */ }
                        }

                        return StatusCode(500, new { message = $"Ошибка при загрузке изображения: {ex.Message}" });
                    }
                }

                var newProduct = new Product
                {
                    ProductId = Guid.NewGuid(),
                    Name = prodDto.Name,
                    Description = prodDto.Desc,
                    Price = prodDto.Price,
                    Size = [.. prodDto.Size],
                    Quantity = prodDto.Quantity,
                    ArticleNumber = new Random().Next(1000000, 9999999),
                    ImageUrl = imageUrls
                    };
                
                _dbContext.Products.Add(newProduct);
                await _dbContext.SaveChangesAsync();
                GC.Collect();
                return Ok(new { mess = "Товар создан" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = "Недостаточно прав для выполнения данной операции." });
            }
            catch (Exception ex)
            {
                
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("editProd/{id}")]
        public async Task<IActionResult> EditProduct(Guid id, [FromBody] string Name, string Desc, decimal Price, long Quantity, string[] Size)
        {
            try
            {
                var productToUpdate = await _dbContext.Products.FindAsync(id);

                if(productToUpdate == null)
                {
                    return NotFound(new { mess = "Продукта нет" });
                }
                productToUpdate.Name = Name;
                productToUpdate.Description = Desc;
                productToUpdate.Price = Price;
                productToUpdate.Quantity = Quantity;
                productToUpdate.Size = Size;

                await _dbContext.SaveChangesAsync();
                GC.Collect();
                return Ok(new { mess = "Товар изменен" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = "Недостаточно прав для выполнения данной операции." });
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "Ошибка обновления. Возможно, данные были изменены другим пользователем." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delById")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var product = await _dbContext.Products
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if(product == null)
            {
                return NotFound(new { mess = "Продукта нет" });
            }
            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();
            GC.Collect();
            return Ok(new { mess = "Продукт удален!"});
        }

    }
}
