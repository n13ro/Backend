
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
                
            var productResponses = allProd.Select(product => new Product
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Size = [..product.Size],
               
            }).ToList();
            GC.Collect();
            return productResponses;
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
            var productResponse = new Product
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Size = [..product.Size],
            };
            GC.Collect();
            return Ok(productResponse);
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

            return Ok(new { mess = "Продукт удален!"});
        }

    }
}
