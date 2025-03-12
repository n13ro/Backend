
using backend.Database;
using backend.Models;
using Backend.DTOs;
using Backend.Enums;
using Backend.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public ProductController(AppDbContext dbContext) => _dbContext = dbContext;

        [HttpGet]
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
            
            return productResponses;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var product = await _dbContext.Products
                .FirstAsync(p => p.ProductId == id);

            var productResponse = new Product
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Size = [..product.Size],
            };

            return Ok(productResponse);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm] ProdDto prodDto )
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

        //[Authorize(Policy = IdentityData.AdminUserPolicyName)]
        [Authorize]
        [HttpDelete("{id}")]
        public async Task DeleteProduct(Guid id)
        {
            await _dbContext.Products.Where(x => x.ProductId == id).ExecuteDeleteAsync();

        }

    }
}
