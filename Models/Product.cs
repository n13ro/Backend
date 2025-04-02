using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class Product
    {
        [Key]
        public required Guid ProductId { get; set; }
        public long ArticleNumber { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required decimal Price { get; set; }
        
        public required ICollection<string> Size { get; set; }

        public long Quantity { get; set; }

        public ICollection<string> ImageUrl { get; set; }
    }
}
