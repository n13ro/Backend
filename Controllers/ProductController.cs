
using backend.Database;
using backend.Models;
using Backend.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowedHosts")]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public ProductController(AppDbContext dbContext) => _dbContext = dbContext;

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
        public async Task<IActionResult> CreateProduct([FromBody] ProdDto prodDto )
        {
            try
            {
                var newProduct = new Product
                {
                    ProductId = Guid.NewGuid(),
                    Name = prodDto.Name,
                    Description = prodDto.Desc,
                    Price = prodDto.Price,
                    Size = [.. prodDto.Size],
                    Quantity = prodDto.Quantity,
                    ArticleNumber = new Random().Next(1000000, 9999999)
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
        public async Task<IActionResult> EditProduct(Guid id, [FromBody] ProdDto prodDto)
        {
            try
            {
                var productToUpdate = await _dbContext.Products.FindAsync(id);

                if(productToUpdate == null)
                {
                    return NotFound(new { mess = "Продукта нет" });
                }
                productToUpdate.Name = prodDto.Name;
                productToUpdate.Description = prodDto.Desc;
                productToUpdate.Price = prodDto.Price;
                productToUpdate.Quantity = prodDto.Quantity;
                productToUpdate.Size = prodDto.Size;

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
