namespace TiendaPromElec.DTOs.Orders;

public sealed record OrderResponse(
    long Id,
    DateTime OrderDate,
    decimal TotalAmount,
    string Status,
    long CustomerId,
    IReadOnlyCollection<OrderItemResponse> Items);
