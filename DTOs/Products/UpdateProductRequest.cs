using System.ComponentModel.DataAnnotations;

namespace TiendaPromElec.DTOs.Products;

public sealed class UpdateProductRequest
{
    [Required]
    [StringLength(120, MinimumLength = 2)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [StringLength(1000, MinimumLength = 5)]
    public string Description { get; init; } = string.Empty;

    [Required]
    [StringLength(80, MinimumLength = 2)]
    public string Brand { get; init; } = string.Empty;

    [Range(typeof(decimal), "0.01", "999999999.99")]
    public decimal Price { get; init; }

    [Range(0, 1_000_000)]
    public int Stock { get; init; }

    [Required]
    [StringLength(2048)]
    [Url]
    public string ImageUrl { get; init; } = string.Empty;

    [Range(typeof(long), "1", "9223372036854775807")]
    public long CategoryId { get; init; }
}
