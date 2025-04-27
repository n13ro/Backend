namespace Backend.DTOs
{
    public class UpdateProductDto
    {
        public string? Name { get; set; }
        public string? Desc { get; set; }
        public decimal? Price { get; set; }
        public long? Quantity { get; set; }
        public string[]? Size { get; set; }
    }
}
