using System.ComponentModel.DataAnnotations;

namespace ProductExpirationTracker.Application.DTOs;

public class CreateProductDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;
    
    public string? PurchaseDate { get; set; }
    
    [Required]
    public string ExpirationDate { get; set; } = string.Empty;
    
    public decimal? Price { get; set; }
}





