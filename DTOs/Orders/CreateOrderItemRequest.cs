using System.ComponentModel.DataAnnotations;

namespace TiendaPromElec.DTOs.Orders;

public sealed class CreateOrderItemRequest
{
    [Range(typeof(long), "1", "9223372036854775807")]
    public long ProductId { get; init; }

    [Range(1, 1000)]
    public int Quantity { get; init; }
}
