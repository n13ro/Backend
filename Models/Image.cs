using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Image
    {
        [Key]
        public Guid ImageId { get; set; }
        public required byte[] ImageData { get; set; }
        public string? ImageName { get;  set; }
        
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
