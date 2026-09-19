namespace TiendaPromElec.DTOs.Orders;

public sealed record OrderItemResponse(
    long Id,
    long ProductId,
    int Quantity,
    decimal UnitPrice);
