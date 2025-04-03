
using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class ProdDto
{
    [Required(ErrorMessage = "Название товара обязательно")]
    [MinLength(3, ErrorMessage = "Название товара должно содержать минимум 3 символа")]
    public required string Name { get; set; }
    
    [Required(ErrorMessage = "Описание товара обязательно")]
    [MinLength(10, ErrorMessage = "Описание товара должно содержать минимум 10 символов")]
    public required string Desc { get; set; }

    [Required(ErrorMessage = "Цена товара обязательна")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Цена должна быть больше 0")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Размер товара обязателен")]
    [MinLength(1, ErrorMessage = "Размер товара должен содержать минимум 1 символ")]
    public string[] Size { get; set; }

    [Required(ErrorMessage = "Количество товара обязательно")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Количество должно быть больше 0")]
    public long Quantity { get; set; }

    public List<IFormFile> images {get; set;}

}