namespace TiendaPromElec.DTOs.Products;

public sealed record ProductResponse(
    long Id,
    string Name,
    string Description,
    string Brand,
    decimal Price,
    int Stock,
    string ImageUrl,
    long CategoryId);
