
using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class ProdDto
{
    [Required(ErrorMessage = "Название товара обязательно")]
    public required string Name { get; set; }
    
    [Required(ErrorMessage = "Описание товара обязательно")]
    public required string Desc { get; set; }

    [Required(ErrorMessage = "Цена товара обязательна")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Размер товара обязателен")]
    public string[] Size { get; set; }

    [Required(ErrorMessage = "Количество товара обязательно")]
    public long Quantity { get; set; }

    public List<IFormFile> images {get; set;}

}