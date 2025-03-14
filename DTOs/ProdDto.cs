
namespace Backend.DTOs;

public class ProdDto
{
    public required string Name { get; set; }
    public required string Desc { get; set; }
    public required decimal Price { get; set; }
    public required string[] Size { get; set; }

    public long Quantity { get; set; } = 0;
}